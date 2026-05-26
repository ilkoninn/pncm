public sealed class CreateAdoptionCommandHandler(IAdoptionRepository repository)
    : IRequestHandler<CreateAdoptionCommand, AdoptionResponseDto>
{
    public async Task<AdoptionResponseDto> Handle(CreateAdoptionCommand request, CancellationToken cancellationToken)
    {
        var adoption = new AdoptionRequest
        {
            PetId = request.PetId,
            AdopterId = request.AdopterId,
            Message = request.Message,
            ContactPhone = request.ContactPhone
        };

        await repository.CreateAsync(adoption, cancellationToken);

        return adoption.Adapt<AdoptionResponseDto>();
    }
}
