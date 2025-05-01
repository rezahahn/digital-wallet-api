using DigitalWallet.Application.DTOs.Messaging;
using DigitalWallet.Application.DTOs.User;
using DigitalWallet.Application.Services;
using DigitalWallet.Core.Entities;
using DigitalWallet.Core.Exceptions;
using DigitalWallet.Core.Interfaces;
using DigitalWallet.Core.Interfaces.Messaging;
using DigitalWallet.Infrastructure.Common.Helpers;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Net;

namespace DigitalWallet.Tests.Services
{
    public class UserServiceTests
    {
        protected readonly Mock<IUserRepository> _userRepositoryMock;
        protected readonly Mock<IPasswordHelper> _passwordHelperMock;
        protected readonly Mock<IUnitOfWork> _unitOfWorkMock;
        protected readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        protected readonly Mock<IAuditLogRepository> _auditLogRepositoryMock;
        protected readonly Mock<IEventBus> _eventBusMock;
        protected readonly UserService _userService;
        protected readonly VerifyKycDto _validKycDto;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHelperMock = new Mock<IPasswordHelper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _auditLogRepositoryMock = new Mock<IAuditLogRepository>();
            _eventBusMock = new Mock<IEventBus>();

            // Setup HttpContext mock
            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
            httpContext.Request.Headers["User-Agent"] = "TestAgent";
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            _userService = new UserService(
                _userRepositoryMock.Object,
                _passwordHelperMock.Object,
                _unitOfWorkMock.Object,
                _httpContextAccessorMock.Object,
                _auditLogRepositoryMock.Object,
                _eventBusMock.Object);

            _validKycDto = new VerifyKycDto
            {
                UserId = 1,
                DocumentType = "KTP",
                DocumentNumber = "1234567890"
            };
        }

        private readonly CreateUserDto _validUserDto = new()
        {
            Username = "testuser",
            Email = "test@example.com",
            PhoneNumber = "+628123456789",
            FullName = "Test User",
            Password = "P@ssw0rd123!"
        };

        [Fact]
        public async Task CreateUserAsync_Success_ReturnsUserDto()
        {
            // Arrange
            var expectedSalt = "somesalt";
            var expectedHash = "hashedpassword";

            _userRepositoryMock.Setup(x => x.EmailExistsAsync(_validUserDto.Email))
                .ReturnsAsync(false);

            _userRepositoryMock.Setup(x => x.PhoneNumberExistsAsync(_validUserDto.PhoneNumber))
                .ReturnsAsync(false);

            _passwordHelperMock.Setup(x => x.GenerateSalt())
                .Returns(expectedSalt);

            _passwordHelperMock.Setup(x => x.HashPassword(_validUserDto.Password, expectedSalt))
                .Returns(expectedHash);

            _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>()))
                .Callback<User>(user =>
                {
                    user.UserId = 1;
                });

            // Act
            var result = await _userService.CreateUserAsync(_validUserDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
            Assert.Equal(_validUserDto.Email, result.Email);
            Assert.Equal(_validUserDto.PhoneNumber, result.PhoneNumber);

            _userRepositoryMock.Verify(x => x.AddAsync(It.Is<User>(u =>
                u.PasswordHash == expectedHash &&
                u.Salt == expectedSalt &&
                u.KYCVerified == false &&
                u.IsActive == true)), Times.Once);

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_EmailExists_ThrowsException()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.EmailExistsAsync(_validUserDto.Email))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<EmailAlreadyExistsException>(() =>
                _userService.CreateUserAsync(_validUserDto));

            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task GetUserByIdAsync_Success_ReturnsUserDto()
        {
            // Arrange
            var testUser = new User
            {
                UserId = 1,
                Email = "test@example.com"
            };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(testUser.UserId))
                .ReturnsAsync(testUser);

            // Act
            var result = await _userService.GetUserByIdAsync(testUser.UserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testUser.UserId, result.UserId);
            Assert.Equal(testUser.Email, result.Email);
        }

        [Fact]
        public async Task WhenInputValid_ReturnsUserDto()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.EmailExistsAsync(_validUserDto.Email))
                .ReturnsAsync(false);
            _userRepositoryMock.Setup(x => x.PhoneNumberExistsAsync(_validUserDto.PhoneNumber))
                .ReturnsAsync(false);

            _passwordHelperMock.Setup(x => x.GenerateSalt())
                .Returns("generated-salt");
            _passwordHelperMock.Setup(x => x.HashPassword(_validUserDto.Password, "generated-salt"))
                .Returns("hashed-password");

            User? addedUser = null;
            _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>()))
                .Callback<User>(u =>
                {
                    addedUser = u;
                    u.UserId = 1;
                });

            // Act
            var result = await _userService.CreateUserAsync(_validUserDto);

            // Assert
            Assert.Equal(1, result.UserId);
            Assert.Equal(_validUserDto.Email, result.Email);

            Assert.Equal("hashed-password", addedUser.PasswordHash);
            Assert.Equal("generated-salt", addedUser.Salt);

            _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task WhenEmailExists_ThrowsEmailAlreadyExistsException()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.EmailExistsAsync(_validUserDto.Email))
                .ReturnsAsync(true);

            // Act
            var exception = await Assert.ThrowsAsync<EmailAlreadyExistsException>(
                () => _userService.CreateUserAsync(_validUserDto));

            // Assert
            Assert.Equal($"Email '{_validUserDto.Email}' is already registered", exception.Message);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task WhenPhoneExists_ThrowsPhoneAlreadyExistsException()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.PhoneNumberExistsAsync(_validUserDto.PhoneNumber))
                .ReturnsAsync(true);

            // Act
            var exception = await Assert.ThrowsAsync<PhoneNumberAlreadyExistsException>(
                () => _userService.CreateUserAsync(_validUserDto));

            // Assert
            Assert.Equal($"Phone number '{_validUserDto.PhoneNumber}' is already registered", exception.Message);
            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task WhenUserExists_PublishesEventAndReturnsTrue()
        {
            // Arrange
            var user = new User { UserId = _validKycDto.UserId, KYCVerified = false };
            _userRepositoryMock.Setup(x => x.GetByIdAsync(_validKycDto.UserId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.VerifyKycAsync(_validKycDto);

            // Assert
            Assert.True(result);
            Assert.True(user.KYCVerified);

            _eventBusMock.Verify(x => x.Publish(
                "create_wallet_queue",
                It.Is<CreateWalletMessage>(m => m.UserId == _validKycDto.UserId)),
                Times.Once);

            _auditLogRepositoryMock.Verify(x => x.AddAsync(
                It.Is<AuditLog>(l =>
                    l.UserId == _validKycDto.UserId &&
                    l.ActionType == "KYC_VERIFICATION" &&
                    l.ActionDetails.Contains(_validKycDto.DocumentType))),
                Times.Once);
        }

        [Fact]
        public async Task WhenUserNotFound_ThrowsUserNotFoundException()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByIdAsync(_validKycDto.UserId))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(
                () => _userService.VerifyKycAsync(_validKycDto));

            _eventBusMock.Verify(x => x.Publish(It.IsAny<string>(), It.IsAny<object>()),
                Times.Never);
        }

        [Fact]
        public async Task WhenUserExists_ReturnsUserDto()
        {
            // Arrange
            var testEmail = "existing@example.com";
            var testUser = new User
            {
                UserId = 1,
                Email = testEmail,
                FullName = "Test User"
            };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(testEmail))
                .ReturnsAsync(testUser);

            // Act
            var result = await _userService.GetUserByEmailAsync(testEmail);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testUser.UserId, result.UserId);
            Assert.Equal(testUser.Email, result.Email);
        }

        [Fact]
        public async Task WhenUserNotExists_ReturnsNull()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.Null(result);
        }
    }
}
