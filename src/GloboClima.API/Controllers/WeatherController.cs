using GloboClima.Core.Interfaces.Services;
using GloboClima.Core.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    /// <summary>
    /// Obtém informações climáticas de uma cidade
    /// </summary>
    /// <param name="city">Nome da cidade</param>
    /// <param name="country">Código do país (opcional)</param>
    [HttpGet("{city}")]
    [ProducesResponseType(typeof(WeatherData), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<WeatherData>> GetWeather(
        string city,
        [FromQuery] string? country = null)
    {
        var result = await _weatherService.GetWeatherAsync(city, country);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Data);
    }
}