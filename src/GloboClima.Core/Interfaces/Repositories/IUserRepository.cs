using GloboClima.Core.Common;
using GloboClima.Core.Models;

namespace GloboClima.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<ServiceResult<User>> GetByIdAsync(string id);
        Task<ServiceResult<User>> GetByEmailAsync(string email);
        Task<ServiceResult<User>> CreateAsync(User user);
        Task<ServiceResult<User>> UpdateAsync(User user);
        Task<ServiceResult<bool>> AddFavoriteCityAsync(string userId, string city);
        Task<ServiceResult<bool>> RemoveFavoriteCityAsync(string userId, string city);
        Task<ServiceResult<List<string>>> GetFavoriteCitiesAsync(string userId);
        Task<ServiceResult<bool>> AddFavoriteCountryAsync(string userId, string country);
        Task<ServiceResult<bool>> RemoveFavoriteCountryAsync(string userId, string country);
        Task<ServiceResult<List<string>>> GetFavoriteCountriesAsync(string userId);
    }
}
