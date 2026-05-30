public sealed class CreateAdoptionCommandHandler(
    IAdoptionRepository repository,
    IPetGrpcClient petGrpcClient,
    ITopicProducer<AdoptionRequestedEvent> producer)
    : IRequestHandler<CreateAdoptionCommand, AdoptionResponseDto>
{
    public async Task<AdoptionResponseDto> Handle(CreateAdoptionCommand request, CancellationToken cancellationToken)
    {
        var petOwnerId = await petGrpcClient.GetPetOwnerAsync(request.PetId, cancellationToken);

        var adoption = new AdoptionRequest
        {
            PetId = request.PetId,
            AdopterId = request.AdopterId,
            PetOwnerId = petOwnerId,
            Message = request.Message,
            ContactPhone = request.ContactPhone,
            PetName = request.PetName,
            PetSlug = request.PetSlug,
            PetPrimaryPhotoUrl = request.PetPrimaryPhotoUrl,
            PetPrimaryPhotoMediaId = request.PetPrimaryPhotoMediaId,
            AdopterName = request.AdopterName
        };

        await repository.CreateAsync(adoption, cancellationToken);

        await producer.Produce(new AdoptionRequestedEvent
        {
            AdoptionId = adoption.Id,
            PetId = adoption.PetId,
            AdopterId = adoption.AdopterId,
            OwnerId = petOwnerId
        }, cancellationToken);

        return adoption.Adapt<AdoptionResponseDto>();
    }
}
