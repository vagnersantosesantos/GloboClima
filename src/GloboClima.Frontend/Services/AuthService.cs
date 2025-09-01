// GloboClima.Frontend/Services/AuthService.cs
using GloboClima.Core.DTOs;

namespace GloboClima.Frontend.Services
{
    public class AuthService
    {
        private readonly ApiService _apiService;
        private readonly IServiceProvider _serviceProvider;
        private string? _currentToken;
        private string? _currentUserId;

        public AuthService(ApiService apiService, IServiceProvider serviceProvider)
        {
            _apiService = apiService;
            _serviceProvider = serviceProvider;
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(_currentToken);
        public string? UserId => _currentUserId;

        public async Task<bool> LoginAsync(string email, string password)
        {
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var response = await _apiService.PostAsync<LoginRequest, AuthResponse>(
                "api/auth/login", loginRequest);

            if (response != null)
            {
                _currentToken = response.Token;
                _currentUserId = response.UserId;
                _apiService.SetAuthToken(response.Token);

                var authStateProvider = _serviceProvider.GetRequiredService<CustomAuthStateProvider>();
                authStateProvider.MarkUserAsAuthenticated(response.UserId, email);

                return true;
            }

            return false;
        }

        public async Task<bool> RegisterAsync(string email, string password, string confirmPassword)
        {
            var registerRequest = new RegisterRequest
            {
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };

            var response = await _apiService.PostAsync<RegisterRequest, AuthResponse>(
                "api/auth/register", registerRequest);

            if (response != null)
            {
                _currentToken = response.Token;
                _currentUserId = response.UserId;
                _apiService.SetAuthToken(response.Token);

                var authStateProvider = _serviceProvider.GetRequiredService<CustomAuthStateProvider>();
                authStateProvider.MarkUserAsAuthenticated(response.UserId, email);

                return true;
            }

            return false;
        }

        public Task LogoutAsync()
        {
            _currentToken = null;
            _currentUserId = null;

            var authStateProvider = _serviceProvider.GetRequiredService<CustomAuthStateProvider>();
            authStateProvider.MarkUserAsLoggedOut();

            return Task.CompletedTask;
        }
    }
}