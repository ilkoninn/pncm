public sealed class GetPetByIdQueryHandler(IPetRepository petRepository, IMediaGrpcClient mediaGrpcClient)
    : IRequestHandler<GetPetByIdQuery, PetResponseDto>
{
    public async Task<PetResponseDto> Handle(GetPetByIdQuery request, CancellationToken cancellationToken)
    {
        var pet = await petRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Heyvan tapılmadı.");

        return await EnrichWithPhotos(pet, cancellationToken);
    }

    private async Task<PetResponseDto> EnrichWithPhotos(Pet pet, CancellationToken ct)
    {
        var dto = pet.Adapt<PetResponseDto>();

        if (pet.Photos is not { Count: > 0 })
            return dto;

        Dictionary<Guid, string> urlMap;
        try { urlMap = await mediaGrpcClient.GetPhotosByOwnerAsync(pet.Id, ownerType: 2, ct); }
        catch { urlMap = []; }

        var enrichedPhotos = pet.Photos.Select(p =>
            new PetPhotoResponseDto(p.Id, p.MediaId, p.IsPrimary,
                urlMap.TryGetValue(p.MediaId, out var url) ? url : null));

        return dto with { Photos = enrichedPhotos };
    }
}
