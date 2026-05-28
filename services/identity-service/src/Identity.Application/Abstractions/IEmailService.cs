public interface IEmailService
{
    Task SendOtpAsync(string toEmail, string otpCode, CancellationToken ct);
    Task SendMagicLinkAsync(string toEmail, string magicLink, CancellationToken ct);
}
