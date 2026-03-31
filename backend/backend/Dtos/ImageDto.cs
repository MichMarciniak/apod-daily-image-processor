using backend.Models;

namespace backend.Dtos;

public record ImageResponse(
    string Title,
    string Explanation,
    DateTime Date,
    string? Copyright,
    List<ConceptResponse> Concepts
);

public record ImageSaveRequest(
    string Title,
    string Explanation,
    DateTime Date,
    string? Copyright,
    List<ConceptResponse> Concepts,
    string HdPath,
    string ThumbnailPath
);