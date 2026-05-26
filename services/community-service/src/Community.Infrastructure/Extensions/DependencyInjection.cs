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

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

        services.AddScoped<IPostRepository, PostRepository>();
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

        var mapsterConfig = TypeAdapterConfig.GlobalSettings;
        MappingConfig.Register(mapsterConfig);
        services.AddSingleton(mapsterConfig);

        return services;
    }
}
