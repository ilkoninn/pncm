public sealed class UpdateBannerCommandHandler(
    IUserRepository userRepository
) : IRequestHandler<UpdateBannerCommand>
{
    public async Task Handle(UpdateBannerCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException("İstifadəçi tapılmadı.");

        user.BannerMediaId = request.MediaId;
        await userRepository.UpdateAsync(user);
    }
}
