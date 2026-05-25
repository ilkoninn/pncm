public sealed class GetStoreByIdQueryHandler(
    IStoreRepository storeRepository
) : IRequestHandler<GetStoreByIdQuery, StoreResponseDto>
{
    public async Task<StoreResponseDto> Handle(
        GetStoreByIdQuery request, CancellationToken cancellationToken)
    {
        var store = await storeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (store is null)
            throw new KeyNotFoundException("Mağaza tapılmadı.");

        return new StoreResponseDto(
            store.Id,
            store.Name,
            store.Address,
            store.City,
            store.Latitude,
            store.Longitude,
            store.Description,
            store.LogoMediaId,
            store.PhoneNumber,
            store.IsActive,
            store.CreatedAt
        );
    }
}