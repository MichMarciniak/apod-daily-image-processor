using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Background;

public class ImageProcessingWorker : BackgroundService
{
    private readonly ImageProcessingQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ImageProcessingWorker> _logger;

    public ImageProcessingWorker(ImageProcessingQueue queue, IServiceScopeFactory scopeFactory,
        ILogger<ImageProcessingWorker> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var imageId in _queue.DequeueAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var image = await db.Images.Where(x => x.Id == imageId).FirstOrDefaultAsync(stoppingToken);

                if (image != null)
                {
                    _logger.LogInformation($"Processing image {image.Title}...");


                    // get image with http client
                    // scale with image sharp
                    // save as file
                    // image status = completed
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process image {imageId}");
            }
        } 
        
    }
}