/// <summary>
/// The TokenStore class manages the storage and retrieval of access and refresh tokens,
/// along with their expiration times.
/// </summary>
public class TokenStore
{
    private string _accessToken;
    private DateTime _tokenExpiryTime;

    private string _refreshToken;
    private DateTime _refreshTokenExpiryTime;

    /// <summary>
    /// Gets the access token if it is valid and not expired.
    /// </summary>
    /// <returns>The access token if valid; otherwise, null.</returns>
    public string GetAccessToken()
    {
        if (string.IsNullOrEmpty(_accessToken) || _tokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }

        return _accessToken;
    }

    /// <summary>
    /// Gets the refresh token if it is valid and not expired.
    /// </summary>
    /// <returns>The refresh token if valid; otherwise, null.</returns>
    public string GetRefreshToken()
    {
        if (string.IsNullOrEmpty(_refreshToken) || _refreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }

        return _refreshToken;
    }

    /// <summary>
    /// Sets the access and refresh tokens along with their expiration times.
    /// </summary>
    /// <param name="token">The access token.</param>
    /// <param name="expiresIn">The access token's time to live in seconds.</param>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="refreshTokenExpiresIn">The refresh token's time to live in seconds.</param>
    public void SetToken(string token, int expiresIn, string refreshToken, int refreshTokenExpiresIn)
    {
        _accessToken = token;
        _tokenExpiryTime = DateTime.UtcNow.AddSeconds(expiresIn);
        _refreshToken = refreshToken;
        _refreshTokenExpiryTime = DateTime.UtcNow.AddSeconds(refreshTokenExpiresIn);
    }
}
