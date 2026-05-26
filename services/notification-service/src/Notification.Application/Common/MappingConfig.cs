public static class MappingConfig
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Notification, NotificationResponseDto>();
    }
}
