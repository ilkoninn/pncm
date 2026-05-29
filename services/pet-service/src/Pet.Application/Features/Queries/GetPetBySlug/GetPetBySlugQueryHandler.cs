public sealed class GetPetBySlugQueryHandler(IPetRepository petRepository, IMediaGrpcClient mediaGrpcClient)
    : IRequestHandler<GetPetBySlugQuery, PetResponseDto>
{
    public async Task<PetResponseDto> Handle(GetPetBySlugQuery request, CancellationToken cancellationToken)
    {
        var pet = await petRepository.GetBySlugAsync(request.Slug, cancellationToken)
            ?? throw new KeyNotFoundException("Heyvan tapılmadı.");

        var dto = pet.Adapt<PetResponseDto>();

        if (pet.Photos is not { Count: > 0 })
            return dto;

        Dictionary<Guid, string> urlMap;
        try { urlMap = await mediaGrpcClient.GetPhotosByOwnerAsync(pet.Id, ownerType: 2, cancellationToken); }
        catch { urlMap = []; }

        var enrichedPhotos = pet.Photos.Select(p =>
            new PetPhotoResponseDto(p.Id, p.MediaId, p.IsPrimary,
                urlMap.TryGetValue(p.MediaId, out var url) ? url : null));

        return dto with { Photos = enrichedPhotos };
    }
}
