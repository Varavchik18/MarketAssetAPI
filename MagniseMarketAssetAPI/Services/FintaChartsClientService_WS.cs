using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using MagniseMarketAssetAPI.Services;

public class FintaChartsClientService_WS
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;
    private readonly TokenStore _tokenStore;
    private readonly Uri _webSocketUri;
    private ClientWebSocket _webSocket;
    private readonly RealTimeDataStore _realTimeDataStore = new RealTimeDataStore();
    private readonly IHubContext<RealTimePriceHub> _hubContext;

    public FintaChartsClientService_WS(HttpClient client, IConfiguration configuration, TokenStore tokenStore, IHubContext<RealTimePriceHub> hubContext)
    {
        _client = client;
        _configuration = configuration;
        _tokenStore = tokenStore;
        _webSocketUri = new Uri(_configuration["Fintacharts:URI_WSS"]);
        _webSocket = new ClientWebSocket();
        _hubContext = hubContext;
    }

    private async Task EnsureConnectedAsync()
    {
        if (_webSocket.State != WebSocketState.Open)
        {
            await ConnectAsync();
        }
    }

    public async Task ConnectAsync()
    {
        var accessToken = _tokenStore.GetAccessToken();
        var uriWithToken = new Uri($"{_webSocketUri}?token={accessToken}");
        await _webSocket.ConnectAsync(uriWithToken, CancellationToken.None);
    }

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

        _ = Task.Run(() => ReceiveMessagesAsync(instrumentId));
    }

    public async Task UnsubscribeAsync(string instrumentId, string provider)
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

    private async Task ReceiveMessagesAsync(string instrumentId)
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
                    if (realTimeData != null)
                    {
                        await _hubContext.Clients.All.SendAsync("ReceivePriceUpdate", realTimeData);
                    }
                }
            }
        }
    }

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

    public RealTimePriceDataDTO GetRealTimeData(string instrumentId)
    {
        return _realTimeDataStore.GetData(instrumentId);
    }
}
