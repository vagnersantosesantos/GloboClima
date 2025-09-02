using GloboClima.Services.Services;
using Moq;
using Moq.Protected;
using System.Net;
using Xunit;

public class CountryServiceTests
{
    private CountryService CreateService(string responseContent, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(responseContent)
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://fakeapi.com/")
        };

        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(_ => _.CreateClient("RestCountries")).Returns(httpClient);

        return new CountryService(factoryMock.Object);
    }

    [Fact]
    public async Task GetCountryAsync_ReturnsSuccess_WhenCountryFound()
    {
        // Arrange (JSON simplificado simulando restcountries)
        var json = @"[
            {
                ""name"": { ""common"": ""Brazil"", ""official"": ""Federative Republic of Brazil"" },
                ""cca2"": ""BR"",
                ""cca3"": ""BRA"",
                ""cioc"": ""BRA"",
                ""flag"": ""🇧🇷"",
                ""flags"": { ""png"": ""https://flagcdn.com/br.png"", ""svg"": ""https://flagcdn.com/br.svg"" },
                ""capital"": [""Brasília""],
                ""region"": ""Americas"",
                ""area"": 8515767,
                ""population"": 212559417,
                ""independent"": true,
                ""status"": ""officially-assigned"",
                ""unMember"": true
            }
        ]";

        var service = CreateService(json);

        // Act
        var result = await service.GetCountryAsync("Brazil");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Brazil", result.Data.Name);
        Assert.Equal("Federative Republic of Brazil", result.Data.OfficialName);
        Assert.Equal("BRA", result.Data.Cca3);
    }

    [Fact]
    public async Task GetCountryAsync_ReturnsFailure_WhenNotFound()
    {
        // Arrange
        var service = CreateService("", HttpStatusCode.NotFound);

        // Act
        var result = await service.GetCountryAsync("Atlantis");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Erro ao consultar país", result.Error);
    }

    [Fact]
    public async Task GetAllCountriesAsync_ReturnsList_WhenSuccess()
    {
        // Arrange
        var json = @"[
            {
                ""name"": { ""common"": ""Brazil"" },
                ""capital"": [""Brasília""],
                ""population"": 212559417,
                ""region"": ""Americas"",
                ""languages"": { ""por"": ""Portuguese"" },
                ""currencies"": { ""BRL"": { ""name"": ""Real"", ""symbol"": ""R$"" } },
                ""flags"": { ""png"": ""https://flagcdn.com/br.png"" },
                ""area"": 8515767
            }
        ]";

        var service = CreateService(json);

        // Act
        var result = await service.GetAllCountriesAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
        Assert.Equal("Brazil", result.Data[0].Name);
        Assert.Equal("Brasília", result.Data[0].Capital);
    }
}
