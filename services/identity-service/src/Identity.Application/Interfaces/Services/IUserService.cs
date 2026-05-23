public interface IUserService
{
    Task<IdentityResult> CreateAsync(AppUser user);
    Task<IdentityResult> UpdateAsync(AppUser user);
    Task<AppUser?> FindByEmailAsync(string email);
    Task<bool> CheckPasswordAsync(AppUser user, string password);
}