public sealed class GetAllStoresQueryHandler(
    IStoreRepository storeRepository,
    IMediaGrpcClient mediaGrpcClient
) : IRequestHandler<GetAllStoresQuery, IEnumerable<StoreResponseDto>>
{
    public async Task<IEnumerable<StoreResponseDto>> Handle(
        GetAllStoresQuery request, CancellationToken cancellationToken)
    {
        var stores = await storeRepository.GetAllAsync(cancellationToken);
        var storeList = stores.ToList();

        if (storeList.Count == 0)
            return [];

        Dictionary<Guid, string> logoMap;
        try
        {
            var storeIds = storeList.Select(s => s.Id);
            logoMap = await mediaGrpcClient.GetPrimaryPhotosAsync(storeIds, ownerType: 1, cancellationToken);
        }
        catch { logoMap = []; }

        return storeList.Select(s => new StoreResponseDto(
            s.Id, s.Name, s.Address, s.City,
            s.Latitude, s.Longitude, s.Description,
            s.LogoMediaId, s.PhoneNumber, s.IsActive, s.CreatedAt,
            logoMap.TryGetValue(s.Id, out var url) ? url : null));
    }
}
