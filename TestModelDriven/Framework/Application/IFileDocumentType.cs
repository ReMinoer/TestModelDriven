using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TestModelDriven.Framework.Application;

public interface IFileDocumentType
{
    string DisplayName { get; }
    IReadOnlyList<IFileType> FileTypes { get; }
    Task<IDocument> NewDocumentAsync(CancellationToken cancellationToken);
    Task<IDocument?> OpenDocumentAsync(string filePath, CancellationToken cancellationToken);
    bool CanSaveDocument(IFileDocument fileDocument);
    Task SaveDocumentAsync(IFileDocument fileDocument, CancellationToken cancellationToken);
}