public sealed class CreateContestCommandHandlerTests
{
    private readonly Mock<IContestRepository> _repositoryMock = new();

    public CreateContestCommandHandlerTests()
    {
        MappingConfig.Register(TypeAdapterConfig.GlobalSettings);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsContestResponseDto()
    {
        var command = new CreateContestCommand(
            "Test Yarışması", "Açıqlama", DateTime.UtcNow, DateTime.UtcNow.AddDays(7), null);

        var contestEntity = new Contest
        {
            Title = command.Title,
            Description = command.Description,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Prize = command.Prize
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Contest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contestEntity);

        var handler = new CreateContestCommandHandler(_repositoryMock.Object);
        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Title.Should().Be(command.Title);
    }

    [Fact]
    public async Task Handle_ValidCommand_StatusIsDraft()
    {
        var command = new CreateContestCommand(
            "Test Yarışması", "Açıqlama", DateTime.UtcNow, DateTime.UtcNow.AddDays(7), null);

        var contestEntity = new Contest
        {
            Title = command.Title,
            Description = command.Description,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Prize = command.Prize
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Contest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contestEntity);

        var handler = new CreateContestCommandHandler(_repositoryMock.Object);
        var result = await handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be(EContestStatus.Draft);
    }
}
