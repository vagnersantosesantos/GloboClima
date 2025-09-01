using Amazon;
using Amazon.DynamoDBv2;
using GloboClima.Core.Interfaces.Repositories;
using GloboClima.Core.Interfaces.Services;
using GloboClima.Infrastructure.Repositories;
using GloboClima.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
        // Controllers
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        // DynamoDB
        services.AddSingleton<IAmazonDynamoDB>(provider =>
        {
            var config = new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(
                    Environment.GetEnvironmentVariable("AWS_REGION") ?? "us-east-1")
            };
            return new AmazonDynamoDBClient(config);
        });

        // JWT Authentication
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

        // HTTP Clients for External APIs
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

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        // Custom Services
        services.AddScoped<IUserRepository, DynamoDBUserRepository>();
        services.AddScoped<IWeatherService, WeatherService>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IAuthService, AuthService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}