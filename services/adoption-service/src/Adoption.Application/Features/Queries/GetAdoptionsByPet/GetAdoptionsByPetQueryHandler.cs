public sealed class GetAdoptionsByPetQueryHandler(IAdoptionRepository repository)
    : IRequestHandler<GetAdoptionsByPetQuery, IEnumerable<AdoptionResponseDto>>
{
    public async Task<IEnumerable<AdoptionResponseDto>> Handle(GetAdoptionsByPetQuery request, CancellationToken cancellationToken)
    {
        var adoptions = await repository.GetByPetIdAsync(request.PetId, cancellationToken);

        return adoptions.Adapt<IEnumerable<AdoptionResponseDto>>();
    }
}
