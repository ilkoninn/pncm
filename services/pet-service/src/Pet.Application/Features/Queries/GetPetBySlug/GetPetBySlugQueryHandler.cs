public sealed class GetPetBySlugQueryHandler(IPetRepository petRepository)
    : IRequestHandler<GetPetBySlugQuery, PetResponseDto>
{
    public async Task<PetResponseDto> Handle(GetPetBySlugQuery request, CancellationToken cancellationToken)
    {
        var pet = await petRepository.GetBySlugAsync(request.Slug, cancellationToken);

        if (pet is null)
            throw new KeyNotFoundException("Heyvan tapılmadı.");

        return pet.Adapt<PetResponseDto>();
    }
}
