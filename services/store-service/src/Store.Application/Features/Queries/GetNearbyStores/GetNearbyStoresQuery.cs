public sealed record GetNearbyStoresQuery(
    decimal Latitude,
    decimal Longitude,
    double RadiusKm = 10
) : IRequest<IEnumerable<StoreResponseDto>>;