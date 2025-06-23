using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TestModelDriven.Framework.Application.Base;

public abstract class FileApplicationBase : Application
{
    public abstract IReadOnlyList<IFileDocumentType> FileDocumentTypes { get; }

    public async Task NewDocumentAsync(IFileDocumentType fileDocumentType, CancellationToken cancellationToken)
    {
        IDocument document = await fileDocumentType.NewDocumentAsync(cancellationToken);
        await AddDocumentAsync(document);
    }

    public async Task OpenFileAsync(string filePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (IFileDocumentType documentType in FileDocumentTypes)
        {
            IDocument? document = await documentType.OpenDocumentAsync(filePath, cancellationToken);
            if (document is null)
                continue;

            await AddDocumentAsync(document);
            return;
        }
    }

    public async Task SaveDocumentAsync(IFileDocument fileDocument, CancellationToken cancellationToken)
    {
        await fileDocument.FileDocumentType.SaveDocumentAsync(fileDocument, cancellationToken);
    }
}