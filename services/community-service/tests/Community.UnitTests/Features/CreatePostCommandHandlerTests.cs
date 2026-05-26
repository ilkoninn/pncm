public sealed class CreatePostCommandHandlerTests
{
    private readonly Mock<IPostRepository> _repositoryMock = new();

    public CreatePostCommandHandlerTests()
    {
        MappingConfig.Register(TypeAdapterConfig.GlobalSettings);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsPostResponseDto()
    {
        var command = new CreatePostCommand(Guid.NewGuid(), null, "Test məzmunu.", []);

        var postEntity = new Post
        {
            UserId = command.UserId,
            PetId = command.PetId,
            Content = command.Content,
            MediaIds = command.MediaIds
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(postEntity);

        var handler = new CreatePostCommandHandler(_repositoryMock.Object);
        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.UserId.Should().Be(command.UserId);
    }
}
