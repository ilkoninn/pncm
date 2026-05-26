public sealed record UpdatePetCommand(
    Guid Id,
    string? Name,
    string? Breed,
    int? AgeMonths,
    string? Color,
    string? Description,
    bool? IsVaccinated,
    bool? IsNeutered,
    string? City,
    decimal? Latitude,
    decimal? Longitude
) : IRequest<PetResponseDto>;