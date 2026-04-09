using backend.Configuration;
using backend.Dtos;
using backend.Services.Interfaces;
using backend.Utils;
using Microsoft.Extensions.Options;

namespace backend.Services.Background;

public class ApiTimeWorker : BackgroundService 
{
    private readonly ILogger<ApiTimeWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ImageProcessingQueue _queue;
    private readonly IApodApiClient _apiClient;

    public ApiTimeWorker( 
        ILogger<ApiTimeWorker> logger,
        IServiceScopeFactory scopeFactory,
        ImageProcessingQueue queue,
        IApodApiClient apiClient)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _queue = queue;
        _apiClient = apiClient;
    }
    
    private async Task FetchAndProcess(CancellationToken ct)
    {
        var response = await _apiClient.GetDailyMetadataAsync(ct);
        if (response == null) return;
        
        _logger.LogInformation($"Got APOD: {response.Title}");
        using var imageStream = await _apiClient.GetImageStreamAsync(response.Url, ct);

        var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ImageService>();

        var imageId = await service.SaveImageFromApi(response, imageStream, ct);
        if (imageId == null)
        {
            _logger.LogWarning($"Image already exists. Skipping.");
            return;
        }

        _logger.LogInformation($"Queueing image: {imageId} to processing...");
        await _queue.EnqueueAsync((Guid)imageId, ct);
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