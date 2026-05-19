public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad tələb olunur.")
            .MaximumLength(100).WithMessage("Ad 100 simvoldan çox ola bilməz.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad tələb olunur.")
            .MaximumLength(100).WithMessage("Soyad 100 simvoldan çox ola bilməz.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email tələb olunur.")
            .EmailAddress().WithMessage("Email formatı düzgün deyil.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon nömrəsi tələb olunur.")
            .Matches(@"^\+?[1-9]\d{9,14}$").WithMessage("Telefon nömrəsi düzgün deyil.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifrə tələb olunur.")
            .MinimumLength(8).WithMessage("Şifrə minimum 8 simvol olmalıdır.")
            .Matches("[A-Z]").WithMessage("Şifrədə ən az bir böyük hərf olmalıdır.")
            .Matches("[0-9]").WithMessage("Şifrədə ən az bir rəqəm olmalıdır.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Şifrələr uyğun gəlmir.");
    }
}