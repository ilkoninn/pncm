public sealed class CreatePetCommandHandler(IPetRepository petRepository)
    : IRequestHandler<CreatePetCommand, PetResponseDto>
{
    public async Task<PetResponseDto> Handle(CreatePetCommand request, CancellationToken cancellationToken)
    {
        var pet = new Pet
        {
            Name = request.Name,
            Species = request.Species,
            Breed = request.Breed,
            AgeMonths = request.AgeMonths,
            Gender = request.Gender,
            Size = request.Size,
            Color = request.Color,
            Description = request.Description,
            IsVaccinated = request.IsVaccinated,
            IsNeutered = request.IsNeutered,
            OwnerId = request.OwnerId,
            OwnerType = request.OwnerType,
            City = request.City,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        await petRepository.CreateAsync(pet, cancellationToken);

        return pet.Adapt<PetResponseDto>();
    }
}
