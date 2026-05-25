public sealed class CreateStoreCommandHandlerTests
{
    private readonly Mock<IStoreRepository> _repositoryMock = new();
    private readonly Mock<IUserGrpcClient> _userGrpcClientMock = new();

    [Fact]
    public async Task Handle_ValidCommand_ReturnsStoreResponseDto()
    {
        var command = new CreateStoreCommand(
            "PetShop Bakı",
            "Nizami küç. 10",
            "Bakı",
            40.4093m,
            49.8671m,
            "Təsvir",
            null,
            "+994121234567",
            Guid.NewGuid()
        );

        var store = new PetStore
        {
            Name = command.Name,
            Address = command.Address,
            City = command.City,
            Latitude = command.Latitude,
            Longitude = command.Longitude,
            Description = command.Description,
            LogoMediaId = command.LogoMediaId,
            PhoneNumber = command.PhoneNumber,
            OwnerId = command.OwnerId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<PetStore>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(store);
            
        _userGrpcClientMock
            .Setup(x => x.UserExistsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        var handler = new CreateStoreCommandHandler(_repositoryMock.Object, _userGrpcClientMock.Object);
        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.City.Should().Be(command.City);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<PetStore>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsRepositoryOnce()
    {
        var command = new CreateStoreCommand(
            "Test Mağaza",
            "Test ünvan",
            "Bakı",
            40.0m,
            49.0m,
            null,
            null,
            null,
            Guid.NewGuid()
        );

        var store = new PetStore
        {
            Name = command.Name,
            Address = command.Address,
            City = command.City,
            Latitude = command.Latitude,
            Longitude = command.Longitude,
            OwnerId = command.OwnerId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<PetStore>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(store);

        _userGrpcClientMock
            .Setup(x => x.UserExistsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        var handler = new CreateStoreCommandHandler(_repositoryMock.Object, _userGrpcClientMock.Object);
        await handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(
            r => r.CreateAsync(It.IsAny<PetStore>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}