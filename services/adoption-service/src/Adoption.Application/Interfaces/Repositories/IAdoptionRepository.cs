public interface IAdoptionRepository
{
    Task<AdoptionRequest?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<AdoptionRequest>> GetByPetIdAsync(Guid petId, CancellationToken ct = default);
    Task<IEnumerable<AdoptionRequest>> GetByAdopterIdAsync(Guid adopterId, CancellationToken ct = default);
    Task<AdoptionRequest> CreateAsync(AdoptionRequest request, CancellationToken ct = default);
    Task<AdoptionRequest> UpdateAsync(AdoptionRequest request, CancellationToken ct = default);
}
