using backend.Configuration;
using backend.Dtos;
using backend.Utils;
using Microsoft.Extensions.Options;

namespace backend.Services.Background;

public class ApodApiClient : BackgroundService 
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<ApodApiClient> _logger;
    private readonly SecretsConfig _secrets;
    private readonly ApiConfig _apiConfig;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ImageProcessingQueue _queue;

    public ApodApiClient(IHttpClientFactory clientFactory,
        ILogger<ApodApiClient> logger,
        IOptions<SecretsConfig> secrets,
        IOptions<ApiConfig> apiConfig,
        IServiceScopeFactory scopeFactory,
        ImageProcessingQueue queue)
    {
        _clientFactory = clientFactory;
        _logger = logger;
        _secrets = secrets.Value;
        _apiConfig = apiConfig.Value;
        _scopeFactory = scopeFactory;
        _queue = queue;
    }

    private async Task FetchAndProcess(CancellationToken ct)
    {
        var client = _clientFactory.CreateClient(_apiConfig.ClientName);

        var url = $"{_apiConfig.BaseApi}?api_key={_secrets.ApiKey}";
        var response = await client.GetFromJsonAsync<ApodApiResponse>(
            url,ct);

        if (response != null)
        {
            _logger.LogInformation($"Got APOD: {response.Title}");
            using var imageStream = await client.GetStreamAsync(response.Url, ct);
            
            var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ImageService>();

            var imageId = await service.SaveImageFromApi(response, imageStream, ct);

            _logger.LogInformation($"Queueing image: {imageId} to processing...");
            await _queue.EnqueueAsync(imageId);
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