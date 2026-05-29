public sealed class UpdateStoreCommandHandler(
    IStoreRepository storeRepository
) : IRequestHandler<UpdateStoreCommand, StoreResponseDto>
{
    public async Task<StoreResponseDto> Handle(
        UpdateStoreCommand request, CancellationToken cancellationToken)
    {
        var store = await storeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (store is null)
            throw new KeyNotFoundException("Mağaza tapılmadı.");

        if (store.OwnerId != request.RequesterId)
            throw new UnauthorizedAccessException("Bu əməliyyat üçün icazəniz yoxdur.");

        store.Name = request.Name;
        store.Address = request.Address;
        store.City = request.City;
        store.Latitude = request.Latitude;
        store.Longitude = request.Longitude;
        store.Description = request.Description;
        store.LogoMediaId = request.LogoMediaId;
        store.PhoneNumber = request.PhoneNumber;
        store.IsActive = request.IsActive;

        var updated = await storeRepository.UpdateAsync(store, cancellationToken);

        return new StoreResponseDto(
            updated.Id,
            updated.Name,
            updated.Address,
            updated.City,
            updated.Latitude,
            updated.Longitude,
            updated.Description,
            updated.LogoMediaId,
            updated.PhoneNumber,
            updated.IsActive,
            updated.CreatedAt
        );
    }
}