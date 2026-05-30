public sealed record UpdateBannerCommand(Guid UserId, Guid MediaId) : IRequest;
