public sealed class UpdateAvatarCommandHandler(
    IUserRepository userRepository
) : IRequestHandler<UpdateAvatarCommand>
{
    public async Task Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId);

        if (user is null)
            throw new KeyNotFoundException("İstifadəçi tapılmadı.");

        user.AvatarMediaId = request.MediaId;
        await userRepository.UpdateAsync(user);
    }
}