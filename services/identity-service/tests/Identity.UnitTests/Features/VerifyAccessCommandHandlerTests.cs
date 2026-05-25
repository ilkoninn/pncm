public sealed class VerifyAccessCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IMagicLinkService> _magicLinkServiceMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly VerifyAccessCommandHandler _handler;

    public VerifyAccessCommandHandlerTests()
    {
        _handler = new VerifyAccessCommandHandler(
            _userRepositoryMock.Object,
            _magicLinkServiceMock.Object,
            _tokenServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenCodeIsInvalid_ShouldThrowValidationException()
    {
        // Arrange
        _magicLinkServiceMock
            .Setup(x => x.ValidateMagicCodeAsync("test@pncm.az", "000000"))
            .ReturnsAsync(false);

        var command = new VerifyAccessCommand("test@pncm.az", "000000", EClient.Web);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_WhenNewUser_ShouldReturnRegistrationToken()
    {
        // Arrange
        _magicLinkServiceMock
            .Setup(x => x.ValidateMagicCodeAsync("new@pncm.az", "123456"))
            .ReturnsAsync(true);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("new@pncm.az"))
            .ReturnsAsync((AppUser?)null);

        _magicLinkServiceMock
            .Setup(x => x.StoreRegistrationTokenAsync("new@pncm.az"))
            .ReturnsAsync("reg-token-123");

        var command = new VerifyAccessCommand("new@pncm.az", "123456", EClient.Web);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsNewUser.Should().BeTrue();
        result.RegistrationToken.Should().Be("reg-token-123");
        result.AccessToken.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenExistingUser_ShouldReturnTokens()
    {
        // Arrange
        var existingUser = new AppUser
        {
            Email = "existing@pncm.az",
            FirstName = "Test",
            LastName = "User"
        };

        _magicLinkServiceMock
            .Setup(x => x.ValidateMagicCodeAsync("existing@pncm.az", "123456"))
            .ReturnsAsync(true);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("existing@pncm.az"))
            .ReturnsAsync(existingUser);

        _tokenServiceMock
            .Setup(x => x.GenerateAccessToken(existingUser))
            .Returns("access-token-123");

        _tokenServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token-123");

        var command = new VerifyAccessCommand("existing@pncm.az", "123456", EClient.Web);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsNewUser.Should().BeFalse();
        result.AccessToken.Should().Be("access-token-123");
        result.RefreshToken.Should().Be("refresh-token-123");
        result.RegistrationToken.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenMobileClient_ShouldCallValidateOtp()
    {
        // Arrange
        _magicLinkServiceMock
            .Setup(x => x.ValidateOtpAsync("mobile@pncm.az", "123456"))
            .ReturnsAsync(false);

        var command = new VerifyAccessCommand("mobile@pncm.az", "123456", EClient.Mobile);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        _magicLinkServiceMock.Verify(x => x.ValidateOtpAsync("mobile@pncm.az", "123456"), Times.Once);
        _magicLinkServiceMock.Verify(x => x.ValidateMagicCodeAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}