using GloboClima.Infrastructure.External.ApiResponses;
using GloboClima.Services.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using static GloboClima.Infrastructure.External.ApiResponses.OpenWeatherMapResponse;

namespace GloboClima.Tests.Services
{
    public class WeatherServiceTests
    {
        private class FakeHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
        {
            private readonly HttpResponseMessage _response = response;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_response);
            }
        }

        private WeatherService CreateService(HttpResponseMessage response)
        {
            var handler = new FakeHttpMessageHandler(response);
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/")
            };

            var factory = new HttpClientFactoryStub(httpClient);
            var inMemorySettings = new Dictionary<string, string>
            {
                { "OpenWeatherMap:ApiKey", "fake_api_key" }
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            return new WeatherService(factory, configuration);
        }

        [Fact]
        public async Task GetWeatherAsync_ReturnsSuccess_WhenApiResponseIsValid()
        {
            var weatherResponse = new OpenWeatherMapResponse
            {
                Name = "São Paulo",
                Sys = new SysData { Country = "BR", Sunrise = 1693640000, Sunset = 1693680000 },
                Main = new MainData { Temp = 25, FeelsLike = 26, TempMin = 20, TempMax = 30, Pressure = 1000, Humidity = 80 },
                Wind = new WindData { Speed = 5, Deg = 90, Gust = 7 },
                Clouds = new CloudsData { All = 40 },
                Coord = new CoordData { Lat = -23.55, Lon = -46.63 },
                Dt = 1693650000,
                Weather = [new WeatherInfo { Main = "Clear", Description = "céu limpo", Icon = "01d" }],
                Visibility = 10000,
                Timezone = -10800
            };

            var json = JsonSerializer.Serialize(weatherResponse);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var service = CreateService(response);

            var result = await service.GetWeatherAsync("São Paulo");

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("São Paulo", result.Data.CityName);
            Assert.Equal("BR", result.Data.Country);
        }

        [Fact]
        public async Task GetWeatherAsync_ReturnsFailure_WhenCityNotFound()
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);

            var service = CreateService(response);

            var result = await service.GetWeatherAsync("CidadeInexistente");

            Assert.False(result.IsSuccess);
            Assert.Equal("Cidade não encontrada", result.Error);
        }

        [Fact]
        public async Task GetWeatherAsync_ReturnsFailure_WhenExceptionOccurs()
        {
            var handler = new FakeHttpMessageHandler(null!);
            var httpClient = new HttpClient(handler);
            var factory = new HttpClientFactoryStub(httpClient);

            var inMemorySettings = new Dictionary<string, string>
            {
                { "OpenWeatherMap:ApiKey", "fake_api_key" }
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var service = new WeatherService(factory, configuration);

            var result = await service.GetWeatherAsync("São Paulo");

            Assert.False(result.IsSuccess);
            Assert.Contains("Erro ao consultar clima", result.Error);
        }
    }

    public class HttpClientFactoryStub(HttpClient client) : IHttpClientFactory
    {
        private readonly HttpClient _client = client;

        public HttpClient CreateClient(string name) => _client;
    }
}
