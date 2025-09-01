using GloboClima.Core.Common;
using GloboClima.Core.Interfaces.Services;
using GloboClima.Core.Models;
using GloboClima.Infrastructure.External.ApiResponses;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;

namespace GloboClima.Services.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("OpenWeatherMap");
            _apiKey = configuration["OpenWeatherMap:ApiKey"];
        }

        public async Task<ServiceResult<WeatherData>> GetWeatherAsync(string city, string? country = null)
        {
            try
            {
                var query = string.IsNullOrEmpty(country) ? city : $"{city},{country}";
                var response = await _httpClient.GetAsync(
                    $"weather?q={query}&appid={_apiKey}&units=metric&lang=pt_br");

                if (!response.IsSuccessStatusCode)
                    return ServiceResult<WeatherData>.Failure("Cidade não encontrada");

                var json = await response.Content.ReadAsStringAsync();
                var weatherResponse = JsonSerializer.Deserialize<OpenWeatherMapResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var weatherData = new WeatherData
                {
                    CityName = weatherResponse.Name,
                    Country = weatherResponse.Sys.Country,

                    Temperature = weatherResponse.Main.Temp,
                    FeelsLike = weatherResponse.Main.FeelsLike,
                    TempMin = weatherResponse.Main.TempMin,
                    TempMax = weatherResponse.Main.TempMax,
                    Pressure = weatherResponse.Main.Pressure,
                    Humidity = weatherResponse.Main.Humidity,
                    SeaLevel = weatherResponse.Main.SeaLevel,
                    GrndLevel = weatherResponse.Main.GrndLevel,

                    WeatherMain = weatherResponse.Weather.FirstOrDefault()?.Main,
                    Description = weatherResponse.Weather.FirstOrDefault()?.Description,
                    Icon = weatherResponse.Weather.FirstOrDefault()?.Icon,

                    WindSpeed = weatherResponse.Wind.Speed,
                    WindDeg = weatherResponse.Wind.Deg,
                    WindGust = weatherResponse.Wind.Gust,

                    Rain1h = weatherResponse.Rain?.OneH,
                    CloudsAll = weatherResponse.Clouds.All,

                    Latitude = weatherResponse.Coord.Lat,
                    Longitude = weatherResponse.Coord.Lon,

                    Visibility = weatherResponse.Visibility,
                    DateTime = DateTimeOffset.FromUnixTimeSeconds(weatherResponse.Dt).UtcDateTime,
                    Sunrise = DateTimeOffset.FromUnixTimeSeconds(weatherResponse.Sys.Sunrise).UtcDateTime,
                    Sunset = DateTimeOffset.FromUnixTimeSeconds(weatherResponse.Sys.Sunset).UtcDateTime,

                    Timezone = weatherResponse.Timezone
                };

                return ServiceResult<WeatherData>.Success(weatherData);
            }
            catch (Exception ex)
            {
                return ServiceResult<WeatherData>.Failure($"Erro ao consultar clima: {ex.Message}");
            }
        }
    }
}
