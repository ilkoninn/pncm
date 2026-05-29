public sealed class GetStoreByIdQueryHandler(
    IStoreRepository storeRepository,
    IMediaGrpcClient mediaGrpcClient
) : IRequestHandler<GetStoreByIdQuery, StoreResponseDto>
{
    public async Task<StoreResponseDto> Handle(
        GetStoreByIdQuery request, CancellationToken cancellationToken)
    {
        var store = await storeRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Mağaza tapılmadı.");

        string? logoUrl = null;
        try
        {
            var logoMap = await mediaGrpcClient.GetPrimaryPhotosAsync([store.Id], ownerType: 1, cancellationToken);
            logoMap.TryGetValue(store.Id, out logoUrl);
        }
        catch { }

        return new StoreResponseDto(
            store.Id, store.Name, store.Address, store.City,
            store.Latitude, store.Longitude, store.Description,
            store.LogoMediaId, store.PhoneNumber, store.IsActive, store.CreatedAt,
            logoUrl);
    }
}
