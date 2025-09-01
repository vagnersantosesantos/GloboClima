using GloboClima.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public FavoritesController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Adiciona uma cidade aos favoritos
    /// </summary>
    [HttpPost("cities/{cityName}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddFavoriteCity(string cityName)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _userRepository.AddFavoriteCityAsync(userId, cityName);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok();
    }

    /// <summary>
    /// Remove uma cidade dos favoritos
    /// </summary>
    [HttpDelete("cities/{cityName}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> RemoveFavoriteCity(string cityName)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _userRepository.RemoveFavoriteCityAsync(userId, cityName);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok();
    }

    /// <summary>
    /// Lista cidades favoritas do usuário
    /// </summary>
    [HttpGet("cities")]
    [ProducesResponseType(typeof(List<string>), 200)]
    public async Task<ActionResult<List<string>>> GetFavoriteCities()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _userRepository.GetFavoriteCitiesAsync(userId);

        return Ok(result.Data ?? new List<string>());
    }

    /// <summary>
    /// Adiciona um país aos favoritos
    /// </summary>
    [HttpPost("countries/{countryName}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddFavoriteCountry(string countryName)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _userRepository.AddFavoriteCountryAsync(userId, countryName);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok();
    }

    /// <summary>
    /// Remove um país dos favoritos
    /// </summary>
    [HttpDelete("countries/{countryName}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> RemoveFavoriteCountry(string countryName)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _userRepository.RemoveFavoriteCountryAsync(userId, countryName);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok();
    }

    /// <summary>
    /// Lista países favoritos do usuário
    /// </summary>
    [HttpGet("countries")]
    [ProducesResponseType(typeof(List<string>), 200)]
    public async Task<ActionResult<List<string>>> GetFavoriteCountries()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _userRepository.GetFavoriteCountriesAsync(userId);

        return Ok(result.Data ?? new List<string>());
    }
}