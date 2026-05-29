public sealed record GetPetsByOwnerQuery(
    Guid OwnerId,
    string? Type = null
) : IRequest<IEnumerable<PetResponseDto>>;
