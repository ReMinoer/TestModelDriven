using System.Collections.Generic;

namespace TestModelDriven.Framework.Application;

public interface IFileDocumentType
{
    string DisplayName { get; }
    IReadOnlyList<IFileType> FileTypes { get; }
    IDocument NewDocument();
    IDocument? OpenDocument(string filePath);
    bool CanSaveDocument(IFileDocument fileDocument);
    void SaveDocument(IFileDocument fileDocument);
}