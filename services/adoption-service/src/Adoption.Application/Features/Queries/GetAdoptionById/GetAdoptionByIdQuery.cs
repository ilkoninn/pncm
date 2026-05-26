public sealed record GetAdoptionByIdQuery(Guid Id) : IRequest<AdoptionResponseDto>;
