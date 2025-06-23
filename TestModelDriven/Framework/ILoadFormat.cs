using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public interface ILoadFormat
{
    Task<object?> LoadAsync(Stream stream, CancellationToken cancellationToken);
}