public record ConfirmAdoptionCommand(Guid AdoptionId, Guid RequesterId) : IRequest<AdoptionResponseDto>;
