public record SendOtpCommand(
    Guid UserId,
    EOtpPurpose Purpose
) : IRequest<Unit>;