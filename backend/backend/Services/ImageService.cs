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
        var id = Guid.NewGuid();
        var fileName = $"{id}_hd.jpg";
        var fullPath = Path.Combine(_config.ImageDir, fileName);

        using (var fileStream = new FileStream(fullPath, FileMode.Create))
        {
            await imageStream.CopyToAsync(fileStream, ct);
        }

        var image = new Image
        {
            Id = id,
            Title = response.Title,
            Explanation = response.Explanation,
            HdPath = fullPath,
            Status = Status.Pending
        };

        _context.Images.Add(image);
        await _context.SaveChangesAsync(ct);

        return id;
    }
    
}