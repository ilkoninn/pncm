using Grpc.Net.Client;
using Pet.API.Protos;

public sealed class PetGrpcClient(IConfiguration configuration) : IPetGrpcClient
{
    public async Task<Guid> GetPetOwnerAsync(Guid petId, CancellationToken cancellationToken = default)
    {
        var address = configuration["GrpcServices:PetService"]!;
        var channel = GrpcChannel.ForAddress(address);
        var client = new PetGrpcService.PetGrpcServiceClient(channel);

        var response = await client.GetPetOwnerAsync(
            new GetPetOwnerRequest { PetId = petId.ToString() },
            cancellationToken: cancellationToken);

        return Guid.Parse(response.OwnerId);
    }

    public async Task TransferOwnershipAsync(Guid petId, Guid newOwnerId, CancellationToken cancellationToken = default)
    {
        var address = configuration["GrpcServices:PetService"]!;
        var channel = GrpcChannel.ForAddress(address);
        var client = new PetGrpcService.PetGrpcServiceClient(channel);

        await client.TransferPetOwnershipAsync(
            new TransferPetOwnershipRequest { PetId = petId.ToString(), NewOwnerId = newOwnerId.ToString() },
            cancellationToken: cancellationToken);
    }
}
