public sealed class CreateStoreCommandHandler(
    IStoreRepository storeRepository,
    IUserGrpcClient userGrpcClient
) : IRequestHandler<CreateStoreCommand, StoreResponseDto>
{
    public async Task<StoreResponseDto> Handle(
        CreateStoreCommand request, CancellationToken cancellationToken)
    {
        var userExists = await userGrpcClient.UserExistsAsync(request.OwnerId);

        if (!userExists)
            throw new KeyNotFoundException("İstifadəçi tapılmadı.");

        var store = new PetStore
        {
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Description = request.Description,
            LogoUrl = request.LogoUrl,
            PhoneNumber = request.PhoneNumber,
            OwnerId = request.OwnerId
        };

        var created = await storeRepository.CreateAsync(store, cancellationToken);

        return new StoreResponseDto(
            created.Id,
            created.Name,
            created.Address,
            created.City,
            created.Latitude,
            created.Longitude,
            created.Description,
            created.LogoUrl,
            created.PhoneNumber,
            created.IsActive,
            created.CreatedAt
        );
    }
}