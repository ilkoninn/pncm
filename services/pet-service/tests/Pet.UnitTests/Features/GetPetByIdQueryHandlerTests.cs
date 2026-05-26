public sealed class GetPetByIdQueryHandlerTests
{
    private readonly Mock<IPetRepository> _repositoryMock = new();

    public GetPetByIdQueryHandlerTests()
    {
        MappingConfig.Register(TypeAdapterConfig.GlobalSettings);
    }

    [Fact]
    public async Task Handle_ExistingId_ReturnsPetResponseDto()
    {
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
            City = "Bakı"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(petEntity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(petEntity);

        var handler = new GetPetByIdQueryHandler(_repositoryMock.Object);
        var result = await handler.Handle(new GetPetByIdQuery(petEntity.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(petEntity.Id);
    }

    [Fact]
    public async Task Handle_NonExistingId_ThrowsKeyNotFoundException()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pet?)null);

        var handler = new GetPetByIdQueryHandler(_repositoryMock.Object);
        var act = async () => await handler.Handle(
            new GetPetByIdQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Heyvan tapılmadı.");
    }
}
