public interface IUserRepository
{
    Task<AppUser?> GetByIdAsync(Guid id);
    Task<AppUser?> GetByEmailAsync(string email);
    Task CreateAsync(AppUser user);
    Task UpdateAsync(AppUser user);
    Task DeleteAsync(Guid id);
}