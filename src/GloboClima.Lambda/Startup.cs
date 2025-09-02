using Amazon;
using Amazon.DynamoDBv2;
using GloboClima.Core.Interfaces.Repositories;
using GloboClima.Core.Interfaces.Services;
using GloboClima.Frontend.Services;
using GloboClima.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthService = GloboClima.Services.Services.AuthService;
using CountryService = GloboClima.Services.Services.CountryService;
using WeatherService = GloboClima.Services.Services.WeatherService;

using AuthServiceFront = GloboClima.Frontend.Services.AuthService;
using CountryServiceFront = GloboClima.Frontend.Services.CountryService;
using WeatherServiceFront = GloboClima.Frontend.Services.WeatherService;
using Microsoft.Extensions.Hosting;

namespace GloboClima.Lambda;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // ✅ API Controllers
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        // ✅ Blazor Server
        services.AddRazorPages();
        services.AddServerSideBlazor();

        // ✅ DynamoDB
        services.AddSingleton<IAmazonDynamoDB>(provider =>
        {
            var config = new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.USEast1
            };
            return new AmazonDynamoDBClient(config);
        });

        // ✅ JWT Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["JWT:Issuer"] ?? "GloboClima",
                    ValidAudience = Configuration["JWT:Audience"] ?? "GloboClima-Users",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["JWT:Secret"] ?? "default-secret-key"))
                };
            });

        // ✅ HTTP Clients for External APIs
        services.AddHttpClient("OpenWeatherMap", client =>
        {
            client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient("RestCountries", client =>
        {
            client.BaseAddress = new Uri("https://restcountries.com/v3.1/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // ✅ HTTP Client para Frontend consumir API (mesma instância)
        services.AddHttpClient("GloboclimaAPI", client =>
        {
            client.BaseAddress = new Uri("http://localhost/"); // API na mesma instância
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // ✅ CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        // ✅ Custom Services (Backend)
        services.AddScoped<IUserRepository, DynamoDBUserRepository>();
        services.AddScoped<IWeatherService, WeatherService>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IAuthService, AuthService>();

        // ✅ Frontend Services
        services.AddScoped<ApiService>();
        services.AddScoped<AuthServiceFront>();
        services.AddScoped<WeatherServiceFront>();
        services.AddScoped<CountryServiceFront>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            // ✅ API Routes
            endpoints.MapControllers();

            // ✅ Blazor Routes
            endpoints.MapRazorPages();
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        });
    }
}