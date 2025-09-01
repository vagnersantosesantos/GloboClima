using GloboClima.Core.Common;
using GloboClima.Core.Models;

namespace GloboClima.Core.Interfaces.Services
{
    public interface IWeatherService
    {
        Task<ServiceResult<WeatherData>> GetWeatherAsync(string city, string? country = null);
    }
}
