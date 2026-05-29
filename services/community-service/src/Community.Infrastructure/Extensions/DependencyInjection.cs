public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<AuditableEntityInterceptor>();

        services.AddDbContext<CommunityDbContext>((sp, options) =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>()));

        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisConnectionString));
        }

        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IMediaGrpcClient, MediaGrpcClient>();
        services.AddScoped<IContestRepository, ContestRepository>();
        services.AddScoped<IContestEntryRepository, ContestEntryRepository>();
        services.AddScoped<IInviteRepository, InviteRepository>();

        var applicationAssembly = typeof(CreatePostCommand).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(applicationAssembly);

        services.AddMassTransit(x =>
        {
            x.UsingInMemory((ctx, cfg) => cfg.ConfigureEndpoints(ctx));
            x.AddRider(rider =>
            {
                rider.AddProducer<ScoreGivenEvent>("score-given");
                rider.AddProducer<ContestEndedEvent>("contest-ended");
                rider.AddConsumer<AdoptionCompletedConsumer>();
                rider.UsingKafka((ctx, k) =>
                {
                    k.Host(configuration["Kafka:BootstrapServers"]);
                    k.TopicEndpoint<Community.Infrastructure.Messaging.Contracts.AdoptionCompletedContract>(
                        "adoption-completed", "community-group",
                        e => e.ConfigureConsumer<AdoptionCompletedConsumer>(ctx));
                });
            });
        });

        var mapsterConfig = TypeAdapterConfig.GlobalSettings;
        MappingConfig.Register(mapsterConfig);
        services.AddSingleton(mapsterConfig);

        return services;
    }
}
