using backend.Models;

namespace backend.Dtos;

public record ImageResponse(
    string Title,
    string Explanation,
    DateTime Date,
    string? Copyright
);

public record ImageSaveRequest(
    string Title,
    string Explanation,
    DateTime Date,
    string? Copyright,
    string HdPath,
    string ThumbnailPath
);

public record ImageThumbnailResponse(
    
);