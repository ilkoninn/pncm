public sealed class GetAllPetsQueryHandler(IPetRepository petRepository, IMediaGrpcClient mediaGrpcClient)
    : IRequestHandler<GetAllPetsQuery, IEnumerable<PetResponseDto>>
{
    public async Task<IEnumerable<PetResponseDto>> Handle(GetAllPetsQuery request, CancellationToken cancellationToken)
    {
        var pets = await petRepository.GetAllAsync(
            request.City, request.Species, request.Gender,
            request.Size, request.IsVaccinated, request.IsNeutered,
            request.ExcludeOwnerId, request.OwnerId, cancellationToken);

        var petList = pets.ToList();
        if (petList.Count == 0)
            return [];

        var petIds = petList.Select(p => p.Id);
        var photoMap = await mediaGrpcClient.GetPrimaryPhotosAsync(petIds, ownerType: 2, cancellationToken);

        return petList.Select(p =>
        {
            var dto = p.Adapt<PetResponseDto>();
            photoMap.TryGetValue(p.Id, out var url);
            return dto with { PrimaryPhotoUrl = url };
        });
    }
}
