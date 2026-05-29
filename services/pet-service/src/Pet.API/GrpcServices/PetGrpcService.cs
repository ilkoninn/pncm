using Grpc.Core;
using Pet.API.Protos;

namespace Pet.API.GrpcServices;

public sealed class PetGrpcService(IPetRepository petRepository) : Protos.PetGrpcService.PetGrpcServiceBase
{
    public override async Task<GetPetOwnerResponse> GetPetOwner(
        GetPetOwnerRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.PetId, out var petId))
            throw new RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.InvalidArgument, "Yanlış pet ID formatı."));

        var pet = await petRepository.GetByIdAsync(petId, context.CancellationToken)
            ?? throw new RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.NotFound, "Heyvan tapılmadı."));

        return new GetPetOwnerResponse { OwnerId = pet.OwnerId.ToString() };
    }
}
