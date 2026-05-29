public sealed class RefreshTokenCleanupService(IServiceScopeFactory scopeFactory, ILogger<RefreshTokenCleanupService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);

            try
            {
                using var scope = scopeFactory.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                await userRepository.PurgeExpiredRefreshTokensAsync();
                logger.LogInformation("Expired refresh tokens purged.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Refresh token cleanup failed.");
            }
        }
    }
}
