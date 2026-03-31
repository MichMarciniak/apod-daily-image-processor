using backend.Data;
using backend.Dtos;
using backend.Mappers;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ImageService
{
    private readonly HttpClient _httpClient;
    private readonly ConceptService _service;
    private readonly AppDbContext _context;

    public ImageService(IHttpClientFactory httpClientFactory, ConceptService service, AppDbContext context)
    {
        _httpClient = httpClientFactory.CreateClient("NasaClient");
        _service = service;
        _context = context;
    }


    public async Task<ImageResponse?> GetImageById(int id)
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

    
}