public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<MediaDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                   .UseSnakeCaseNamingConvention());

        services.AddKeyedSingleton<IMinioClient>("internal", (_, _) => new MinioClient()
            .WithEndpoint(configuration["MinIO:Endpoint"]!)
            .WithCredentials(configuration["MinIO:AccessKey"]!, configuration["MinIO:SecretKey"]!)
            .WithSSL(false)
            .Build());

        services.AddKeyedSingleton<IMinioClient>("public", (_, _) => new MinioClient()
            .WithEndpoint(configuration["MinIO:PublicEndpoint"]!)
            .WithCredentials(configuration["MinIO:AccessKey"]!, configuration["MinIO:SecretKey"]!)
            .WithSSL(false)
            .Build());

        services.AddScoped<IStorageService, MinioStorageService>();
        services.AddScoped<IMediaRepository, MediaRepository>();

        var applicationAssembly = typeof(UploadMediaCommand).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
        });

        services.AddValidatorsFromAssembly(applicationAssembly);

        return services;
    }
}