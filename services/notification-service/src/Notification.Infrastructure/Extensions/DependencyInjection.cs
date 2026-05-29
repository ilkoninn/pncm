using Notification.Infrastructure.Messaging.Contracts;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<AuditableEntityInterceptor>();

        services.AddDbContext<NotificationDbContext>((sp, options) =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>()));

        services.AddScoped<INotificationRepository, NotificationRepository>();

        var applicationAssembly = typeof(CreateNotificationCommand).Assembly;

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(applicationAssembly));

        var mapsterConfig = TypeAdapterConfig.GlobalSettings;
        MappingConfig.Register(mapsterConfig);
        services.AddSingleton(mapsterConfig);

        services.AddMassTransit(x =>
        {
            x.UsingInMemory((ctx, cfg) => cfg.ConfigureEndpoints(ctx));
            x.AddRider(rider =>
            {
                rider.AddConsumer<ScoreGivenConsumer>();
                rider.AddConsumer<AdoptionRequestedConsumer>();
                rider.AddConsumer<AdoptionApprovedConsumer>();
                rider.AddConsumer<AdoptionRejectedConsumer>();
                rider.AddConsumer<AdoptionCompletedConsumer>();
                rider.AddConsumer<UserRegisteredConsumer>();
                rider.AddConsumer<ContestEndedConsumer>();
                rider.UsingKafka((ctx, k) =>
                {
                    k.Host(configuration["Kafka:BootstrapServers"]);
                    k.TopicEndpoint<ScoreGivenContract>("score-given", "notification-group", e => e.ConfigureConsumer<ScoreGivenConsumer>(ctx));
                    k.TopicEndpoint<AdoptionRequestedContract>("adoption-requested", "notification-group", e => e.ConfigureConsumer<AdoptionRequestedConsumer>(ctx));
                    k.TopicEndpoint<AdoptionApprovedContract>("adoption-approved", "notification-group", e => e.ConfigureConsumer<AdoptionApprovedConsumer>(ctx));
                    k.TopicEndpoint<AdoptionRejectedContract>("adoption-rejected", "notification-group", e => e.ConfigureConsumer<AdoptionRejectedConsumer>(ctx));
                    k.TopicEndpoint<AdoptionCompletedContract>("adoption-completed", "notification-group", e => e.ConfigureConsumer<AdoptionCompletedConsumer>(ctx));
                    k.TopicEndpoint<UserRegisteredContract>("user-registered", "notification-group", e => e.ConfigureConsumer<UserRegisteredConsumer>(ctx));
                    k.TopicEndpoint<ContestEndedContract>("contest-ended", "notification-group", e => e.ConfigureConsumer<ContestEndedConsumer>(ctx));
                });
            });
        });

        return services;
    }
}
