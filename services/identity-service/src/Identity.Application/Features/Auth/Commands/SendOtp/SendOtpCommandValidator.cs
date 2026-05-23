public class SendOtpCommandValidator : AbstractValidator<SendOtpCommand>
{
    public SendOtpCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("İstifadəçi IDsi boş ola bilməz.");
        RuleFor(x => x.Purpose).IsInEnum().WithMessage("Otp məqsədi düzgün deyil.");
    }
}