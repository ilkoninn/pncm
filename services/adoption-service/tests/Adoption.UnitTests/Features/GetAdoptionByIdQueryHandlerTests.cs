public sealed class GetAdoptionByIdQueryHandlerTests
{
    private readonly Mock<IAdoptionRepository> _repositoryMock = new();

    public GetAdoptionByIdQueryHandlerTests()
    {
        MappingConfig.Register(TypeAdapterConfig.GlobalSettings);
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsAdoptionResponseDto()
    {
        var adoptionEntity = new AdoptionRequest
        {
            PetId = Guid.NewGuid(),
            AdopterId = Guid.NewGuid(),
            Message = "Test mesajı",
            ContactPhone = "+994501234567"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(adoptionEntity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(adoptionEntity);

        var handler = new GetAdoptionByIdQueryHandler(_repositoryMock.Object);
        var result = await handler.Handle(new GetAdoptionByIdQuery(adoptionEntity.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(adoptionEntity.Id);
    }

    [Fact]
    public async Task Handle_NonExistingId_ThrowsKeyNotFoundException()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AdoptionRequest?)null);

        var handler = new GetAdoptionByIdQueryHandler(_repositoryMock.Object);
        var act = async () => await handler.Handle(
            new GetAdoptionByIdQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Müraciət tapılmadı.");
    }
}
