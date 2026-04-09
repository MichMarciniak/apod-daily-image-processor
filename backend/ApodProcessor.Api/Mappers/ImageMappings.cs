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
            image.HdPath
        );
    }

    public static ImageThumbnailResponse ToThumbDto(this Image image)
    {
        return new ImageThumbnailResponse(
            image.Title,
            image.Date,
            image.ThumbnailPath!
        );
    }
}