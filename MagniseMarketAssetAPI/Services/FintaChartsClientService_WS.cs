using System.Net.WebSockets;
using System.Text.Json.Serialization;
using System.Text.Json;

using System.Text;

/// <summary>
/// The FintaChartsClientService_WS class manages WebSocket connections, subscriptions, and real-time data 
/// processing for the Fintacharts service.
/// </summary>
public class FintaChartsClientService_WS
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;
    private readonly TokenStore _tokenStore;
    private readonly Uri _webSocketUri;
    private ClientWebSocket _webSocket;
    private readonly RealTimeDataStore _realTimeDataStore = new RealTimeDataStore();

    /// <summary>
    /// Initializes a new instance of the <see cref="FintaChartsClientService_WS"/> class.
    /// </summary>
    /// <param name="client">The HttpClient instance to be used for making HTTP requests.</param>
    /// <param name="configuration">The configuration instance to access application settings.</param>
    /// <param name="tokenStore">The token store instance to retrieve the access token.</param>
    public FintaChartsClientService_WS(HttpClient client, IConfiguration configuration, TokenStore tokenStore)
    {
        _client = client;
        _configuration = configuration;
        _tokenStore = tokenStore;
        _webSocketUri = new Uri(_configuration["Fintacharts:URI_WSS"]);
        _webSocket = new ClientWebSocket();
    }

    /// <summary>
    /// Ensures that the WebSocket is connected. If not, it attempts to connect.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task EnsureConnectedAsync()
    {
        if (_webSocket.State != WebSocketState.Open)
        {
            await ConnectAsync();
        }
    }

    /// <summary>
    /// Connects to the WebSocket using the access token.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ConnectAsync()
    {
        var accessToken = _tokenStore.GetAccessToken();
        var uriWithToken = new Uri($"{_webSocketUri}?token={accessToken}");
        await _webSocket.ConnectAsync(uriWithToken, CancellationToken.None);
    }

    /// <summary>
    /// Subscribes to real-time data updates for a specific instrument and provider.
    /// </summary>
    /// <param name="instrumentId">The ID of the instrument to subscribe to.</param>
    /// <param name="provider">The provider of the instrument data.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SubscribeAsync(string instrumentId, string provider)
    {
        await EnsureConnectedAsync();

        var subscriptionMessage = new
        {
            type = "l1-subscription",
            id = "1",
            instrumentId = instrumentId,
            provider = provider,
            subscribe = true,
            kinds = new[] { "ask", "bid", "last" }
        };

        var messageJson = System.Text.Json.JsonSerializer.Serialize(subscriptionMessage);
        var messageBytes = Encoding.UTF8.GetBytes(messageJson);
        await _webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);

        // Start receiving messages
        await ReceiveMessagesAsync(instrumentId);
    }

    /// <summary>
    /// Unsubscribes from real-time data updates for a specific instrument and provider.
    /// </summary>
    /// <param name="instrumentId">The ID of the instrument to unsubscribe from.</param>
    /// <param name="provider">The provider of the instrument data.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task UnsubscribeAsync(string instrumentId, string provider)
    {
        var unsubscribeMessage = new
        {
            type = "l1-subscription",
            id = "1",
            instrumentId = instrumentId,
            provider = provider,
            subscribe = false,
            kinds = new[] { "ask", "bid", "last" }
        };

        var messageJson = System.Text.Json.JsonSerializer.Serialize(unsubscribeMessage);
        var messageBytes = Encoding.UTF8.GetBytes(messageJson);
        await _webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    /// <summary>
    /// Receives messages from the WebSocket and processes real-time data updates.
    /// </summary>
    /// <param name="instrumentId">The ID of the instrument for which messages are being received.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ReceiveMessagesAsync(string instrumentId)
    {
        var buffer = new byte[1024 * 4];
        while (_webSocket.State == WebSocketState.Open)
        {
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            else
            {
                var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                };

                var message = System.Text.Json.JsonSerializer.Deserialize<WebSocketMessage>(messageJson, options);

                if (message.Type == "l1-update")
                {
                    ProcessMessage(message);
                    var realTimeData = _realTimeDataStore.GetData(instrumentId);
                    if (realTimeData != null && realTimeData.Last != null && realTimeData.Ask != null && realTimeData.Bid != null)
                    {
                        await UnsubscribeAsync(instrumentId, message.Provider);
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Processes a WebSocket message and updates the real-time data store.
    /// </summary>
    /// <param name="message">The WebSocket message to process.</param>
    private void ProcessMessage(WebSocketMessage message)
    {
        PriceData last = null, ask = null, bid = null;

        if (message.Last != null)
        {
            last = new PriceData
            {
                Timestamp = message.Last.Timestamp,
                Price = message.Last.Price,
                Volume = message.Last.Volume
            };
        }

        if (message.Ask != null)
        {
            ask = new PriceData
            {
                Timestamp = message.Ask.Timestamp,
                Price = message.Ask.Price,
                Volume = message.Ask.Volume
            };
        }

        if (message.Bid != null)
        {
            bid = new PriceData
            {
                Timestamp = message.Bid.Timestamp,
                Price = message.Bid.Price,
                Volume = message.Bid.Volume
            };
        }

        _realTimeDataStore.UpdateData(message.InstrumentId, last, ask, bid);
    }

    /// <summary>
    /// Gets the real-time price data for a specific instrument.
    /// </summary>
    /// <param name="instrumentId">The ID of the instrument.</param>
    /// <returns>A <see cref="RealTimePriceDataDTO"/> containing the real-time data for the instrument.</returns>
    public RealTimePriceDataDTO GetRealTimeData(string instrumentId)
    {
        return _realTimeDataStore.GetData(instrumentId);
    }
}