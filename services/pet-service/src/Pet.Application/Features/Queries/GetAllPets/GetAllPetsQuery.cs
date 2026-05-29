public sealed record GetAllPetsQuery(
    string? City = null,
    ESpecies? Species = null,
    EGender? Gender = null,
    EPetSize? Size = null,
    bool? IsVaccinated = null,
    bool? IsNeutered = null
) : IRequest<IEnumerable<PetResponseDto>>;
