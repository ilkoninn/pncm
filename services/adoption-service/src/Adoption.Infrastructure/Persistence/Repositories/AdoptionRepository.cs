public sealed class AdoptionRepository(AdoptionDbContext context) : IAdoptionRepository
{
    public async Task<AdoptionRequest?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.AdoptionRequests.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IEnumerable<AdoptionRequest>> GetByPetIdAsync(Guid petId, Guid petOwnerId, CancellationToken ct = default)
        => await context.AdoptionRequests
            .Where(x => x.PetId == petId && x.PetOwnerId == petOwnerId && !x.IsDeleted)
            .ToListAsync(ct);

    public async Task<IEnumerable<AdoptionRequest>> GetByAdopterIdAsync(Guid adopterId, CancellationToken ct = default)
        => await context.AdoptionRequests.Where(x => x.AdopterId == adopterId && !x.IsDeleted).ToListAsync(ct);

    public async Task<AdoptionRequest> CreateAsync(AdoptionRequest request, CancellationToken ct = default)
    {
        context.AdoptionRequests.Add(request);
        await context.SaveChangesAsync(ct);
        return request;
    }

    public async Task<AdoptionRequest> UpdateAsync(AdoptionRequest request, CancellationToken ct = default)
    {
        context.AdoptionRequests.Update(request);
        await context.SaveChangesAsync(ct);
        return request;
    }
}
