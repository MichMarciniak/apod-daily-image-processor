namespace backend.Dtos;

public record ApodApiResponse
{
    public DateTime Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public string? Copyright { get; set; }
}
