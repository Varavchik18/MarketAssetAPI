public class StartupHostedService : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<StartupHostedService> _logger;

    public StartupHostedService(IServiceScopeFactory serviceScopeFactory, ILogger<StartupHostedService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StartupHostedService is starting.");

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();
            try
            {
                await tokenService.SetTokenAsync();
                _logger.LogInformation("TokenService: SetTokenAsync successfully called.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the token.");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StartupHostedService is stopping.");
        return Task.CompletedTask;
    }
}
