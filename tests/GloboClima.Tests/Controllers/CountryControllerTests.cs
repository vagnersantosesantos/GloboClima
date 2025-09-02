using GloboClima.Core.Common;
using GloboClima.Core.Models;
using GloboClima.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GloboClima.Tests.Controllers
{
    public class CountryControllerTests
    {
        private readonly Mock<ICountryService> _mockCountryService;
        private readonly CountryController _controller;

        public CountryControllerTests()
        {
            _mockCountryService = new Mock<ICountryService>();
            _controller = new CountryController(_mockCountryService.Object);
        }

        [Fact]
        public async Task GetCountry_ReturnsOk_WhenServiceSucceeds()
        {
            // Arrange
            var countryName = "Brazil";
            var countryData = new CountryNameData { Name = "Brazil", Cca2 = "BR" };
            _mockCountryService
                .Setup(s => s.GetCountryAsync(countryName))
                .ReturnsAsync(ServiceResult<CountryNameData>.Success(countryData));

            // Act
            var result = await _controller.GetCountry(countryName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<CountryNameData>(okResult.Value);
            Assert.Equal("Brazil", data.Name);
        }

        [Fact]
        public async Task GetCountry_ReturnsNotFound_WhenServiceFails()
        {
            // Arrange
            var countryName = "UnknownCountry";
            _mockCountryService
                .Setup(s => s.GetCountryAsync(countryName))
                .ReturnsAsync(ServiceResult<CountryNameData>.Failure("Country not found"));

            // Act
            var result = await _controller.GetCountry(countryName);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Country not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetAllCountries_ReturnsOkWithList()
        {
            // Arrange
            var countries = new List<CountryData>
            {
                new CountryData { Name = "Brazil", Capital = "Brasília" },
                new CountryData { Name = "USA", Capital = "Washington D.C." }
            };
            _mockCountryService
                .Setup(s => s.GetAllCountriesAsync())
                .ReturnsAsync(ServiceResult<List<CountryData>>.Success(countries));

            // Act
            var result = await _controller.GetAllCountries();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<List<CountryData>>(okResult.Value);
            Assert.Equal(2, data.Count);
        }
    }
}
