public class TokenBlacklistService(IConnectionMultiplexer redis) : ITokenBlacklistService
{
    public async Task BlacklistAsync(string token, TimeSpan ttl)
    {
        var db = redis.GetDatabase();
        await db.StringSetAsync($"blacklist:{token}", "true", ttl);
    }

    public async Task<bool> IsBlacklistedAsync(string token)
    {
        var db = redis.GetDatabase();
        return await db.KeyExistsAsync($"blacklist:{token}");
    }
}