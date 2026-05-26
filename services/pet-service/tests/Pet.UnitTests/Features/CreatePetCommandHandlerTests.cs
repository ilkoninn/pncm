public sealed class CreatePetCommandHandlerTests
{
    private readonly Mock<IPetRepository> _repositoryMock = new();

    public CreatePetCommandHandlerTests()
    {
        MappingConfig.Register(TypeAdapterConfig.GlobalSettings);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsPetResponseDto()
    {
        var command = new CreatePetCommand(
            "Buddy", ESpecies.Dog, null, null, EGender.Male, EPetSize.Medium,
            null, null, false, false, Guid.NewGuid(), EOwnerType.User, "Bakı", null, null);

        var petEntity = new Pet
        {
            Name = command.Name,
            Species = command.Species,
            Gender = command.Gender,
            Size = command.Size,
            IsVaccinated = command.IsVaccinated,
            IsNeutered = command.IsNeutered,
            OwnerId = command.OwnerId,
            OwnerType = command.OwnerType,
            City = command.City
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Pet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(petEntity);

        var handler = new CreatePetCommandHandler(_repositoryMock.Object);
        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
    }

    [Fact]
    public async Task Handle_ValidCommand_StatusIsAvailable()
    {
        var command = new CreatePetCommand(
            "Buddy", ESpecies.Dog, null, null, EGender.Male, EPetSize.Medium,
            null, null, false, false, Guid.NewGuid(), EOwnerType.User, "Bakı", null, null);

        var petEntity = new Pet
        {
            Name = command.Name,
            Species = command.Species,
            Gender = command.Gender,
            Size = command.Size,
            IsVaccinated = command.IsVaccinated,
            IsNeutered = command.IsNeutered,
            OwnerId = command.OwnerId,
            OwnerType = command.OwnerType,
            City = command.City
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Pet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(petEntity);

        var handler = new CreatePetCommandHandler(_repositoryMock.Object);
        var result = await handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be(EPetStatus.Available);
    }
}
