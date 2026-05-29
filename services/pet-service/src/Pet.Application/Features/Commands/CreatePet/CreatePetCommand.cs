public sealed record CreatePetCommand(
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
    string City,
    decimal? Latitude,
    decimal? Longitude,
    EPetStatus Status = EPetStatus.Available
) : IRequest<PetResponseDto>;