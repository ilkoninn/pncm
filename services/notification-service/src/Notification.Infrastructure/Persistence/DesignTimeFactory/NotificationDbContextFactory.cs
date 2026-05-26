public sealed class NotificationDbContextFactory : IDesignTimeDbContextFactory<NotificationDbContext>
{
    public NotificationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<NotificationDbContext>();
        optionsBuilder
            .UseNpgsql("Host=localhost;Database=pncm_notification;Username=postgres;Password=postgres")
            .UseSnakeCaseNamingConvention();
        return new NotificationDbContext(optionsBuilder.Options);
    }
}
