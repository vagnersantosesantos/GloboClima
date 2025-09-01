using GloboClima.Core.Common;
using GloboClima.Core.Models;

namespace GloboClima.Core.Interfaces.Services
{
    public interface ICountryService
    {
        Task<ServiceResult<CountryNameData>> GetCountryAsync(string countryName);
        Task<ServiceResult<List<CountryData>>> GetAllCountriesAsync();
    }
}
