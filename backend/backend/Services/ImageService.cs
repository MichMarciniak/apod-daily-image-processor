using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ImageService
{
    private HttpClient _httpClient;
    private ConceptService _service;
    private AppDbContext _context;

    public ImageService(IHttpClientFactory httpClientFactory, ConceptService service, AppDbContext context)
    {
        _httpClient = httpClientFactory.CreateClient("NasaClient");
        _service = service;
        _context = context;
    }


    public async Task<Image> GetImageById(int id)
    {
        var res = await _context.Images.Where(x => x.Id == id).FirstOrDefaultAsync();

        return res;

    }

    public async Task<Image> GetImageByDate(DateTime date)
    {
        var res = await _context.Images.Where(x => x.Date == date).FirstOrDefaultAsync();

        return res;
    }

    
}