public sealed class StoreRepository(StoreDbContext context) : IStoreRepository
{
    public async Task<PetStore?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Stores.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<PetStore>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Stores.Where(x => x.IsActive).ToListAsync(cancellationToken);

    public async Task<IEnumerable<PetStore>> GetNearbyAsync(
        decimal latitude, decimal longitude, double radiusKm, CancellationToken cancellationToken = default)
    {
        var stores = await context.Stores
            .Where(x => x.IsActive)
            .ToListAsync(cancellationToken);

        return stores.Where(s => CalculateDistance(
            (double)latitude, (double)longitude,
            (double)s.Latitude, (double)s.Longitude) <= radiusKm);
    }

    public async Task<PetStore> CreateAsync(PetStore store, CancellationToken cancellationToken = default)
    {
        store.Id = Guid.NewGuid();
        store.CreatedAt = DateTime.UtcNow;
        context.Stores.Add(store);
        await context.SaveChangesAsync(cancellationToken);
        return store;
    }

    public async Task<PetStore> UpdateAsync(PetStore store, CancellationToken cancellationToken = default)
    {
        store.UpdatedAt = DateTime.UtcNow;
        context.Stores.Update(store);
        await context.SaveChangesAsync(cancellationToken);
        return store;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var store = await context.Stores.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (store is null) return;
        context.Stores.Remove(store);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static double CalculateDistance(
        double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double ToRad(double deg) => deg * Math.PI / 180;
}