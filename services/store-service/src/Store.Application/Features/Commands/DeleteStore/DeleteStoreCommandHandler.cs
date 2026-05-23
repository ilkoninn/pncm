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

        await storeRepository.DeleteAsync(request.Id, cancellationToken);

        return Unit.Value;
    }
}