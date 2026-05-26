public sealed class UpdateAdoptionStatusCommandHandler(IAdoptionRepository repository)
    : IRequestHandler<UpdateAdoptionStatusCommand, AdoptionResponseDto>
{
    public async Task<AdoptionResponseDto> Handle(UpdateAdoptionStatusCommand request, CancellationToken cancellationToken)
    {
        var adoption = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Müraciət tapılmadı.");

        adoption.Status = request.Status;

        await repository.UpdateAsync(adoption, cancellationToken);

        return adoption.Adapt<AdoptionResponseDto>();
    }
}
