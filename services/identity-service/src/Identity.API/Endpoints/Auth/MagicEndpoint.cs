public class MagicEndpoint(IMediator mediator) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/auth/magic");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var token = Query<string>("token", isRequired: false);

        if (string.IsNullOrWhiteSpace(token))
        {
            HttpContext.Response.ContentType = "text/html; charset=utf-8";
            HttpContext.Response.StatusCode = 400;
            await HttpContext.Response.WriteAsync(BuildErrorPage(), ct);
            return;
        }

        var code = await mediator.Send(new ValidateMagicLinkCommand(token), ct);

        HttpContext.Response.ContentType = "text/html; charset=utf-8";

        if (code is null)
        {
            HttpContext.Response.StatusCode = 400;
            await HttpContext.Response.WriteAsync(BuildErrorPage(), ct);
            return;
        }

        await HttpContext.Response.WriteAsync(BuildCodePage(code), ct);
    }

    private static string BuildCodePage(string code) => $$"""
        <!DOCTYPE html>
        <html lang="az">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Pəncəm — Giriş Kodu</title>
            <style>
                * { margin: 0; padding: 0; box-sizing: border-box; }
                body {
                    min-height: 100vh;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    background: linear-gradient(135deg, #0f0c29, #302b63, #24243e);
                    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
                    color: #fff;
                }
                .card {
                    background: rgba(255,255,255,0.08);
                    backdrop-filter: blur(20px);
                    border: 1px solid rgba(255,255,255,0.15);
                    border-radius: 24px;
                    padding: 48px 56px;
                    text-align: center;
                    max-width: 420px;
                    width: 90%;
                    box-shadow: 0 32px 64px rgba(0,0,0,0.4);
                }
                .logo { font-size: 28px; font-weight: 800; letter-spacing: -1px; margin-bottom: 6px; }
                .logo span { color: #a78bfa; }
                .subtitle { color: rgba(255,255,255,0.45); font-size: 14px; margin-bottom: 40px; }
                .label {
                    font-size: 11px; color: rgba(255,255,255,0.45);
                    text-transform: uppercase; letter-spacing: 3px; margin-bottom: 14px;
                }
                .code {
                    font-size: 52px; font-weight: 800; letter-spacing: 10px;
                    color: #a78bfa; font-variant-numeric: tabular-nums; margin-bottom: 8px;
                }
                .expiry { font-size: 13px; color: rgba(255,255,255,0.35); margin-bottom: 28px; }
                .warning {
                    background: rgba(167,139,250,0.08);
                    border: 1px solid rgba(167,139,250,0.2);
                    border-radius: 12px;
                    padding: 12px 16px;
                    font-size: 13px;
                    color: rgba(255,255,255,0.5);
                }
            </style>
        </head>
        <body>
            <div class="card">
                <div class="logo">Pəncəm<span>.</span></div>
                <div class="subtitle">Tətbiqinizdə bu kodu daxil edin</div>
                <div class="label">Giriş Kodunuz</div>
                <div class="code">{{code}}</div>
                <div class="expiry">5 dəqiqə ərzində etibarlıdır</div>
                <div class="warning">⚠&nbsp; Kodu heç kimlə paylaşmayın</div>
            </div>
        </body>
        </html>
        """;

    private static string BuildErrorPage() => """
        <!DOCTYPE html>
        <html lang="az">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Pəncəm — Xəta</title>
            <style>
                * { margin: 0; padding: 0; box-sizing: border-box; }
                body {
                    min-height: 100vh;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    background: linear-gradient(135deg, #0f0c29, #302b63, #24243e);
                    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
                    color: #fff;
                }
                .card {
                    background: rgba(255,255,255,0.08);
                    backdrop-filter: blur(20px);
                    border: 1px solid rgba(255,255,255,0.15);
                    border-radius: 24px;
                    padding: 48px 56px;
                    text-align: center;
                    max-width: 420px;
                    width: 90%;
                    box-shadow: 0 32px 64px rgba(0,0,0,0.4);
                }
                .logo { font-size: 28px; font-weight: 800; letter-spacing: -1px; margin-bottom: 6px; }
                .logo span { color: #a78bfa; }
                .icon { font-size: 56px; margin: 28px 0 16px; }
                .title { font-size: 20px; font-weight: 700; margin-bottom: 8px; color: #f87171; }
                .subtitle { font-size: 14px; color: rgba(255,255,255,0.45); }
            </style>
        </head>
        <body>
            <div class="card">
                <div class="logo">Pəncəm<span>.</span></div>
                <div class="icon">✗</div>
                <div class="title">Link etibarsızdır</div>
                <div class="subtitle">Link müddəti bitib və ya artıq istifadə edilib.<br>Yeni giriş linki tələb edin.</div>
            </div>
        </body>
        </html>
        """;
}
