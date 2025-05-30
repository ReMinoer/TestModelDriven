using System.Collections.Generic;

namespace TestModelDriven.Framework.Application.Base;

public abstract class FileApplicationBase : Application
{
    public abstract IReadOnlyList<IFileDocumentType> FileDocumentTypes { get; }

    public void NewDocument(IFileDocumentType fileDocumentType)
    {
        IDocument document = fileDocumentType.NewDocument();

        AddDocument(document);
    }

    public void OpenFile(string filePath)
    {
        foreach (IFileDocumentType documentType in FileDocumentTypes)
        {
            IDocument? document = documentType.OpenDocument(filePath);
            if (document is null)
                continue;

            AddDocument(document);
            return;
        }
    }

    public void SaveDocument(IFileDocument fileDocument)
    {
        fileDocument.FileDocumentType.SaveDocument(fileDocument);
    }
}