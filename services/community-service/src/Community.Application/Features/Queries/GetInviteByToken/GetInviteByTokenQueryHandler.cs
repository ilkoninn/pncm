public sealed class GetInviteByTokenQueryHandler(IInviteRepository repository)
    : IRequestHandler<GetInviteByTokenQuery, InviteResponseDto>
{
    public async Task<InviteResponseDto> Handle(GetInviteByTokenQuery request, CancellationToken cancellationToken)
    {
        var invite = await repository.GetByTokenAsync(request.Token, cancellationToken)
            ?? throw new KeyNotFoundException("Dəvətnamə tapılmadı.");

        return invite.Adapt<InviteResponseDto>();
    }
}
