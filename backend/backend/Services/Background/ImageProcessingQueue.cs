using System.Threading.Channels;

namespace backend.Services.Background;

public class ImageProcessingQueue
{
    private readonly Channel<int> _queue;

    public ImageProcessingQueue()
    {
        _queue = Channel.CreateBounded<int>(5);
    }

    public async ValueTask EnqueueAsync(int imageId, CancellationToken ct = default)
    {
         await _queue.Writer.WriteAsync(imageId, ct);
    }

    public IAsyncEnumerable<int> DequeueAsync(CancellationToken ct = default)
    {
        return _queue.Reader.ReadAllAsync(ct);
    }
}