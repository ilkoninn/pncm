public interface IPetRepository
{
    Task<Pet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Pet?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IEnumerable<Pet>> GetAllAsync(
        string? city = null,
        ESpecies? species = null,
        EGender? gender = null,
        EPetSize? size = null,
        bool? isVaccinated = null,
        bool? isNeutered = null,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Pet>> GetByOwnerAsync(Guid ownerId, EOwnerType ownerType, CancellationToken cancellationToken = default);
    Task<IEnumerable<Pet>> GetNearbyAsync(decimal latitude, decimal longitude, double radiusKm, CancellationToken cancellationToken = default);
    Task<Pet> CreateAsync(Pet pet, CancellationToken cancellationToken = default);
    Task<Pet> UpdateAsync(Pet pet, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddPhotoAsync(PetPhoto photo, CancellationToken cancellationToken = default);
    Task DeletePhotoAsync(Guid photoId, CancellationToken cancellationToken = default);
}