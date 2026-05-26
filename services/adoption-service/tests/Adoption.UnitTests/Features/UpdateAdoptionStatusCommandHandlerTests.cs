public sealed class UpdateAdoptionStatusCommandHandlerTests
{
    private readonly Mock<IAdoptionRepository> _repositoryMock = new();

    public UpdateAdoptionStatusCommandHandlerTests()
    {
        MappingConfig.Register(TypeAdapterConfig.GlobalSettings);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesStatus()
    {
        var adoptionId = Guid.NewGuid();

        var adoptionEntity = new AdoptionRequest
        {
            PetId = Guid.NewGuid(),
            AdopterId = Guid.NewGuid(),
            Message = "Test mesajı",
            ContactPhone = "+994501234567",
            Status = EAdoptionStatus.Pending
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(adoptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(adoptionEntity);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<AdoptionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(adoptionEntity);

        var handler = new UpdateAdoptionStatusCommandHandler(_repositoryMock.Object);
        var result = await handler.Handle(new UpdateAdoptionStatusCommand(adoptionId, EAdoptionStatus.Approved), CancellationToken.None);

        result.Status.Should().Be(EAdoptionStatus.Approved);
    }

    [Fact]
    public async Task Handle_NonExistingRequest_ThrowsKeyNotFoundException()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AdoptionRequest?)null);

        var handler = new UpdateAdoptionStatusCommandHandler(_repositoryMock.Object);
        var act = async () => await handler.Handle(
            new UpdateAdoptionStatusCommand(Guid.NewGuid(), EAdoptionStatus.Approved), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Müraciət tapılmadı.");
    }
}
