public sealed class DeleteStoreCommandHandlerTests
{
    private readonly Mock<IStoreRepository> _repositoryMock = new();

    [Fact]
    public async Task Handle_ExistingId_CallsDeleteOnce()
    {
        var storeId = Guid.NewGuid();
        var store = new PetStore
        {
            Id = storeId,
            Name = "Test",
            Address = "Test",
            City = "Bakı",
            Latitude = 40.0m,
            Longitude = 49.0m,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(storeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(store);

        _repositoryMock
            .Setup(r => r.DeleteAsync(storeId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new DeleteStoreCommandHandler(_repositoryMock.Object);
        await handler.Handle(new DeleteStoreCommand(storeId), CancellationToken.None);

        _repositoryMock.Verify(
            r => r.DeleteAsync(storeId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingId_ThrowsKeyNotFoundException()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PetStore?)null);

        var handler = new DeleteStoreCommandHandler(_repositoryMock.Object);

        await handler.Invoking(h => h.Handle(
            new DeleteStoreCommand(Guid.NewGuid()), CancellationToken.None))
            .Should().ThrowAsync<KeyNotFoundException>();
    }
}