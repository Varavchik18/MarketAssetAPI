using System.Net.Http.Headers;
using System.Text.Json;
/// <summary>
/// The FintaChartsClientService class provides methods to interact with the Fintacharts API,
/// including retrieving lists of assets and historical price data.
/// </summary>
public class FintaChartsClientService
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;
    private readonly TokenStore _tokenStore;
    private readonly string _accesstoken;

    /// <summary>
    /// Initializes a new instance of the <see cref="FintaChartsClientService"/> class.
    /// </summary>
    /// <param name="client">The HttpClient instance to be used for making HTTP requests.</param>
    /// <param name="configuration">The configuration instance to access application settings.</param>
    /// <param name="tokenStore">The token store instance to retrieve the access token.</param>
    public FintaChartsClientService(HttpClient client, IConfiguration configuration, TokenStore tokenStore)
    {
        _client = client;
        _configuration = configuration;
        _tokenStore = tokenStore;
        _accesstoken = _tokenStore.GetAccessToken();
    }

    /// <summary>
    /// Retrieves a list of assets from the Fintacharts API.
    /// </summary>
    /// <param name="provider">The provider of the assets (optional).</param>
    /// <param name="kind">The kind of assets (optional).</param>
    /// <param name="symbol">The symbol of the assets (optional).</param>
    /// <param name="page">The page number for pagination (default is 1).</param>
    /// <param name="size">The number of assets per page (default is 10).</param>
    /// <returns>A task that represents the asynchronous operation, containing the assets response DTO.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the access token is invalid.</exception>
    public async Task<AssetsResponseDTO> GetAssetsListAsync(string provider = null, string kind = null, string symbol = null, int page = 1, int size = 10)
    {
        if (string.IsNullOrEmpty(_accesstoken))
        {
            throw new UnauthorizedAccessException("Unable to get access token.");
        }

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accesstoken);

        var uri = $"{_configuration["Fintacharts:URI"]}/api/instruments/v1/instruments?page={page}&size={size}";

        if (!string.IsNullOrEmpty(provider))
        {
            uri += $"&provider={provider}";
        }

        if (!string.IsNullOrEmpty(kind))
        {
            uri += $"&kind={kind}";
        }

        if (!string.IsNullOrEmpty(symbol))
        {
            uri += $"&symbol={symbol}";
        }

        var response = await _client.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var assetResponse = JsonSerializer.Deserialize<AssetsResponseDTO>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        });

        return assetResponse;
    }

    /// <summary>
    /// Retrieves historical price data for a specific instrument and date range from the Fintacharts API.
    /// </summary>
    /// <param name="instrumentId">The ID of the instrument.</param>
    /// <param name="provider">The provider of the instrument data.</param>
    /// <param name="interval">The interval between data points.</param>
    /// <param name="periodicity">The periodicity of the data.</param>
    /// <param name="startDate">The start date for the data range.</param>
    /// <param name="endDate">The end date for the data range.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of historical price DTOs.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the access token is invalid.</exception>
    public async Task<List<HistoricalPriceDTO>> GetHistoricalPricesDateRangeAsync(string instrumentId, string provider, int interval, string periodicity, DateTime startDate, DateTime endDate)
    {
        if (string.IsNullOrEmpty(_accesstoken))
        {
            throw new UnauthorizedAccessException("Unable to get access token.");
        }

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accesstoken);

        var uri = $"{_configuration["Fintacharts:URI"]}/api/bars/v1/bars/date-range?instrumentId={instrumentId}&provider={provider}&interval={interval}&periodicity={periodicity}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";

        var response = await _client.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var historicalPricesResponse = JsonSerializer.Deserialize<HistoricalPricesResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        });

        return historicalPricesResponse?.Data.Count > 0 ? historicalPricesResponse?.Data : throw new Exception("Api response is null for that asset");
    }

    /// <summary>
    /// Retrieves historical price data for a specific instrument and count back from the Fintacharts API.
    /// </summary>
    /// <param name="instrumentId">The ID of the instrument.</param>
    /// <param name="provider">The provider of the instrument data.</param>
    /// <param name="interval">The interval between data points.</param>
    /// <param name="periodicity">The periodicity of the data.</param>
    /// <param name="barsCount">The number of bars to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of historical price DTOs.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the access token is invalid.</exception>
    public async Task<List<HistoricalPriceDTO>> GetHistoricalPricesCountBackAsync(string instrumentId, string provider, int interval, string periodicity, int barsCount)
    {
        if (string.IsNullOrEmpty(_accesstoken))
        {
            throw new UnauthorizedAccessException("Unable to get access token.");
        }

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accesstoken);

        var uri = $"{_configuration["Fintacharts:URI"]}/api/bars/v1/bars/count-back?instrumentId={instrumentId}&provider={provider}&interval={interval}&periodicity={periodicity}&barsCount={barsCount}";

        var response = await _client.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var historicalPricesResponse = JsonSerializer.Deserialize<HistoricalPricesResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        });

        return historicalPricesResponse?.Data ?? new List<HistoricalPriceDTO>();
    }

    /// <summary>
    /// Internal class to represent the response containing historical price data.
    /// </summary>
    private class HistoricalPricesResponse
    {
        /// <summary>
        /// Gets or sets the list of historical price data.
        /// </summary>
        public List<HistoricalPriceDTO> Data { get; set; }
    }

}