public sealed class CreateAdoptionCommandHandler(
    IAdoptionRepository repository,
    ITopicProducer<AdoptionRequestedEvent> producer)
    : IRequestHandler<CreateAdoptionCommand, AdoptionResponseDto>
{
    public async Task<AdoptionResponseDto> Handle(CreateAdoptionCommand request, CancellationToken cancellationToken)
    {
        var adoption = new AdoptionRequest
        {
            PetId = request.PetId,
            AdopterId = request.AdopterId,
            Message = request.Message,
            ContactPhone = request.ContactPhone,
            PetName = request.PetName,
            PetSlug = request.PetSlug,
            PetPrimaryPhotoUrl = request.PetPrimaryPhotoUrl
        };

        await repository.CreateAsync(adoption, cancellationToken);

        await producer.Produce(new AdoptionRequestedEvent
        {
            AdoptionId = adoption.Id,
            PetId = adoption.PetId,
            AdopterId = adoption.AdopterId,
            OwnerId = Guid.Empty
        }, cancellationToken);

        return adoption.Adapt<AdoptionResponseDto>();
    }
}
