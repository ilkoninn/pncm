using Adoption.Domain.Events;

public sealed class ConfirmAdoptionCommandHandler(
    IAdoptionRepository repository,
    IPetGrpcClient petGrpcClient,
    ITopicProducer<AdoptionCompletedEvent> completedProducer)
    : IRequestHandler<ConfirmAdoptionCommand, AdoptionResponseDto>
{
    public async Task<AdoptionResponseDto> Handle(ConfirmAdoptionCommand request, CancellationToken cancellationToken)
    {
        var adoption = await repository.GetByIdAsync(request.AdoptionId, cancellationToken)
            ?? throw new KeyNotFoundException("Müraciət tapılmadı.");

        if (adoption.AdopterId != request.RequesterId)
            throw new UnauthorizedAccessException("Bu əməliyyatı yalnız müraciət edən icra edə bilər.");

        if (adoption.Status != EAdoptionStatus.Approved)
            throw new InvalidOperationException("Yalnız təsdiqlənmiş müraciətlər tamamlana bilər.");

        await petGrpcClient.TransferOwnershipAsync(adoption.PetId, adoption.AdopterId, cancellationToken);

        adoption.Status = EAdoptionStatus.Completed;
        await repository.UpdateAsync(adoption, cancellationToken);

        await completedProducer.Produce(new AdoptionCompletedEvent
        {
            AdoptionId  = adoption.Id,
            PetId       = adoption.PetId,
            PetName     = adoption.PetName,
            PetSlug     = adoption.PetSlug,
            PetPrimaryPhotoUrl = adoption.PetPrimaryPhotoUrl,
            NewOwnerId  = adoption.AdopterId,
            NewOwnerName = adoption.AdopterName,
            OriginalOwnerId = adoption.PetOwnerId
        }, cancellationToken);

        return adoption.Adapt<AdoptionResponseDto>();
    }
}
