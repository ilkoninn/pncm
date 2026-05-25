public sealed class UpdateStoreLogoCommandHandler(
    IStoreRepository storeRepository
) : IRequestHandler<UpdateStoreLogoCommand>
{
    public async Task Handle(UpdateStoreLogoCommand request, CancellationToken cancellationToken)
    {
        var store = await storeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (store is null)
            throw new KeyNotFoundException("Mağaza tapılmadı.");

        store.LogoMediaId = request.MediaId;
        await storeRepository.UpdateAsync(store, cancellationToken);
    }
}