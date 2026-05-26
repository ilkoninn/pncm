public sealed class GetPetByIdQueryHandler(IPetRepository petRepository)
    : IRequestHandler<GetPetByIdQuery, PetResponseDto>
{
    public async Task<PetResponseDto> Handle(GetPetByIdQuery request, CancellationToken cancellationToken)
    {
        var pet = await petRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Heyvan tapılmadı.");

        return pet.Adapt<PetResponseDto>();
    }
}
