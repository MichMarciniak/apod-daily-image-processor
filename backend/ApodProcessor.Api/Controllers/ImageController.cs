using System.ComponentModel;
using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    private readonly ImageService _service;

    public ImageController(ImageService service)
    {
        _service = service;
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<ImageThumbnailResponse>>> GetThumbnails()
    {
        var res = await _service.GetAllThumbnails();

        return res;
    }

    [HttpGet]
    public async Task<ActionResult<ImageResponse>> GetImageByDate([FromQuery, DefaultValue(null)] DateTime? date = null)
    {
        var targetDate = date ?? DateTime.UtcNow.Date;

        var result = await _service.GetImageByDate(targetDate);

        if (result == null) return NotFound("No image with this date");

        return Ok(result);
    }
    
}