public sealed class GetPetsByOwnerQueryHandler(IPetRepository petRepository, IMediaGrpcClient mediaGrpcClient)
    : IRequestHandler<GetPetsByOwnerQuery, IEnumerable<PetResponseDto>>
{
    public async Task<IEnumerable<PetResponseDto>> Handle(GetPetsByOwnerQuery request, CancellationToken cancellationToken)
    {
        var pets = await petRepository.GetByOwnerAsync(request.OwnerId, EOwnerType.User, request.Type, cancellationToken);

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
