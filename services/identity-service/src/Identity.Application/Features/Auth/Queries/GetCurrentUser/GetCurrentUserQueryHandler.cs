public sealed class GetCurrentUserQueryHandler(
    IUserRepository userRepository
) : IRequestHandler<GetCurrentUserQuery, UserResponseDto>
{
    public async Task<UserResponseDto> Handle(
        GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId);

        if (user is null)
            throw new KeyNotFoundException("İstifadəçi tapılmadı.");

        return new UserResponseDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email!,
            user.PhoneNumber
        );
    }
}