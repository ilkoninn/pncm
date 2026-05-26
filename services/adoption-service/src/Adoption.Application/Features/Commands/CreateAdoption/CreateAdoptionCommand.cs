public sealed record CreateAdoptionCommand(
    Guid PetId,
    Guid AdopterId,
    string Message,
    string ContactPhone
) : IRequest<AdoptionResponseDto>;
