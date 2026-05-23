public class CompleteRegisterCommandValidator : AbstractValidator<CompleteRegisterCommand>
{
    public CompleteRegisterCommandValidator()
    {
        RuleFor(x => x.RegistrationToken)
            .NotEmpty().WithMessage("Qeydiyyat tokeni tələb olunur.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad tələb olunur.")
            .MaximumLength(100).WithMessage("Ad 100 simvoldan çox ola bilməz.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad tələb olunur.")
            .MaximumLength(100).WithMessage("Soyad 100 simvoldan çox ola bilməz.");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{9,14}$").WithMessage("Telefon nömrəsi düzgün formatda deyil.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}
