using GloboClima.Core.Common;
using GloboClima.Core.Interfaces.Services;
using GloboClima.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GloboClima.Tests.Controllers
{
    public class WeatherControllerTests
    {
        private readonly Mock<IWeatherService> _mockWeatherService;
        private readonly WeatherController _controller;

        public WeatherControllerTests()
        {
            _mockWeatherService = new Mock<IWeatherService>();
            _controller = new WeatherController(_mockWeatherService.Object);
        }

        [Fact]
        public async Task GetWeather_ReturnsOk_WhenServiceSucceeds()
        {
            var weatherData = new WeatherData
            {
                CityName = "São Paulo",
                Country = "BR",
                Temperature = 25
            };

            _mockWeatherService
                .Setup(s => s.GetWeatherAsync("São Paulo", null))
                .ReturnsAsync(ServiceResult<WeatherData>.Success(weatherData));

            var result = await _controller.GetWeather("São Paulo");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<WeatherData>(okResult.Value);
            Assert.Equal("São Paulo", data.CityName);
            Assert.Equal("BR", data.Country);
            Assert.Equal(25, data.Temperature);
        }

        [Fact]
        public async Task GetWeather_ReturnsNotFound_WhenServiceFails()
        {
            _mockWeatherService
                .Setup(s => s.GetWeatherAsync("CidadeInexistente", null))
                .ReturnsAsync(ServiceResult<WeatherData>.Failure("Cidade não encontrada"));

            var result = await _controller.GetWeather("CidadeInexistente");

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Cidade não encontrada", notFoundResult.Value);
        }

        [Fact]
        public async Task GetWeather_ReturnsOk_WithCountryParameter()
        {
            var weatherData = new WeatherData
            {
                CityName = "Paris",
                Country = "FR",
                Temperature = 18
            };

            _mockWeatherService
                .Setup(s => s.GetWeatherAsync("Paris", "FR"))
                .ReturnsAsync(ServiceResult<WeatherData>.Success(weatherData));

            var result = await _controller.GetWeather("Paris", "FR");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<WeatherData>(okResult.Value);
            Assert.Equal("Paris", data.CityName);
            Assert.Equal("FR", data.Country);
        }
    }
}
