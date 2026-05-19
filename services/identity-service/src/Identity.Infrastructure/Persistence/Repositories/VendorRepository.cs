public class VendorRepository(
    AppDbContext context
) : IVendorRepository
{
    public async Task<VendorProfile?> GetByIdAsync(Guid id)
    {
        return await context.VendorProfiles
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<VendorProfile?> GetByUserIdAsync(Guid userId)
    {
        return await context.VendorProfiles
            .FirstOrDefaultAsync(v => v.UserId == userId);
    }

    public async Task CreateAsync(VendorProfile vendorProfile)
    {
        await context.VendorProfiles.AddAsync(vendorProfile);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(VendorProfile vendorProfile)
    {
        context.VendorProfiles.Update(vendorProfile);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var vendorProfile = await GetByIdAsync(id);
        if (vendorProfile is null) return;
    
        context.VendorProfiles.Remove(vendorProfile);
        await context.SaveChangesAsync();
    }
}