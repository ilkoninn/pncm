public record VerifyAccessCommand(
    string Email,
    string Code,
    EClient Client
) : IRequest<VerifyAccessResponseDto>;
