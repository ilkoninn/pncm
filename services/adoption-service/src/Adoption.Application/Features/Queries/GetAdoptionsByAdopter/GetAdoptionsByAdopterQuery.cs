public sealed record GetAdoptionsByAdopterQuery(Guid AdopterId) : IRequest<IEnumerable<AdoptionResponseDto>>;
