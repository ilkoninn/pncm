public sealed class GetNearbyPetsQueryHandler(IPetRepository petRepository)
    : IRequestHandler<GetNearbyPetsQuery, IEnumerable<PetResponseDto>>
{
    public async Task<IEnumerable<PetResponseDto>> Handle(GetNearbyPetsQuery request, CancellationToken cancellationToken)
    {
        var pets = await petRepository.GetNearbyAsync(request.Latitude, request.Longitude, request.RadiusKm, cancellationToken);

        return pets.Adapt<IEnumerable<PetResponseDto>>();
    }
}
