using backend.Models;

namespace backend.Dtos;

public record ImageResponse(
    string Title,
    string Explanation,
    DateTime Date,
    string? Copyright,
    List<ConceptResponse> Concepts
);

