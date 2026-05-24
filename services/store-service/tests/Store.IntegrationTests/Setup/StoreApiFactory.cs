public class StoreApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _postgres.GetConnectionString(),
            });
        });

       builder.ConfigureServices(services =>
        {
            var grpcClient = services.SingleOrDefault(
                d => d.ServiceType == typeof(IUserGrpcClient));
            if (grpcClient != null)
                services.Remove(grpcClient);

            var mockGrpcClient = new Mock<IUserGrpcClient>();
            mockGrpcClient
                .Setup(x => x.UserExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            services.AddScoped<IUserGrpcClient>(_ => mockGrpcClient.Object);

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
            db.Database.Migrate();
        });
    }

    public new async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }
}