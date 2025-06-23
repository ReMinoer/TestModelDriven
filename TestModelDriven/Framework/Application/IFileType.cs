using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TestModelDriven.Framework.Application;

public interface IFileType
{
    string DisplayName { get; }
    IReadOnlyList<string> Extensions { get; }
    bool CanSave { get; }
    Task<object?> LoadAsync(string filePath, CancellationToken cancellationToken);
    Task SaveAsync(object data, string filePath, CancellationToken cancellationToken);
}