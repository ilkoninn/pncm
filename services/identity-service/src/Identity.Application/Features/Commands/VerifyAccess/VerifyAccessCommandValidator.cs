public sealed class VerifyAccessCommandValidator : AbstractValidator<VerifyAccessCommand>
{
    public VerifyAccessCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email tələb olunur.")
            .EmailAddress().WithMessage("Email formatı yanlışdır.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Kod tələb olunur.")
            .Length(6).WithMessage("Kod 6 rəqəmdən ibarət olmalıdır.")
            .Matches(@"^\d{6}$").WithMessage("Kod yalnız rəqəmlərdən ibarət olmalıdır.");

        RuleFor(x => x.Client)
            .IsInEnum().WithMessage("Client 'Web' və ya 'Mobile' olmalıdır.");
    }
}
