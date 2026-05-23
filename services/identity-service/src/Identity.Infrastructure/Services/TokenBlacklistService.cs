public sealed class TokenBlacklistService(IConnectionMultiplexer redis) : ITokenBlacklistService
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task BlacklistAsync(string token, TimeSpan ttl)
    {
        await _db.StringSetAsync($"blacklist:{token}", "true", ttl);
    }

    public async Task<bool> IsBlacklistedAsync(string token)
    {
        return await _db.KeyExistsAsync($"blacklist:{token}");
    }
}
