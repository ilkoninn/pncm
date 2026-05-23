public interface IMagicLinkService
{
    Task<string> GenerateMagicLinkTokenAsync(string email);
    Task<string?> ValidateMagicTokenAsync(string token);
    Task<string> GenerateMagicCodeAsync(string email);
    Task<bool> ValidateMagicCodeAsync(string email, string code);
    Task<string> GenerateOtpAsync(string email);
    Task<bool> ValidateOtpAsync(string email, string code);
    Task<string> StoreRegistrationTokenAsync(string email);
    Task<string?> ValidateRegistrationTokenAsync(string token);
}
