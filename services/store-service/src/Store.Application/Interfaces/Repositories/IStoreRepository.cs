public interface IStoreRepository
{
    Task<PetStore?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PetStore>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PetStore>> GetNearbyAsync(decimal latitude, decimal longitude, double radiusKm, CancellationToken cancellationToken = default);
    Task<PetStore> CreateAsync(PetStore store, CancellationToken cancellationToken = default);
    Task<PetStore> UpdateAsync(PetStore store, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}