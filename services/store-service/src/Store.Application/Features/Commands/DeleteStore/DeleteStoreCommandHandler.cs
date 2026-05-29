public sealed class DeleteStoreCommandHandler(
    IStoreRepository storeRepository
) : IRequestHandler<DeleteStoreCommand, Unit>
{
    public async Task<Unit> Handle(
        DeleteStoreCommand request, CancellationToken cancellationToken)
    {
        var store = await storeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (store is null)
            throw new KeyNotFoundException("Mağaza tapılmadı.");

        if (store.OwnerId != request.RequesterId)
            throw new UnauthorizedAccessException("Bu əməliyyat üçün icazəniz yoxdur.");

        await storeRepository.DeleteAsync(request.Id, cancellationToken);

        return Unit.Value;
    }
}