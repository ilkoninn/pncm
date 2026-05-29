public interface IPetGrpcClient
{
    Task<Guid> GetPetOwnerAsync(Guid petId, CancellationToken cancellationToken = default);
    Task TransferOwnershipAsync(Guid petId, Guid newOwnerId, CancellationToken cancellationToken = default);
}
