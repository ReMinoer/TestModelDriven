using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

    public object? Load(string filePath)
    {
        using FileStream fileStream = File.OpenRead(filePath);
        return _loadFormat.Load(fileStream);
    }

    public void Save(object data, string filePath)
    {
        if (_saveFormat is null)
            throw new InvalidOperationException("Save is not supported.");

        using FileStream fileStream = File.OpenWrite(filePath);
        _saveFormat.Save(data, fileStream);
    }
}