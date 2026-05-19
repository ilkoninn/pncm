public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token tələb olunur.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token tələb olunur.");
    }
}