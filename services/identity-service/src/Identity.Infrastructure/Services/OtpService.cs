public sealed class OtpService(IConnectionMultiplexer redis) : IOtpService
{
    private static readonly TimeSpan OtpTtl = TimeSpan.FromMinutes(3);
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<string> GenerateOtpAsync(Guid userId, EOtpPurpose purpose)
    {
        var code = Random.Shared.Next(100000, 999999).ToString();
        var key = $"otp:{userId}:{purpose}";
        
        await _db.StringSetAsync(key, code, OtpTtl);
        
        return code;
    }

    public async Task<bool> ValidateOtpAsync(Guid userId, string code, EOtpPurpose purpose)
    {
        var key = $"otp:{userId}:{purpose}";
        var storedCode = await _db.StringGetAsync(key);
        
        if (storedCode.IsNullOrEmpty || storedCode != code)
            return false;

        await _db.KeyDeleteAsync(key);
        return true;
    }
}