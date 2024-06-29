using System.Text;
using Newtonsoft.Json.Linq;

/// <summary>
/// The TokenService class handles the acquisition and storage of access and refresh tokens
/// from the Fintacharts authentication service.
/// </summary>
public class TokenService
{
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;
    private readonly TokenStore _tokenStore;
    private readonly ILogger<TokenService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenService"/> class.
    /// </summary>
    /// <param name="client">The HttpClient instance to be used for making HTTP requests.</param>
    /// <param name="configuration">The configuration instance to access application settings.</param>
    /// <param name="tokenStore">The token store instance to save the acquired tokens.</param>
    /// <param name="logger">The logger instance to log information and errors.</param>
    public TokenService(HttpClient client, IConfiguration configuration, TokenStore tokenStore, ILogger<TokenService> logger)
    {
        _client = client;
        _configuration = configuration;
        _tokenStore = tokenStore;
        _logger = logger;
    }

    /// <summary>
    /// Asynchronously sets the access and refresh tokens by making a request to the authentication service.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
