public sealed record GetPetsByOwnerQuery(
    Guid OwnerId,
    EOwnerType OwnerType
) : IRequest<IEnumerable<PetResponseDto>>;
