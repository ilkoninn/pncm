public sealed class GetAllPetsQueryHandler(IPetRepository petRepository)
    : IRequestHandler<GetAllPetsQuery, IEnumerable<PetResponseDto>>
{
    public async Task<IEnumerable<PetResponseDto>> Handle(GetAllPetsQuery request, CancellationToken cancellationToken)
    {
        var pets = await petRepository.GetAllAsync(
            request.City, request.Species, request.Gender,
            request.Size, request.IsVaccinated, request.IsNeutered,
            cancellationToken);

        return pets.Adapt<IEnumerable<PetResponseDto>>();
    }
}
