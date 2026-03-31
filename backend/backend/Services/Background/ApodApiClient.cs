using System.ComponentModel;
using backend.Configuration;
using backend.Dtos;
using Microsoft.Extensions.Options;

namespace backend.Services.Background;

public class ApodApiClient : BackgroundService 
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<ApodApiClient> _logger;
    private readonly SecretsConfig _config;

    public ApodApiClient(IHttpClientFactory clientFactory, ILogger<ApodApiClient> logger, IOptions<SecretsConfig> config)
    {
        _clientFactory = clientFactory;
        _logger = logger;
        _config = config.Value;
    }

    private async Task FetchAndProcess(CancellationToken ct)
    {
        var client = _clientFactory.CreateClient("NasaClient");
        
        var response = await client.GetFromJsonAsync<ApodApiResponse>($"?api_key={_config.ApiKey}", ct);

        if (response != null)
        {
            _logger.LogInformation($"Got APOD: {response.Title}");
            _logger.LogInformation($"Full Response: {response}");
            
            // send it to service
            
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromDays(1));
        
        _logger.LogInformation("Fetching Data");

        do
        {
            try
            {
                await FetchAndProcess(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching data from APOD.");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
        
    }
}