public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("İstifadəçi ID tələb olunur.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("OTP kodu tələb olunur.")
            .Length(6).WithMessage("OTP kodu 6 rəqəm olmalıdır.");

        RuleFor(x => x.Purpose)
            .IsInEnum().WithMessage("OTP məqsədi düzgün deyil.");
    }
}