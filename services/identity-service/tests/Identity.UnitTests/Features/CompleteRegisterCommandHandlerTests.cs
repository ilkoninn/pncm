public sealed class CompleteRegisterCommandHandlerTests
{
    private readonly Mock<IMagicLinkService> _magicLinkServiceMock = new();
    private readonly Mock<IUserService> _userManagerMock;
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<ITopicProducer<UserRegisteredEvent>> _producerMock = new();
    private readonly CompleteRegisterCommandHandler _handler;

    public CompleteRegisterCommandHandlerTests()
    {
        _userManagerMock = new Mock<IUserService>();

        _producerMock
            .Setup(p => p.Produce(It.IsAny<UserRegisteredEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _handler = new CompleteRegisterCommandHandler(
            _magicLinkServiceMock.Object,
            _userManagerMock.Object,
            _userRepositoryMock.Object,
            _tokenServiceMock.Object,
            _producerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenTokenIsInvalid_ShouldThrowValidationException()
    {
        // Arrange
        _magicLinkServiceMock
            .Setup(x => x.ValidateRegistrationTokenAsync("invalid-token"))
            .ReturnsAsync((string?)null);

        var command = new CompleteRegisterCommand("invalid-token", "İlkin", "Rəcəbov", null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Qeydiyyat tokeni*");
    }

    [Fact]
    public async Task Handle_WhenTokenIsValid_ShouldCreateUserAndReturnTokens()
    {
        // Arrange
        _magicLinkServiceMock
            .Setup(x => x.ValidateRegistrationTokenAsync("valid-token"))
            .ReturnsAsync("ilkin@pncm.az");

        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(IdentityResult.Success);

        _tokenServiceMock
            .Setup(x => x.GenerateAccessToken(It.IsAny<AppUser>()))
            .Returns("access-token-123");

        _tokenServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token-123");

        var command = new CompleteRegisterCommand("valid-token", "İlkin", "Rəcəbov", "+994706600017");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AccessToken.Should().Be("access-token-123");
        result.RefreshToken.Should().Be("refresh-token-123");
        result.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(15), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_WhenUserCreationFails_ShouldThrowValidationException()
    {
        // Arrange
        _magicLinkServiceMock
            .Setup(x => x.ValidateRegistrationTokenAsync("valid-token"))
            .ReturnsAsync("ilkin@pncm.az");

        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError 
            { 
                Description = "Email artıq mövcuddur." 
            }));

        var command = new CompleteRegisterCommand("valid-token", "İlkin", "Rəcəbov", null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}