public sealed class GetNearbyStoresQueryHandler(
    IStoreRepository storeRepository
) : IRequestHandler<GetNearbyStoresQuery, IEnumerable<StoreResponseDto>>
{
    public async Task<IEnumerable<StoreResponseDto>> Handle(
        GetNearbyStoresQuery request, CancellationToken cancellationToken)
    {
        var stores = await storeRepository.GetNearbyAsync(
            request.Latitude,
            request.Longitude,
            request.RadiusKm,
            cancellationToken);

        return stores.Select(s => new StoreResponseDto(
            s.Id,
            s.Name,
            s.Address,
            s.City,
            s.Latitude,
            s.Longitude,
            s.Description,
            s.LogoUrl,
            s.PhoneNumber,
            s.IsActive,
            s.CreatedAt
        ));
    }
}