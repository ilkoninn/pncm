public record CompleteRegisterCommand(
    string RegistrationToken,
    string FirstName,
    string LastName,
    string? PhoneNumber
) : IRequest<CompleteRegisterResponseDto>;
