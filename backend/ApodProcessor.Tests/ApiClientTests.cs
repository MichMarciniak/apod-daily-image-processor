using System.Net;
using backend.Configuration;
using backend.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace ApodProcessor.Tests;

public class ApiClientTests
{
    private readonly IOptions<ApiConfig> _apiOptions;
    private readonly IOptions<SecretsConfig> _secretOptions;

    public ApiClientTests()
    {
         var apiConfig = new ApiConfig { BaseApi = "https://nasa.com" };
         var secretsConfig = new SecretsConfig { ApiKey = "some_key" };

         _apiOptions = Options.Create(apiConfig);
         _secretOptions = Options.Create(secretsConfig);
    }

    [Fact]
    public async Task GetDailyMetadataAsync_ShouldReturnData_WhenApiIsUp()
    {
        var expedtedTitle = "Star Wars";
        var handlerMock = new Mock<HttpMessageHandler>();

        // next time use mockhttp libarary, never write it again
        // its painful
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(GetStringContent(expedtedTitle))
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var client = new ApodApiClient(factoryMock.Object, _apiOptions, _secretOptions);

        var result = await client.GetDailyMetadataAsync(CancellationToken.None);

        result.Should().NotBeNull();
        result.Title.Should().Be(expedtedTitle);
    }

    private string GetStringContent(string title)
    {
        return $$"""
                 {
                     "title": "{{title}}",
                     "url": "http://nasa.com"
                 }
                 """;

    }

    
}