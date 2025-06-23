using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public interface ISaveFormat
{
    Task SaveAsync(object data, Stream stream, CancellationToken cancellationToken);
}