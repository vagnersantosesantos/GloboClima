using GloboClima.Core.Common;
using GloboClima.Core.Interfaces.Services;
using GloboClima.Core.Models;
using GloboClima.Infrastructure.External.ApiResponses;
using System.Text.Json;

namespace GloboClima.Services.Services
{
    public class CountryService : ICountryService
    {
        private readonly HttpClient _httpClient;

        public CountryService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("RestCountries");
        }

        public async Task<ServiceResult<CountryNameData>> GetCountryAsync(string countryName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"name/{countryName}");

                if (!response.IsSuccessStatusCode)
                    return ServiceResult<CountryNameData>.Failure($"Erro ao consultar país: {response.StatusCode}");

                var json = await response.Content.ReadAsStringAsync();

                var countries = JsonSerializer.Deserialize<RestCountryNameResponse[]>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (countries != null && countries.Length > 0)
                {
                    var countryData = MapToCountryNameData(countries[0]);
                    return ServiceResult<CountryNameData>.Success(countryData);
                }

                return ServiceResult<CountryNameData>.Failure("País não encontrado");
            }
            catch (Exception ex)
            {
                return ServiceResult<CountryNameData>.Failure($"Erro ao consultar país: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<CountryData>>> GetAllCountriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("all?fields=name,flags,capital");

                if (!response.IsSuccessStatusCode)
                    return ServiceResult<List<CountryData>>.Failure($"Erro ao consultar países: {response.StatusCode}");

                var json = await response.Content.ReadAsStringAsync();

                var countries = JsonSerializer.Deserialize<RestCountriesResponse[]>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var countryDataList = countries?.Select(MapToCountryData).ToList() ?? new List<CountryData>();

                return ServiceResult<List<CountryData>>.Success(countryDataList);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<CountryData>>.Failure($"Erro ao consultar países: {ex.Message}");
            }
        }

        private static CountryData MapToCountryData(RestCountriesResponse country)
        {
            return new CountryData
            {
                Name = country.Name.Common,
                Capital = country.Capital?.FirstOrDefault() ?? "N/A",
                Population = country.Population,
                Region = country.Region,
                Languages = country.Languages?.Values.ToList() ?? new List<string>(),
                Currencies = country.Currencies?.Keys.ToList() ?? new List<string>(),
                Flag = country.Flags.Png,
                Area = country.Area
            };
        }

        private static CountryNameData MapToCountryNameData(RestCountryNameResponse country)
        {
            return new CountryNameData
            {
                Name = country.Name.Common,
                OfficialName = country.Name.Official,
                NativeName = country.Name.NativeName?.Values.FirstOrDefault()?.Common ?? string.Empty,

                Tld = country.Tld,
                Cca2 = country.Cca2,
                Cca3 = country.Cca3,
                Cioc = country.Cioc,

                Independent = country.Independent,
                Status = country.Status,
                UnMember = country.UnMember,

                Currencies = country.Currencies?.ToDictionary(
                    c => c.Key,
                    c => new CountryNameData.CurrencyData
                    {
                        Name = c.Value.Name,
                        Symbol = c.Value.Symbol
                    }) ?? new(),

                Capital = country.Capital,
                Region = country.Region,
                Subregion = country.Subregion,

                Languages = country.Languages ?? new(),
                Latlng = country.Latlng,
                Landlocked = country.Landlocked,
                Borders = country.Borders,
                Area = country.Area,
                Population = country.Population,

                Translations = country.Translations?.ToDictionary(
                    t => t.Key,
                    t => new CountryNameData.TranslationData
                    {
                        Official = t.Value.Official,
                        Common = t.Value.Common
                    }) ?? new(),

                Flag = country.Flag,
                FlagPng = country.Flags.Png,
                FlagSvg = country.Flags.Svg
            };
        }

    }
}
