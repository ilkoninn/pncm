public sealed class GiveScoreCommandHandlerTests
{
    private readonly Mock<IContestEntryRepository> _repositoryMock = new();
    private readonly Mock<ITopicProducer<ScoreGivenEvent>> _producerMock = new();

    public GiveScoreCommandHandlerTests()
    {
        MappingConfig.Register(TypeAdapterConfig.GlobalSettings);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsContestEntryResponseDto()
    {
        var entryId = Guid.NewGuid();
        var givenByUserId = Guid.NewGuid();

        var entryDto = new ContestEntryResponseDto(
            entryId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1, DateTime.UtcNow);

        _repositoryMock
            .Setup(r => r.AddScoreAsync(entryId, givenByUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entryDto);

        _producerMock
            .Setup(p => p.Produce(It.IsAny<ScoreGivenEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new GiveScoreCommandHandler(_repositoryMock.Object, _producerMock.Object);
        var result = await handler.Handle(new GiveScoreCommand(entryId, givenByUserId), CancellationToken.None);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_InvalidEntryId_ThrowsKeyNotFoundException()
    {
        _repositoryMock
            .Setup(r => r.AddScoreAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Yarışma iştirakçısı tapılmadı."));

        var handler = new GiveScoreCommandHandler(_repositoryMock.Object, _producerMock.Object);
        var act = async () => await handler.Handle(
            new GiveScoreCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Yarışma iştirakçısı tapılmadı.");
    }
}
