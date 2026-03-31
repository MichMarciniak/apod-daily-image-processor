using backend.Configuration;
using backend.Data;
using backend.Dtos;
using backend.Mappers;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace backend.Services;

public class ImageService
{
    private readonly AppDbContext _context;
    private readonly ApiConfig _config;

    public ImageService(AppDbContext context, IOptions<ApiConfig> config)
    {
        _context = context;
        _config = config.Value;
    }

    public async Task<List<ImageThumbnailResponse>> GetAllThumbnails()
    {
        var res = await _context.Images.Select(x => x.ToThumbDto()).ToListAsync();

        return res;
    }

    public async Task<ImageResponse?> GetImageById(Guid id)
    {
        var res = await _context.Images
            .Where(x => x.Id == id)
            .Select(x => x.ToDto())
            .FirstOrDefaultAsync();

        return res;
    }

    public async Task<ImageResponse?> GetImageByDate(DateTime date)
    {
        var res = await _context.Images
            .Where(x => x.Date == date)
            .Select(x => x.ToDto())
            .FirstOrDefaultAsync();

        return res;
    }

    public async Task<Guid> SaveImageFromApi(ApodApiResponse response, Stream imageStream, CancellationToken ct)
    {
        var fileName = $"{response.Date:yyyy-MM-dd}.jpg";

        var folderPath = Path.Combine(_config.ImageDir, "images", "hd");
        
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        var fullPath = Path.Combine(folderPath, fileName);
        
        using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await imageStream.CopyToAsync(fileStream, ct);
        }

        var image = new Image
        {
            Title = response.Title,
            Explanation = response.Explanation,
            Date = response.Date,
            HdPath = $"/images/hd/{fileName}",
            Status = Status.Pending
        };

        _context.Images.Add(image);
        await _context.SaveChangesAsync(ct);

        return image.Id;
    }
    
}