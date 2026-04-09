namespace backend.Models;

public class Image
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Explanation { get; set; }

    public string HdPath { get; set; }
    public string? ThumbnailPath { get; set; }

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

