public sealed class UserRepository(
    IdentityDbContext context
) : IUserRepository
{
    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<AppUser?> GetByIdAsync(Guid id)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task CreateAsync(AppUser user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AppUser user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user is null) return;
    
        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }

    // For refresh token management
    public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        return await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        var refreshToken = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);
        
        if (refreshToken is null) return;
        
        refreshToken.IsRevoked = true;
        await context.SaveChangesAsync();
    }
}