public sealed class SmtpEmailService(IConfiguration configuration) : IEmailService
{
    private readonly string _host = configuration["Email:Smtp:Host"]!;
    private readonly int _port = int.Parse(configuration["Email:Smtp:Port"]!);
    private readonly string _username = configuration["Email:Smtp:Username"]!;
    private readonly string _password = configuration["Email:Smtp:Password"]!;
    private readonly string _fromAddress = configuration["Email:Smtp:FromAddress"]!;
    private readonly string _fromName = configuration["Email:Smtp:FromName"]!;

    public async Task SendOtpAsync(string toEmail, string otpCode, CancellationToken ct)
    {
        var message = BuildMessage(toEmail, "Pəncəm — Giriş Kodu", $"""
            <div style="font-family:sans-serif;max-width:480px;margin:auto;padding:32px;border:1px solid #e5e7eb;border-radius:12px;">
              <h2 style="color:#1e293b;margin-bottom:8px;">Giriş Kodu</h2>
              <p style="color:#475569;margin-bottom:24px;">Aşağıdakı kodu istifadə edərək hesabınıza daxil olun. Kod <strong>10 dəqiqə</strong> ərzində etibarlıdır.</p>
              <div style="background:#f1f5f9;border-radius:8px;padding:24px;text-align:center;letter-spacing:8px;font-size:32px;font-weight:700;color:#0f172a;">
                {otpCode}
              </div>
              <p style="color:#94a3b8;font-size:12px;margin-top:24px;">Bu kodu heç kimlə paylaşmayın.</p>
            </div>
            """);

        await SendAsync(message, ct);
    }

    public async Task SendMagicLinkAsync(string toEmail, string magicLink, CancellationToken ct)
    {
        var message = BuildMessage(toEmail, "Pəncəm — Magic Link", $"""
            <div style="font-family:sans-serif;max-width:480px;margin:auto;padding:32px;border:1px solid #e5e7eb;border-radius:12px;">
              <h2 style="color:#1e293b;margin-bottom:8px;">Giriş Linki</h2>
              <p style="color:#475569;margin-bottom:24px;">Hesabınıza daxil olmaq üçün aşağıdakı düyməyə klikləyin. Link <strong>10 dəqiqə</strong> ərzində etibarlıdır.</p>
              <a href="{magicLink}" style="display:inline-block;background:#0f172a;color:#fff;text-decoration:none;padding:12px 28px;border-radius:8px;font-weight:600;">
                Daxil ol
              </a>
              <p style="color:#94a3b8;font-size:12px;margin-top:24px;">Əgər siz bu tələbi göndərməmisinizsə, bu emaili nəzərə almayın.</p>
            </div>
            """);

        await SendAsync(message, ct);
    }

    private MimeMessage BuildMessage(string toEmail, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_fromName, _fromAddress));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };
        return message;
    }

    private async Task SendAsync(MimeMessage message, CancellationToken ct)
    {
        using var client = new SmtpClient();
        await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls, ct);
        await client.AuthenticateAsync(_username, _password, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
    }
}
