using backend.Configuration;
using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace backend.Services.Background;

public class ImageProcessingWorker : BackgroundService
{
    private readonly ImageProcessingQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ImageProcessingWorker> _logger;
    private readonly ApiConfig _config;

    public ImageProcessingWorker(ImageProcessingQueue queue, IServiceScopeFactory scopeFactory,
        ILogger<ImageProcessingWorker> logger, IOptions<ApiConfig> config)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _config = config.Value;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var imageId in _queue.DequeueAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var image = await db.Images.FirstOrDefaultAsync(x => x.Id == imageId, stoppingToken);

                if (image != null && !string.IsNullOrEmpty(image.HdPath))
                {
                    _logger.LogInformation($"Processing image {image.Title}...");

                    var thumbFileName = Path.GetFileName(image.HdPath).Replace("_hd", "_thumb");
                    var thumbPath = Path.Combine(_config.ImageDir, thumbFileName);

                    using (var imageTool = await SixLabors.ImageSharp.Image.LoadAsync(image.HdPath, stoppingToken))
                    {
                        imageTool.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(300, 0),
                            Mode = ResizeMode.Max
                        }));

                        await imageTool.SaveAsync(thumbPath, stoppingToken);
                    }

                    image.ThumbnailPath = thumbPath;
                    image.Status = Status.Completed;

                    await db.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation($"Successfully processed thumbnail for {image.Title}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process image {imageId}");
            }
        } 
        
    }
}