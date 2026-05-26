public sealed record GetPetByIdQuery(Guid Id) : IRequest<PetResponseDto>;
