using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class TokenService
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;
    private readonly TokenStore _tokenStore;
    private readonly ILogger<TokenService> _logger;

    public TokenService(HttpClient client, IConfiguration configuration, TokenStore tokenStore, ILogger<TokenService> logger)
    {
        _client = client;
        _configuration = configuration;
        _tokenStore = tokenStore;
        _logger = logger;
    }

    public async Task SetTokenAsync()
    {
        _logger.LogInformation("TokenService: SetTokenAsync is called.");

        var uri = $"{_configuration["Fintacharts:URI"]}/identity/realms/fintatech/protocol/openid-connect/token";
        var username = _configuration["Fintacharts:USERNAME"];
        var password = _configuration["Fintacharts:PASSWORD"];
        var grantType = "password";
        var clientId = "app-cli";

        var content = new StringContent($"grant_type={grantType}&client_id={clientId}&username={username}&password={password}", Encoding.UTF8, "application/x-www-form-urlencoded");
        var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = content
        };

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var responseObject = JObject.Parse(responseString);
        var accessToken = responseObject["access_token"].ToString();
        var expiresIn = int.Parse(responseObject["expires_in"].ToString());

        var refreshToken = responseObject["refresh_token"].ToString();
        var refreshExpiresIn = int.Parse(responseObject["refresh_expires_in"].ToString());

        _tokenStore.SetToken(accessToken, expiresIn, refreshToken, refreshExpiresIn);
    }
}
