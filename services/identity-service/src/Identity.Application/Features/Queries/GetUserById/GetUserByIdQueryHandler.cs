public sealed class GetUserByIdQueryHandler(
    IUserRepository userRepository,
    IMediaGrpcClient mediaGrpcClient
) : IRequestHandler<GetUserByIdQuery, UserPublicResponseDto>
{
    public async Task<UserPublicResponseDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException("İstifadəçi tapılmadı.");

        string? avatarUrl = null;
        try { avatarUrl = await mediaGrpcClient.GetAvatarUrlAsync(user.Id, cancellationToken); }
        catch { }

        return new UserPublicResponseDto(user.Id, user.FirstName, user.LastName, avatarUrl, user.Bio, user.City);
    }
}
