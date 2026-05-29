public interface IPetGrpcClient
{
    Task<Guid> GetPetOwnerAsync(Guid petId, CancellationToken cancellationToken = default);
}
