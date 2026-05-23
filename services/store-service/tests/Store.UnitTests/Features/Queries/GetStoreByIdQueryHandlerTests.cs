public sealed class GetStoreByIdQueryHandlerTests
{
    private readonly Mock<IStoreRepository> _repositoryMock = new();

    [Fact]
    public async Task Handle_ExistingId_ReturnsStoreResponseDto()
    {
        var storeId = Guid.NewGuid();
        var store = new PetStore
        {
            Id = storeId,
            Name = "PetShop Bakı",
            Address = "Nizami küç. 10",
            City = "Bakı",
            Latitude = 40.4093m,
            Longitude = 49.8671m,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(storeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(store);

        var handler = new GetStoreByIdQueryHandler(_repositoryMock.Object);
        var result = await handler.Handle(new GetStoreByIdQuery(storeId), CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(storeId);
        result.Name.Should().Be(store.Name);
    }

    [Fact]
    public async Task Handle_NonExistingId_ThrowsKeyNotFoundException()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PetStore?)null);

        var handler = new GetStoreByIdQueryHandler(_repositoryMock.Object);

        await handler.Invoking(h => h.Handle(
            new GetStoreByIdQuery(Guid.NewGuid()), CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>();
    }
}