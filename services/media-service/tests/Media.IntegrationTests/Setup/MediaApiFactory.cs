public class MediaApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine")
        .Build();

    private readonly MinioContainer _minio = new MinioBuilder("minio/minio:latest")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        await _minio.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _postgres.GetConnectionString(),
                ["MinIO:Endpoint"] = $"{_minio.Hostname}:{_minio.GetMappedPublicPort(9000)}",
                ["MinIO:PublicEndpoint"] = $"{_minio.Hostname}:{_minio.GetMappedPublicPort(9000)}",
                ["MinIO:AccessKey"] = _minio.GetAccessKey(),
                ["MinIO:SecretKey"] = _minio.GetSecretKey(),
            });
        });

        builder.ConfigureServices(services =>
        {
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MediaDbContext>();
            db.Database.Migrate();
        });
    }

    public new async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await _minio.DisposeAsync();
    }
}