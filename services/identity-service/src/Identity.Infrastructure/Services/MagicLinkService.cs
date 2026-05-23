public sealed class MagicLinkService(IConnectionMultiplexer redis) : IMagicLinkService
{
    private static readonly TimeSpan MagicTokenTtl = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan MagicCodeTtl = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan OtpTtl = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan RegistrationTokenTtl = TimeSpan.FromMinutes(15);

    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<string> GenerateMagicLinkTokenAsync(string email)
    {
        var token = Guid.NewGuid().ToString("N");
        await _db.StringSetAsync($"magic:{token}", email, MagicTokenTtl);
        return token;
    }

    public async Task<string?> ValidateMagicTokenAsync(string token)
    {
        var key = $"magic:{token}";
        var email = await _db.StringGetAsync(key);
        if (email.IsNullOrEmpty) return null;
        await _db.KeyDeleteAsync(key);
        return email.ToString();
    }

    public async Task<string> GenerateMagicCodeAsync(string email)
    {
        var code = Random.Shared.Next(100000, 1000000).ToString();
        await _db.StringSetAsync($"magic-code:{email}", code, MagicCodeTtl);
        return code;
    }

    public async Task<bool> ValidateMagicCodeAsync(string email, string code)
    {
        var key = $"magic-code:{email}";
        var stored = await _db.StringGetAsync(key);
        if (stored.IsNullOrEmpty || stored != code) return false;
        await _db.KeyDeleteAsync(key);
        return true;
    }

    public async Task<string> GenerateOtpAsync(string email)
    {
        var code = Random.Shared.Next(100000, 1000000).ToString();
        await _db.StringSetAsync($"otp:{email}:access", code, OtpTtl);
        return code;
    }

    public async Task<bool> ValidateOtpAsync(string email, string code)
    {
        var key = $"otp:{email}:access";
        var stored = await _db.StringGetAsync(key);
        if (stored.IsNullOrEmpty || stored != code) return false;
        await _db.KeyDeleteAsync(key);
        return true;
    }

    public async Task<string> StoreRegistrationTokenAsync(string email)
    {
        var token = Guid.NewGuid().ToString("N");
        await _db.StringSetAsync($"reg:{token}", email, RegistrationTokenTtl);
        return token;
    }

    public async Task<string?> ValidateRegistrationTokenAsync(string token)
    {
        var key = $"reg:{token}";
        var email = await _db.StringGetAsync(key);
        if (email.IsNullOrEmpty) return null;
        await _db.KeyDeleteAsync(key);
        return email.ToString();
    }
}
