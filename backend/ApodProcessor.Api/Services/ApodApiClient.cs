using backend.Configuration;
using backend.Dtos;
using backend.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace backend.Services;

public class ApodApiClient : IApodApiClient
{
    private readonly IHttpClientFactory _factory;
    private readonly ApiConfig _apiConfig;
    private readonly SecretsConfig _secrets;

    public ApodApiClient(IHttpClientFactory factory, IOptions<ApiConfig> config, IOptions<SecretsConfig> secrets)
    {
        _factory = factory;
        _apiConfig = config.Value;
        _secrets = secrets.Value;
    }
    
    public async Task<ApodApiResponse?> GetDailyMetadataAsync(CancellationToken ct)
    {
        using var client = _factory.CreateClient(_apiConfig.ClientName);

        var url = $"{_apiConfig.BaseApi}?api_key={_secrets.ApiKey}";
        var response = await client.GetFromJsonAsync<ApodApiResponse>(
            url,ct);

        return response;
    }

    public async Task<Stream> GetImageStreamAsync(string url, CancellationToken ct)
    {
        using var client = _factory.CreateClient(_apiConfig.ClientName);
        return await client.GetStreamAsync(url, ct);
    }
}