public sealed record UpdateAdoptionStatusCommand(
    Guid Id,
    EAdoptionStatus Status
) : IRequest<AdoptionResponseDto>;
