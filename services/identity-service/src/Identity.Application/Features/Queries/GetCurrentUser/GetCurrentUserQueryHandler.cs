public sealed class GetCurrentUserQueryHandler(
    IUserRepository userRepository,
    IMediaGrpcClient mediaGrpcClient
) : IRequestHandler<GetCurrentUserQuery, UserResponseDto>
{
    public async Task<UserResponseDto> Handle(
        GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException("İstifadəçi tapılmadı.");

        string? avatarUrl = null;
        try { avatarUrl = await mediaGrpcClient.GetAvatarUrlAsync(user.Id, cancellationToken); }
        catch { }

        string? bannerUrl = null;
        if (user.BannerMediaId.HasValue)
            try { bannerUrl = await mediaGrpcClient.GetBannerUrlAsync(user.Id, user.BannerMediaId.Value, cancellationToken); }
            catch { }

        return new UserResponseDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email!,
            user.PhoneNumber,
            user.AvatarMediaId,
            avatarUrl,
            user.Bio,
            user.City,
            user.BannerMediaId,
            bannerUrl
        );
    }
}
