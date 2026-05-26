public sealed class UpdateAdoptionStatusCommandHandler(
    IAdoptionRepository repository,
    ITopicProducer<AdoptionApprovedEvent> approvedProducer,
    ITopicProducer<AdoptionRejectedEvent> rejectedProducer)
    : IRequestHandler<UpdateAdoptionStatusCommand, AdoptionResponseDto>
{
    public async Task<AdoptionResponseDto> Handle(UpdateAdoptionStatusCommand request, CancellationToken cancellationToken)
    {
        var adoption = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Müraciət tapılmadı.");

        adoption.Status = request.Status;

        await repository.UpdateAsync(adoption, cancellationToken);

        if (adoption.Status == EAdoptionStatus.Approved)
            await approvedProducer.Produce(new AdoptionApprovedEvent
            {
                AdoptionId = adoption.Id,
                AdopterId = adoption.AdopterId
            }, cancellationToken);
        else if (adoption.Status == EAdoptionStatus.Rejected)
            await rejectedProducer.Produce(new AdoptionRejectedEvent
            {
                AdoptionId = adoption.Id,
                AdopterId = adoption.AdopterId
            }, cancellationToken);

        return adoption.Adapt<AdoptionResponseDto>();
    }
}
