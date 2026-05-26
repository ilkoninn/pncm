public sealed class GetAdoptionsByAdopterQueryHandler(IAdoptionRepository repository)
    : IRequestHandler<GetAdoptionsByAdopterQuery, IEnumerable<AdoptionResponseDto>>
{
    public async Task<IEnumerable<AdoptionResponseDto>> Handle(GetAdoptionsByAdopterQuery request, CancellationToken cancellationToken)
    {
        var adoptions = await repository.GetByAdopterIdAsync(request.AdopterId, cancellationToken);

        return adoptions.Adapt<IEnumerable<AdoptionResponseDto>>();
    }
}
