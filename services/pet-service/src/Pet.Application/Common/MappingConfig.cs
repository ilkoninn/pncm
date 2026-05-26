public static class MappingConfig
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<PetPhoto, PetPhotoResponseDto>();

        config.NewConfig<Pet, PetResponseDto>()
            .Map(dest => dest.Photos, src => src.Photos.Adapt<IEnumerable<PetPhotoResponseDto>>());
    }
}
