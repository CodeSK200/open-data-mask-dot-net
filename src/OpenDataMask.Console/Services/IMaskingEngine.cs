using OpenDataMask.Console.Models;

namespace OpenDataMask.Console.Services
{
    public interface IMaskingEngine
    {
        Task ExecuteAsync(MaskingConfig config, CancellationToken cancellationToken);
    }
}
