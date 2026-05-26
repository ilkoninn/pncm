public sealed class UpdateAdoptionStatusCommandHandlerTests
{
    private readonly Mock<IAdoptionRepository> _repositoryMock = new();
    private readonly Mock<ITopicProducer<AdoptionApprovedEvent>> _approvedProducerMock = new();
    private readonly Mock<ITopicProducer<AdoptionRejectedEvent>> _rejectedProducerMock = new();

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

        _approvedProducerMock
            .Setup(p => p.Produce(It.IsAny<AdoptionApprovedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _rejectedProducerMock
            .Setup(p => p.Produce(It.IsAny<AdoptionRejectedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var handler = new UpdateAdoptionStatusCommandHandler(_repositoryMock.Object, _approvedProducerMock.Object, _rejectedProducerMock.Object);
        var result = await handler.Handle(new UpdateAdoptionStatusCommand(adoptionId, EAdoptionStatus.Approved), CancellationToken.None);

        result.Status.Should().Be(EAdoptionStatus.Approved);
    }

    [Fact]
    public async Task Handle_NonExistingRequest_ThrowsKeyNotFoundException()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AdoptionRequest?)null);

        _approvedProducerMock
            .Setup(p => p.Produce(It.IsAny<AdoptionApprovedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _rejectedProducerMock
            .Setup(p => p.Produce(It.IsAny<AdoptionRejectedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var handler = new UpdateAdoptionStatusCommandHandler(_repositoryMock.Object, _approvedProducerMock.Object, _rejectedProducerMock.Object);
        var act = async () => await handler.Handle(
            new UpdateAdoptionStatusCommand(Guid.NewGuid(), EAdoptionStatus.Approved), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Müraciət tapılmadı.");
    }
}
