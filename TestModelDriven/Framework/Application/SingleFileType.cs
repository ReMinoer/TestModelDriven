using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestModelDriven.Framework.Application;

public class SingleFileType : IFileType
{
    private readonly ILoadFormat _loadFormat;
    private readonly ISaveFormat? _saveFormat;

    public string DisplayName { get; }
    public IReadOnlyList<string> Extensions { get; }
    public bool CanSave => _saveFormat is not null;

    public SingleFileType(string displayName, IEnumerable<string> extensions, ILoadFormat loadFormat)
    {
        DisplayName = displayName;
        Extensions = extensions.ToList().AsReadOnly();
        _loadFormat = loadFormat;
    }

    public SingleFileType(string displayName, IEnumerable<string> extensions, ISaveFormat saveFormat, ILoadFormat loadFormat)
        : this(displayName, extensions, loadFormat)
    {
        _saveFormat = saveFormat;
    }

    public SingleFileType(string displayName, IEnumerable<string> extensions, ISaveLoadFormat saveLoadFormat)
        : this(displayName, extensions, saveLoadFormat, saveLoadFormat)
    {
    }

    public async Task<object?> LoadAsync(string filePath, CancellationToken cancellationToken)
    {
        await using FileStream fileStream = File.OpenRead(filePath);
        return await _loadFormat.LoadAsync(fileStream, cancellationToken);
    }
        
    public async Task SaveAsync(object data, string filePath, CancellationToken cancellationToken)
    {
        if (_saveFormat is null)
            throw new InvalidOperationException("Save is not supported.");

        await using FileStream fileStream = File.OpenWrite(filePath);
        await _saveFormat.SaveAsync(data, fileStream, cancellationToken);
    }
}