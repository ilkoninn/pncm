public sealed class UpdatePetStatusCommandHandler(IPetRepository petRepository)
    : IRequestHandler<UpdatePetStatusCommand, PetResponseDto>
{
    public async Task<PetResponseDto> Handle(UpdatePetStatusCommand request, CancellationToken cancellationToken)
    {
        var pet = await petRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Heyvan tapılmadı.");

        pet.Status = request.Status;

        await petRepository.UpdateAsync(pet, cancellationToken);

        return pet.Adapt<PetResponseDto>();
    }
}
