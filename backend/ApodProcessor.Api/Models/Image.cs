namespace backend.Models;

public class Image
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;

    public string HdPath { get; set; } = string.Empty;
    public string? ThumbnailPath { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public string? Copyright { get; set; }


    public Status Status { get; set; }
}

public enum Status
{
    Pending,
    Processing,
    Completed,
    Failed
}

