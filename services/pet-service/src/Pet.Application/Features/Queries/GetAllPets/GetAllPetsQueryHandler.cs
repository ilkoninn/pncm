public sealed class GetAllPetsQueryHandler(IPetRepository petRepository)
    : IRequestHandler<GetAllPetsQuery, IEnumerable<PetResponseDto>>
{
    public async Task<IEnumerable<PetResponseDto>> Handle(GetAllPetsQuery request, CancellationToken cancellationToken)
    {
        var pets = await petRepository.GetAllAsync(cancellationToken);

        return pets.Adapt<IEnumerable<PetResponseDto>>();
    }
}
