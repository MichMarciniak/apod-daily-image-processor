using backend.Dtos;
using backend.Models;

namespace backend.Mappers;

public static class ImageMappings
{
    public static ImageResponse ToDto(this Image image)
    {
        return new ImageResponse(
            image.Title,
            image.Explanation,
            image.Date,
            image.Copyright,
            image.Concepts.Select(x => x.ToDto()).ToList()
        );
    }
}