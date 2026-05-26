public sealed record GetAdoptionsByPetQuery(Guid PetId) : IRequest<IEnumerable<AdoptionResponseDto>>;
