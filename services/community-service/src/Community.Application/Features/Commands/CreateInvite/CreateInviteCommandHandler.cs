public sealed class CreateInviteCommandHandler(IInviteRepository repository)
    : IRequestHandler<CreateInviteCommand, InviteResponseDto>
{
    public async Task<InviteResponseDto> Handle(CreateInviteCommand request, CancellationToken cancellationToken)
    {
        var invite = new Invite
        {
            ContestId = request.ContestId,
            InviterId = request.InviterId
        };

        await repository.CreateAsync(invite, cancellationToken);

        return invite.Adapt<InviteResponseDto>();
    }
}
