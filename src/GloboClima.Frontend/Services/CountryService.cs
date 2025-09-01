using GloboClima.Core.Models;

namespace GloboClima.Frontend.Services
{
    public class CountryService
    {
        private readonly ApiService _apiService;

        public CountryService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<CountryNameData?> GetCountryAsync(string countryName) 
        {
            return await _apiService.GetAsync<CountryNameData>($"api/country/{countryName}");
        }

        public async Task<List<CountryData>> GetAllCountriesAsync()
        {
            var countries = await _apiService.GetAsync<List<CountryData>>("api/country");
            return countries ?? new List<CountryData>();
        }

        public async Task<bool> AddFavoriteCountryAsync(string country)
        {
            return await _apiService.PostAsync($"api/favorites/countries/{country}");
        }

        public async Task<bool> RemoveFavoriteCountryAsync(string country)
        {
            return await _apiService.DeleteAsync($"api/favorites/countries/{country}");
        }

        public async Task<List<string>> GetFavoriteCountriesAsync()
        {
            var countries = await _apiService.GetAsync<List<string>>("api/favorites/countries");
            return countries ?? new List<string>();
        }
    }
}