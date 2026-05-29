public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<AuditableEntityInterceptor>();

        services.AddDbContext<AdoptionDbContext>((sp, options) =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>()));

        services.AddScoped<IAdoptionRepository, AdoptionRepository>();
        services.AddSingleton<IPetGrpcClient, PetGrpcClient>();

        var applicationAssembly = typeof(CreateAdoptionCommand).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(applicationAssembly);

        var mapsterConfig = TypeAdapterConfig.GlobalSettings;
        MappingConfig.Register(mapsterConfig);
        services.AddSingleton(mapsterConfig);

        services.AddMassTransit(x =>
        {
            x.UsingInMemory((ctx, cfg) => cfg.ConfigureEndpoints(ctx));
            x.AddRider(rider =>
            {
                rider.AddProducer<AdoptionRequestedEvent>("adoption-requested");
                rider.AddProducer<AdoptionApprovedEvent>("adoption-approved");
                rider.AddProducer<AdoptionRejectedEvent>("adoption-rejected");
                rider.AddProducer<AdoptionCompletedEvent>("adoption-completed");
                rider.UsingKafka((ctx, k) =>
                {
                    k.Host(configuration["Kafka:BootstrapServers"]);
                });
            });
        });

        return services;
    }
}
