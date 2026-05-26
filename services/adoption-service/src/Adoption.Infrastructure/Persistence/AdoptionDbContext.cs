public sealed class AdoptionDbContext(DbContextOptions<AdoptionDbContext> options) : DbContext(options)
{
    public DbSet<AdoptionRequest> AdoptionRequests => Set<AdoptionRequest>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AdoptionDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
}
