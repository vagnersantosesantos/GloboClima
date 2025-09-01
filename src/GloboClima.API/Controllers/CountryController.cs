using GloboClima.Core.Interfaces.Services;
using GloboClima.Core.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CountryController : ControllerBase
{
    private readonly ICountryService _countryService;

    public CountryController(ICountryService countryService)
    {
        _countryService = countryService;
    }

    /// <summary>
    /// Obtém informações de um país
    /// </summary>
    /// <param name="countryName">Nome do país</param>
    [HttpGet("{countryName}")]
    [ProducesResponseType(typeof(CountryData), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<CountryNameData>> GetCountry(string countryName)
    {
        var result = await _countryService.GetCountryAsync(countryName);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Data);
    }

    /// <summary>
    /// Lista todos os países
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CountryData>), 200)]
    public async Task<ActionResult<List<CountryData>>> GetAllCountries()
    {
        var result = await _countryService.GetAllCountriesAsync();
        return Ok(result.Data);
    }
}
