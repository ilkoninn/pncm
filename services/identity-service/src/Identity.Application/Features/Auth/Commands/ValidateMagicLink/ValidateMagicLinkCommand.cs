public record ValidateMagicLinkCommand(string Token) : IRequest<string?>;
