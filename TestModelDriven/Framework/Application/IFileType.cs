using System.Collections.Generic;

namespace TestModelDriven.Framework.Application;

public interface IFileType
{
    string DisplayName { get; }
    IReadOnlyList<string> Extensions { get; }
    bool CanSave { get; }
    object? Load(string filePath);
    void Save(object data, string filePath);
}