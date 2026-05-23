public record RequestAccessCommand(string Email, EClient Client) : IRequest<RequestAccessResponseDto>;
