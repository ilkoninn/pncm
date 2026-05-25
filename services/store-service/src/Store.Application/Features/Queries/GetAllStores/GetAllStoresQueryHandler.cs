public sealed class GetAllStoresQueryHandler(
    IStoreRepository storeRepository
) : IRequestHandler<GetAllStoresQuery, IEnumerable<StoreResponseDto>>
{
    public async Task<IEnumerable<StoreResponseDto>> Handle(
        GetAllStoresQuery request, CancellationToken cancellationToken)
    {
        var stores = await storeRepository.GetAllAsync(cancellationToken);

        return stores.Select(s => new StoreResponseDto(
            s.Id,
            s.Name,
            s.Address,
            s.City,
            s.Latitude,
            s.Longitude,
            s.Description,
            s.LogoMediaId,
            s.PhoneNumber,
            s.IsActive,
            s.CreatedAt
        ));
    }
}