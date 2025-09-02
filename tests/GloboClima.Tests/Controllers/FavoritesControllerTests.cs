using GloboClima.Core.Common;
using GloboClima.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace GloboClima.Tests.Controllers
{
    public class FavoritesControllerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly FavoritesController _controller;

        public FavoritesControllerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _controller = new FavoritesController(_mockUserRepository.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user1")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task AddFavoriteCity_ReturnsOk_WhenSuccess()
        {
            _mockUserRepository
                .Setup(r => r.AddFavoriteCityAsync("user1", "São Paulo"))
                .ReturnsAsync(ServiceResult<bool>.Success(true));

            var result = await _controller.AddFavoriteCity("São Paulo");

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddFavoriteCity_ReturnsBadRequest_WhenFailure()
        {
            _mockUserRepository
                .Setup(r => r.AddFavoriteCityAsync("user1", "São Paulo"))
                .ReturnsAsync(ServiceResult<bool>.Failure("Erro"));

            var result = await _controller.AddFavoriteCity("São Paulo");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Erro", badRequest.Value);
        }

        [Fact]
        public async Task GetFavoriteCities_ReturnsOkWithData()
        {
            var favorites = new List<string> { "São Paulo", "Rio de Janeiro" };
            _mockUserRepository
                .Setup(r => r.GetFavoriteCitiesAsync("user1"))
                .ReturnsAsync(ServiceResult<List<string>>.Success(favorites));

            var result = await _controller.GetFavoriteCities();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<List<string>>(okResult.Value);
            Assert.Equal(2, data.Count);
        }

        [Fact]
        public async Task AddFavoriteCountry_ReturnsOk_WhenSuccess()
        {
            _mockUserRepository
                .Setup(r => r.AddFavoriteCountryAsync("user1", "Brazil"))
                .ReturnsAsync(ServiceResult<bool>.Success(true));

            var result = await _controller.AddFavoriteCountry("Brazil");

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task RemoveFavoriteCountry_ReturnsBadRequest_WhenFailure()
        {
            _mockUserRepository
                .Setup(r => r.RemoveFavoriteCountryAsync("user1", "Brazil"))
                .ReturnsAsync(ServiceResult<bool>.Failure("Erro"));

            var result = await _controller.RemoveFavoriteCountry("Brazil");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Erro", badRequest.Value);
        }

        [Fact]
        public async Task GetFavoriteCountries_ReturnsOkWithData()
        {
            var countries = new List<string> { "Brazil", "USA" };
            _mockUserRepository
                .Setup(r => r.GetFavoriteCountriesAsync("user1"))
                .ReturnsAsync(ServiceResult<List<string>>.Success(countries));

            var result = await _controller.GetFavoriteCountries();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<List<string>>(okResult.Value);
            Assert.Equal(2, data.Count);
        }
    }
}
