public sealed class CreateAdoptionCommandHandlerTests
{
    private readonly Mock<IAdoptionRepository> _repositoryMock = new();

    public CreateAdoptionCommandHandlerTests()
    {
        MappingConfig.Register(TypeAdapterConfig.GlobalSettings);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsAdoptionResponseDto()
    {
        var command = new CreateAdoptionCommand(
            Guid.NewGuid(), Guid.NewGuid(), "Evladlığa götürmək istəyirəm.", "+994501234567");

        var adoptionEntity = new AdoptionRequest
        {
            PetId = command.PetId,
            AdopterId = command.AdopterId,
            Message = command.Message,
            ContactPhone = command.ContactPhone
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<AdoptionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(adoptionEntity);

        var handler = new CreateAdoptionCommandHandler(_repositoryMock.Object);
        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.PetId.Should().Be(command.PetId);
    }

    [Fact]
    public async Task Handle_ValidCommand_StatusIsPending()
    {
        var command = new CreateAdoptionCommand(
            Guid.NewGuid(), Guid.NewGuid(), "Evladlığa götürmək istəyirəm.", "+994501234567");

        var adoptionEntity = new AdoptionRequest
        {
            PetId = command.PetId,
            AdopterId = command.AdopterId,
            Message = command.Message,
            ContactPhone = command.ContactPhone
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<AdoptionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(adoptionEntity);

        var handler = new CreateAdoptionCommandHandler(_repositoryMock.Object);
        var result = await handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be(EAdoptionStatus.Pending);
    }
}
