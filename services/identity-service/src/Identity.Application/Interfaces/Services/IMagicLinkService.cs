public interface IMagicLinkService
{
    Task<string> GenerateMagicLinkTokenAsync(string email, CancellationToken ct = default);
    Task<string?> ValidateMagicTokenAsync(string token);
    Task<string> GenerateMagicCodeAsync(string email);
    Task<bool> ValidateMagicCodeAsync(string email, string code);
    Task<string> GenerateOtpAsync(string email, CancellationToken ct = default);
    Task<bool> ValidateOtpAsync(string email, string code);
    Task<string> StoreRegistrationTokenAsync(string email);
    Task<string?> ValidateRegistrationTokenAsync(string token);
}
