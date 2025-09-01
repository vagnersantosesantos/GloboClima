using GloboClima.Core.Common;
using GloboClima.Core.DTOs;
using GloboClima.Core.Interfaces.Repositories;
using GloboClima.Core.Interfaces.Services;
using GloboClima.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace GloboClima.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Validar se senhas coincidem
                if (request.Password != request.ConfirmPassword)
                    return ServiceResult<AuthResponse>.Failure("Senhas não coincidem");

                // Verificar se usuário já existe
                var existingUser = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUser.IsSuccess)
                    return ServiceResult<AuthResponse>.Failure("Email já cadastrado");

                // Criar novo usuário
                var user = new User
                {
                    Email = request.Email,
                    PasswordHash = HashPassword(request.Password)
                };

                var createResult = await _userRepository.CreateAsync(user);
                if (!createResult.IsSuccess)
                    return ServiceResult<AuthResponse>.Failure(createResult.Error);

                // Gerar token
                var token = GenerateJwtToken(user);
                var authResponse = new AuthResponse
                {
                    Token = token,
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };

                return ServiceResult<AuthResponse>.Success(authResponse);
            }
            catch (Exception ex)
            {
                return ServiceResult<AuthResponse>.Failure($"Erro ao registrar usuário: {ex.Message}");
            }
        }

        public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                // Buscar usuário por email
                var userResult = await _userRepository.GetByEmailAsync(request.Email);
                if (!userResult.IsSuccess)
                    return ServiceResult<AuthResponse>.Failure("Credenciais inválidas");

                var user = userResult.Data;

                // Verificar senha
                if (!VerifyPassword(request.Password, user.PasswordHash))
                    return ServiceResult<AuthResponse>.Failure("Credenciais inválidas");

                // Gerar token
                var token = GenerateJwtToken(user);
                var authResponse = new AuthResponse
                {
                    Token = token,
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };

                return ServiceResult<AuthResponse>.Success(authResponse);
            }
            catch (Exception ex)
            {
                return ServiceResult<AuthResponse>.Failure($"Erro ao fazer login: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JWT:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return ServiceResult<bool>.Success(true);
            }
            catch
            {
                return ServiceResult<bool>.Success(false);
            }
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password + "salt")));
        }

        private bool VerifyPassword(string password, string hash)
        {
            var computedHash = HashPassword(password);
            return computedHash == hash;
        }
    }
}
