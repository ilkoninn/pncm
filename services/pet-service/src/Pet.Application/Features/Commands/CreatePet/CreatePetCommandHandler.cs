public sealed class CreatePetCommandHandler(IPetRepository petRepository)
    : IRequestHandler<CreatePetCommand, PetResponseDto>
{
    public async Task<PetResponseDto> Handle(CreatePetCommand request, CancellationToken cancellationToken)
    {
        var id = Guid.CreateVersion7();

        var pet = new Pet
        {
            Id = id,
            Name = request.Name,
            Slug = GenerateSlug(request.Name, id),
            Species = request.Species,
            Breed = request.Breed,
            AgeMonths = request.AgeMonths,
            Gender = request.Gender,
            Size = request.Size,
            Color = request.Color,
            Description = request.Description,
            IsVaccinated = request.IsVaccinated,
            IsNeutered = request.IsNeutered,
            Status = request.Status,
            OwnerId = request.OwnerId,
            OwnerType = EOwnerType.User,
            City = request.City,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        await petRepository.CreateAsync(pet, cancellationToken);

        return pet.Adapt<PetResponseDto>();
    }

    private static string GenerateSlug(string name, Guid id)
    {
        var slug = name.ToLowerInvariant()
            .Replace("ə", "e").Replace("ı", "i").Replace("ö", "o")
            .Replace("ü", "u").Replace("ç", "c").Replace("ğ", "g")
            .Replace("ş", "s").Replace(" ", "-");

        slug = new string(slug.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray()).Trim('-');

        return $"{slug}-{id.ToString("N")[..8]}";
    }
}
