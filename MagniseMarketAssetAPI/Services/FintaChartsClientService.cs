using System;
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

    public FintaChartsClientService(HttpClient client, IConfiguration configuration, TokenStore tokenStore)
    {
        _client = client;
        _configuration = configuration;
        _tokenStore = tokenStore;
    }

    public async Task<FintachartAPIResponseDTO> GetAssetsListAsync(string provider = null, string kind = null, string symbol = null, int page = 1, int size = 10)
    {
        var token = _tokenStore.GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException("Unable to get access token.");
        }

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
        var assetResponse = JsonSerializer.Deserialize<FintachartAPIResponseDTO>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        });

        return assetResponse;
    }
}
