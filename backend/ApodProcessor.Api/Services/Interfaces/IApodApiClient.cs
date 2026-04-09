using backend.Dtos;

namespace backend.Services.Interfaces;

public interface IApodApiClient
{
    public Task<ApodApiResponse?> GetDailyMetadataAsync(CancellationToken ct);

    public Task<Stream> GetImageStreamAsync(string url, CancellationToken ct);
}