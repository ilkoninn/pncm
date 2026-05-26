public static class MappingConfig
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Post, PostResponseDto>();
        config.NewConfig<Contest, ContestResponseDto>();
        config.NewConfig<ContestEntry, ContestEntryResponseDto>();
        config.NewConfig<Invite, InviteResponseDto>();
    }
}
