public class UploadMediaCommandHandlerTests
{
    private readonly Mock<IStorageService> _storageMock = new();
    private readonly Mock<IMediaRepository> _repositoryMock = new();
    private readonly UploadMediaCommandHandler _handler;

    public UploadMediaCommandHandlerTests()
    {
        _handler = new UploadMediaCommandHandler(_storageMock.Object, _repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsDto()
    {
        var ownerId = Guid.NewGuid();
        var command = new UploadMediaCommand(
            new MemoryStream([1, 2, 3]),
            "test.png",
            "image/png",
            3,
            ownerId,
            EOwnerType.User
        );

        var mediaFile = new MediaFile
        {
            Id = Guid.NewGuid(),
            FileName = "user/test.png",
            OriginalFileName = "test.png",
            ContentType = "image/png",
            Size = 3,
            Url = string.Empty,
            BucketName = "pncm-media",
            ObjectKey = "user/test.png",
            OwnerId = ownerId,
            OwnerType = EOwnerType.User
        };

        _storageMock
            .Setup(s => s.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("user/test.png");

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<MediaFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mediaFile);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.FileName.Should().Be("test.png");
        result.OwnerId.Should().Be(ownerId);
        result.OwnerType.Should().Be(EOwnerType.User);
    }

    [Fact]
    public async Task Handle_ValidCommand_ObjectKeyContainsLowercaseOwnerType()
    {
        var command = new UploadMediaCommand(
            new MemoryStream([1, 2, 3]),
            "test.png",
            "image/png",
            3,
            Guid.NewGuid(),
            EOwnerType.User
        );

        var mediaFile = new MediaFile
        {
            Id = Guid.NewGuid(),
            FileName = "user/test.png",
            OriginalFileName = "test.png",
            ContentType = "image/png",
            Size = 3,
            Url = string.Empty,
            BucketName = "pncm-media",
            ObjectKey = "user/test.png",
            OwnerId = Guid.NewGuid(),
            OwnerType = EOwnerType.User,
            CreatedAt = DateTime.UtcNow
        };

        _storageMock
            .Setup(s => s.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("user/test.png");

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<MediaFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mediaFile);

        string? capturedObjectKey = null;
        _storageMock
            .Setup(s => s.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, string, string, string, CancellationToken>((_, key, _, _, _) => capturedObjectKey = key)
            .ReturnsAsync("user/test.png");

        await _handler.Handle(command, CancellationToken.None);

        capturedObjectKey.Should().StartWith("user/");
    }
}