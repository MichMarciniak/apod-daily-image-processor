namespace backend.Dtos;

public record ApodApiResponse(
    DateTime Date,
    string Title,
    string Url,
    string Explanation,
    string? Copyright
);
