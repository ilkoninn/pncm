public record VerifyOtpCommand(
    Guid UserId,
    string Code,
    EOtpPurpose Purpose
) : IRequest<bool>;