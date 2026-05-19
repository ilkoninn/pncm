public interface ITokenBlacklistService
{
    Task BlacklistAsync(string token, TimeSpan ttl);
    Task<bool> IsBlacklistedAsync(string token);
}