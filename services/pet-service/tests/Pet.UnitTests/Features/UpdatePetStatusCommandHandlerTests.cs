public sealed class UpdatePetStatusCommandHandlerTests
{
    private readonly Mock<IPetRepository> _repositoryMock = new();

    public UpdatePetStatusCommandHandlerTests()
    {
        MappingConfig.Register(TypeAdapterConfig.GlobalSettings);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesStatus()
    {
        var petId = Guid.NewGuid();

        var petEntity = new Pet
        {
            Name = "Buddy",
            Species = ESpecies.Dog,
            Gender = EGender.Male,
            Size = EPetSize.Medium,
            IsVaccinated = false,
            IsNeutered = false,
            OwnerId = Guid.NewGuid(),
            OwnerType = EOwnerType.User,
            City = "Bakı",
            Status = EPetStatus.Available
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(petId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(petEntity);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Pet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(petEntity);

        var handler = new UpdatePetStatusCommandHandler(_repositoryMock.Object);
        var result = await handler.Handle(new UpdatePetStatusCommand(petId, EPetStatus.Adopted), CancellationToken.None);

        result.Status.Should().Be(EPetStatus.Adopted);
    }

    [Fact]
    public async Task Handle_NonExistingPet_ThrowsKeyNotFoundException()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pet?)null);

        var handler = new UpdatePetStatusCommandHandler(_repositoryMock.Object);
        var act = async () => await handler.Handle(
            new UpdatePetStatusCommand(Guid.NewGuid(), EPetStatus.Adopted), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Heyvan tapılmadı.");
    }
}
