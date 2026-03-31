using System.Threading.Channels;

namespace backend.Utils;

public class ImageProcessingQueue
{
    private readonly Channel<Guid> _queue;

    public ImageProcessingQueue()
    {
        _queue = Channel.CreateBounded<Guid>(5);
    }

    public async ValueTask EnqueueAsync(Guid imageId, CancellationToken ct = default)
    {
         await _queue.Writer.WriteAsync(imageId, ct);
    }

    public IAsyncEnumerable<Guid> DequeueAsync(CancellationToken ct = default)
    {
        return _queue.Reader.ReadAllAsync(ct);
    }
}