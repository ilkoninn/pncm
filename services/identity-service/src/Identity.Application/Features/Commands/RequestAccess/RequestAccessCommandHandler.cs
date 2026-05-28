public sealed class RequestAccessCommandHandler(
    IUserRepository userRepository,
    IMagicLinkService magicLinkService
) : IRequestHandler<RequestAccessCommand, RequestAccessResponseDto>
{
    public async Task<RequestAccessResponseDto> Handle(
        RequestAccessCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);
        var isNewUser = user is null;

        if (request.Client == EClient.Web)
            await magicLinkService.GenerateMagicLinkTokenAsync(request.Email, cancellationToken);
        else
            await magicLinkService.GenerateOtpAsync(request.Email, cancellationToken);

        return new RequestAccessResponseDto(isNewUser);
    }
}
