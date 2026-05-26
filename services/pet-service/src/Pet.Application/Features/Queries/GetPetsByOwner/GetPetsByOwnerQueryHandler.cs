public sealed class GetPetsByOwnerQueryHandler(IPetRepository petRepository)
    : IRequestHandler<GetPetsByOwnerQuery, IEnumerable<PetResponseDto>>
{
    public async Task<IEnumerable<PetResponseDto>> Handle(GetPetsByOwnerQuery request, CancellationToken cancellationToken)
    {
        var pets = await petRepository.GetByOwnerAsync(request.OwnerId, request.OwnerType, cancellationToken);

        return pets.Adapt<IEnumerable<PetResponseDto>>();
    }
}
