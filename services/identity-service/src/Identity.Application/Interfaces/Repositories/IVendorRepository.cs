public interface IVendorRepository
{
    Task<VendorProfile?> GetByIdAsync(Guid id);
    Task<VendorProfile?> GetByUserIdAsync(Guid userId);
    Task CreateAsync(VendorProfile vendor);
    Task UpdateAsync(VendorProfile vendor);
    Task DeleteAsync(Guid id);
}