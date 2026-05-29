public sealed record GetAllPetsQuery(
    string? City = null,
    ESpecies? Species = null,
    EGender? Gender = null,
    EPetSize? Size = null,
    bool? IsVaccinated = null,
    bool? IsNeutered = null,
    Guid? ExcludeOwnerId = null,
    Guid? OwnerId = null
) : IRequest<IEnumerable<PetResponseDto>>;
