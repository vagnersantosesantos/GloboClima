using System.Threading.Tasks;
using GloboClima.Core.Common;
using GloboClima.Core.DTOs;
using GloboClima.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GloboClima.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var request = new RegisterRequest { Email = "teste@teste.com", Password = "123456" };
            var response = ServiceResult<AuthResponse>.Success(
                new AuthResponse
                {
                    Token = "fake-jwt",
                    UserId = "1",
                    ExpiresAt = DateTime.Now.AddHours(24)
                }
            );

            _authServiceMock
                .Setup(s => s.RegisterAsync(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<AuthResponse>(okResult.Value);
            Assert.Equal("fake-jwt", value.Token);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenFailure()
        {
            // Arrange
            var request = new RegisterRequest { Email = "teste@teste.com", Password = "123456" };

            var response = ServiceResult<AuthResponse>.Failure("Erro ao registrar");

            _authServiceMock
                .Setup(s => s.RegisterAsync(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Erro ao registrar", badRequest.Value);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var request = new LoginRequest { Email = "teste@teste.com", Password = "123456" };

            var response = ServiceResult<AuthResponse>.Success(
                new AuthResponse
                {
                    Token = "fake-jwt",
                    UserId = "1",
                    ExpiresAt = DateTime.Now.AddHours(24)
                }
            );

            _authServiceMock
                .Setup(s => s.LoginAsync(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<AuthResponse>(okResult.Value);
            Assert.Equal("fake-jwt", value.Token);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenFailure()
        {
            // Arrange
            var request = new LoginRequest { Email = "teste@teste.com", Password = "123456" };
            var response = ServiceResult<AuthResponse>.Failure("Credenciais inválidas");

            _authServiceMock
                .Setup(s => s.LoginAsync(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Credenciais inválidas", unauthorized.Value);
        }
    }
}