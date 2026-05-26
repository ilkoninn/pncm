public sealed record CreatePetRequestDto(
    string Name,
    ESpecies Species,
    string? Breed,
    int? AgeMonths,
    EGender Gender,
    EPetSize Size,
    string? Color,
    string? Description,
    bool IsVaccinated,
    bool IsNeutered,
    Guid OwnerId,
    EOwnerType OwnerType,
    string City,
    decimal? Latitude,
    decimal? Longitude
);