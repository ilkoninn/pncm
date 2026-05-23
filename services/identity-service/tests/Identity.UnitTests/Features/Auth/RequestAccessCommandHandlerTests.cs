public sealed class RequestAccessCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IMagicLinkService> _magicLinkServiceMock = new();
    private readonly RequestAccessCommandHandler _handler;

    public RequestAccessCommandHandlerTests()
    {
        _handler = new RequestAccessCommandHandler(
            _userRepositoryMock.Object,
            _magicLinkServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldReturnIsNewUserTrue()
    {
        // Arrange
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("new@pncm.az"))
            .ReturnsAsync((AppUser?)null);

        _magicLinkServiceMock
            .Setup(x => x.GenerateMagicLinkTokenAsync("new@pncm.az"))
            .ReturnsAsync("token123");

        var command = new RequestAccessCommand("new@pncm.az", EClient.Web);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsNewUser.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenUserExists_ShouldReturnIsNewUserFalse()
    {
        // Arrange
        var existingUser = new AppUser 
        { 
            Email = "existing@pncm.az",
            FirstName = "Test",
            LastName = "User"
        };
        
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("existing@pncm.az"))
            .ReturnsAsync(existingUser);

        _magicLinkServiceMock
            .Setup(x => x.GenerateMagicLinkTokenAsync("existing@pncm.az"))
            .ReturnsAsync("token123");

        var command = new RequestAccessCommand("existing@pncm.az", EClient.Web);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsNewUser.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenClientIsMobile_ShouldCallGenerateOtp()
    {
        // Arrange
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("mobile@pncm.az"))
            .ReturnsAsync((AppUser?)null);

        _magicLinkServiceMock
            .Setup(x => x.GenerateOtpAsync("mobile@pncm.az"))
            .ReturnsAsync("123456");

        var command = new RequestAccessCommand("mobile@pncm.az", EClient.Mobile);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _magicLinkServiceMock.Verify(x => x.GenerateOtpAsync("mobile@pncm.az"), Times.Once);
        result.IsNewUser.Should().BeTrue();
    }
}