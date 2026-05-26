public sealed class UpdatePetCommandHandler(IPetRepository petRepository)
    : IRequestHandler<UpdatePetCommand, PetResponseDto>
{
    public async Task<PetResponseDto> Handle(UpdatePetCommand request, CancellationToken cancellationToken)
    {
        var pet = await petRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Heyvan tapılmadı.");

        pet.Name = request.Name ?? pet.Name;
        pet.Breed = request.Breed ?? pet.Breed;
        pet.AgeMonths = request.AgeMonths ?? pet.AgeMonths;
        pet.Color = request.Color ?? pet.Color;
        pet.Description = request.Description ?? pet.Description;
        pet.IsVaccinated = request.IsVaccinated ?? pet.IsVaccinated;
        pet.IsNeutered = request.IsNeutered ?? pet.IsNeutered;
        pet.City = request.City ?? pet.City;
        pet.Latitude = request.Latitude ?? pet.Latitude;
        pet.Longitude = request.Longitude ?? pet.Longitude;

        await petRepository.UpdateAsync(pet, cancellationToken);

        return pet.Adapt<PetResponseDto>();
    }
}
