using GloboClima.Core.Models;

namespace GloboClima.Frontend.Services
{
    public class WeatherService
    {
        private readonly ApiService _apiService;

        public WeatherService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<WeatherData?> GetWeatherAsync(string city, string? country = null)
        {
            var endpoint = $"api/weather/{city}";
            if (!string.IsNullOrEmpty(country))
                endpoint += $"?country={country}";

            return await _apiService.GetAsync<WeatherData>(endpoint);
        }

        public async Task<bool> AddFavoriteCityAsync(string city)
        {
            return await _apiService.PostAsync($"api/favorites/cities/{city}");
        }

        public async Task<bool> RemoveFavoriteCityAsync(string city)
        {
            return await _apiService.DeleteAsync($"api/favorites/cities/{city}");
        }

        public async Task<List<string>> GetFavoriteCitiesAsync()
        {
            var cities = await _apiService.GetAsync<List<string>>("api/favorites/cities");
            return cities ?? new List<string>();
        }
    }
}