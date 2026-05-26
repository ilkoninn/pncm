public sealed record GetNearbyPetsQuery(
    decimal Latitude,
    decimal Longitude,
    double RadiusKm
) : IRequest<IEnumerable<PetResponseDto>>;
