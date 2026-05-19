public interface IOtpService
{
    Task<string> GenerateOtpAsync(Guid userId, EOtpPurpose purpose);
    Task<bool> ValidateOtpAsync(Guid userId, string code, EOtpPurpose purpose);
}