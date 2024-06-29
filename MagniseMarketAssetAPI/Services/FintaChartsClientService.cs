using System;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class FintaChartsClientService
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;
    private readonly TokenStore _tokenStore;
    private readonly string _accesstoken;

    public FintaChartsClientService(HttpClient client, IConfiguration configuration, TokenStore tokenStore)
    {
        _client = client;
        _configuration = configuration;
        _tokenStore = tokenStore;
        _accesstoken = _tokenStore.GetAccessToken();
    }

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

    private class HistoricalPricesResponse
    {
        public List<HistoricalPriceDTO> Data { get; set; }
    }

}
