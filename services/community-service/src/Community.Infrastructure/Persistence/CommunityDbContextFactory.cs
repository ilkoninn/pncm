public sealed class CommunityDbContextFactory : IDesignTimeDbContextFactory<CommunityDbContext>
{
    public CommunityDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CommunityDbContext>();
        optionsBuilder
            .UseNpgsql("Host=localhost;Database=pncm_community;Username=postgres;Password=postgres")
            .UseSnakeCaseNamingConvention();

        return new CommunityDbContext(optionsBuilder.Options);
    }
}
