public sealed class GetAdoptionByIdQueryHandler(IAdoptionRepository repository)
    : IRequestHandler<GetAdoptionByIdQuery, AdoptionResponseDto>
{
    public async Task<AdoptionResponseDto> Handle(GetAdoptionByIdQuery request, CancellationToken cancellationToken)
    {
        var adoption = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Müraciət tapılmadı.");

        return adoption.Adapt<AdoptionResponseDto>();
    }
}
