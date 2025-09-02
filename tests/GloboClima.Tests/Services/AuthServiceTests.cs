using GloboClima.Core.Common;
using GloboClima.Core.DTOs;
using GloboClima.Core.Interfaces.Repositories;
using GloboClima.Core.Models;
using GloboClima.Services.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace GloboClima.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();

            // Configuração fake do JWT
            _configurationMock.Setup(c => c["JWT:Secret"]).Returns("fakeZkKj9qL9B1GZcVhdh0O7m7FztQ7m3O8wUOP6m9Ck3X8");
            _configurationMock.Setup(c => c["JWT:Issuer"]).Returns("globo-clima");
            _configurationMock.Setup(c => c["JWT:Audience"]).Returns("globo-cliente");

            _authService = new AuthService(_userRepositoryMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldFail_WhenPasswordsDoNotMatch()
        {
            var request = new RegisterRequest
            {
                Email = "test@test.com",
                Password = "123",
                ConfirmPassword = "321"
            };

            var result = await _authService.RegisterAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal("Senhas não coincidem", result.Error);
        }

        [Fact]
        public async Task RegisterAsync_ShouldFail_WhenUserAlreadyExists()
        {
            var request = new RegisterRequest
            {
                Email = "test@test.com",
                Password = "123",
                ConfirmPassword = "123"
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(ServiceResult<User>.Success(new User { Email = request.Email }));

            var result = await _authService.RegisterAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal("Email já cadastrado", result.Error);
        }

        [Fact]
        public async Task RegisterAsync_ShouldSucceed_WhenValidData()
        {
            var request = new RegisterRequest
            {
                Email = "test@test.com",
                Password = "123",
                ConfirmPassword = "123"
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(ServiceResult<User>.Failure("Not found"));

            _userRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(ServiceResult<User>.Success(new User { Id = "1", Email = request.Email }));

            var result = await _authService.RegisterAsync(request);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data.Token);
        }

        [Fact]
        public async Task LoginAsync_ShouldFail_WhenUserNotFound()
        {
            var request = new LoginRequest { Email = "notfound@test.com", Password = "123" };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(ServiceResult<User>.Failure("Not found"));

            var result = await _authService.LoginAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal("Credenciais inválidas", result.Error);
        }

        [Fact]
        public async Task LoginAsync_ShouldFail_WhenPasswordInvalid()
        {
            var request = new LoginRequest { Email = "test@test.com", Password = "wrong" };

            var user = new User
            {
                Id = "1",
                Email = request.Email,
                PasswordHash = "invalidhash"
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(ServiceResult<User>.Success(user));

            var result = await _authService.LoginAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal("Credenciais inválidas", result.Error);
        }

        [Fact]
        public async Task LoginAsync_ShouldSucceed_WhenCredentialsValid()
        {
            var request = new LoginRequest { Email = "test@test.com", Password = "123" };

            var user = new User
            {
                Id = "1",
                Email = request.Email,
                PasswordHash = _authService.GenerateJwtToken(new User { Id = "1", Email = request.Email }) // forçar hash válido
            };

            user.PasswordHash = typeof(AuthService)
                .GetMethod("HashPassword", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_authService, new object[] { "123" }) as string;

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(ServiceResult<User>.Success(user));

            var result = await _authService.LoginAsync(request);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data.Token);
        }

        [Fact]
        public async Task ValidateTokenAsync_ShouldReturnTrue_WhenValidToken()
        {
            var user = new User { Id = "1", Email = "test@test.com" };
            var token = _authService.GenerateJwtToken(user);

            var result = await _authService.ValidateTokenAsync(token);

            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task ValidateTokenAsync_ShouldReturnFalse_WhenInvalidToken()
        {
            var result = await _authService.ValidateTokenAsync("invalid.token.here");

            Assert.True(result.IsSuccess);
            Assert.False(result.Data);
        }
    }
}
