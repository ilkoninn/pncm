public sealed record GetPetsByOwnerQuery(
    Guid OwnerId
) : IRequest<IEnumerable<PetResponseDto>>;
