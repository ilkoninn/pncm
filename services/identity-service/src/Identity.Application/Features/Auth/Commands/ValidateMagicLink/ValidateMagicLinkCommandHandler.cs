public sealed class ValidateMagicLinkCommandHandler(
    IMagicLinkService magicLinkService
) : IRequestHandler<ValidateMagicLinkCommand, string?>
{
    public async Task<string?> Handle(
        ValidateMagicLinkCommand request, CancellationToken cancellationToken)
    {
        var email = await magicLinkService.ValidateMagicTokenAsync(request.Token);
        if (email is null) return null;

        return await magicLinkService.GenerateMagicCodeAsync(email);
    }
}
