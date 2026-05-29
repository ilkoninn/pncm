public sealed record GetPetBySlugQuery(string Slug) : IRequest<PetResponseDto>;
