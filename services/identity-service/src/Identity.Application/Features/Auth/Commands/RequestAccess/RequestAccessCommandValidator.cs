public class RequestAccessCommandValidator : AbstractValidator<RequestAccessCommand>
{
    public RequestAccessCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email tələb olunur.")
            .EmailAddress().WithMessage("Email formatı yanlışdır.");

        RuleFor(x => x.Client)
            .IsInEnum().WithMessage("Client 'Web' və ya 'Mobile' olmalıdır.");
    }
}
