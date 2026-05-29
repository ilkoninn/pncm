public sealed class UpdateUserCommandHandler(
    IUserRepository userRepository
) : IRequestHandler<UpdateUserCommand, UserResponseDto>
{
    public async Task<UserResponseDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException("İstifadəçi tapılmadı.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.Bio = request.Bio;
        user.City = request.City;

        await userRepository.UpdateAsync(user);

        return new UserResponseDto(user.Id, user.FirstName, user.LastName, user.Email!, user.PhoneNumber, user.AvatarMediaId, Bio: user.Bio, City: user.City);
    }
}
