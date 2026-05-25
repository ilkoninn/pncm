public class GetMediaByIdQueryHandlerTests
{
    private readonly Mock<IMediaRepository> _repositoryMock = new();
    private readonly Mock<IStorageService> _storageMock = new();
    private readonly GetMediaByIdQueryHandler _handler;

    public GetMediaByIdQueryHandlerTests()
    {
        _handler = new GetMediaByIdQueryHandler(_repositoryMock.Object, _storageMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsPresignedUrl()
    {
        var id = Guid.NewGuid();
        var mediaFile = new MediaFile
        {
            FileName = "user/test.png",
            OriginalFileName = "test.png",
            ContentType = "image/png",
            Size = 3,
            Url = string.Empty,
            BucketName = "pncm-media",
            ObjectKey = "user/test.png",
            OwnerId = Guid.NewGuid(),
            OwnerType = EOwnerType.User
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mediaFile);

        _storageMock
            .Setup(s => s.GetPresignedUrlAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("http://localhost:9000/pncm-media/user/test.png?X-Amz-Signature=abc");

        var result = await _handler.Handle(new GetMediaByIdQuery(id), CancellationToken.None);

        result.Should().NotBeNull();
        result.Url.Should().Contain("X-Amz-Signature");
    }

    [Fact]
    public async Task Handle_NonExistingId_ThrowsKeyNotFoundException()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MediaFile?)null);

        var act = async () => await _handler.Handle(new GetMediaByIdQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Media faylı tapılmadı.");
    }
}