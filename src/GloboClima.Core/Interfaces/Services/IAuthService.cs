using GloboClima.Core.Common;
using GloboClima.Core.DTOs;
using GloboClima.Core.Models;

namespace GloboClima.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request);
        Task<ServiceResult<bool>> ValidateTokenAsync(string token);
        string GenerateJwtToken(User user);
    }
}
