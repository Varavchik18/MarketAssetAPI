public class TokenStore
{
    private string _accessToken;
    private DateTime _tokenExpiryTime;

    private string _refreshToken;
    private DateTime _refreshTokenExpiryTime;

    public string GetAccessToken()
    {
        if (string.IsNullOrEmpty(_accessToken) || _tokenExpiryTime <= DateTime.UtcNow)
        {
            return null; 
        }

        return _accessToken;
    }

    public string GetRefreshToken()
    {
        if (string.IsNullOrEmpty(_refreshToken) || _refreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }

        return _refreshToken;
    }

    public void SetToken(string token, int expiresIn, string refreshToken, int refreshTokenExpiresIn)
    {
        _accessToken = token;
        _tokenExpiryTime = DateTime.UtcNow.AddSeconds(expiresIn);
        _refreshToken = refreshToken;
        _refreshTokenExpiryTime = DateTime.UtcNow.AddSeconds(refreshTokenExpiresIn);
    }
}
