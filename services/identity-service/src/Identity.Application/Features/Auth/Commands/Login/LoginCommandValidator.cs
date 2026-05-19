public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email tələb olunur.")
            .EmailAddress().WithMessage("Email formatı düzgün deyil.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifrə tələb olunur.");
    }
}