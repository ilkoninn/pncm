public sealed class UserService(UserManager<AppUser> userManager) : IUserService
{
    public async Task<IdentityResult> CreateAsync(AppUser user) =>
        await userManager.CreateAsync(user);

    public async Task<IdentityResult> UpdateAsync(AppUser user) =>
        await userManager.UpdateAsync(user);

    public async Task<AppUser?> FindByEmailAsync(string email) =>
        await userManager.FindByEmailAsync(email);

    public async Task<bool> CheckPasswordAsync(AppUser user, string password) =>
        await userManager.CheckPasswordAsync(user, password);
}