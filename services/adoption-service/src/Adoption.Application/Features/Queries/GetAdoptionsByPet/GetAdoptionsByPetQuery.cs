public sealed record GetAdoptionsByPetQuery(Guid PetId, Guid RequesterId) : IRequest<IEnumerable<AdoptionResponseDto>>;
