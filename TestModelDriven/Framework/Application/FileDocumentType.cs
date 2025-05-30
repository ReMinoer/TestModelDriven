using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace TestModelDriven.Framework.Application;

public class FileDocumentType<TData, TModel> : IFileDocumentType
    where TData : IData<TModel>
    where TModel : class, IDocument, new()
{
    public string DisplayName { get; }
    public IReadOnlyList<IFileType> FileTypes { get; }

    public FileDocumentType(string displayName, params IFileType[] fileTypes)
    {
        DisplayName = displayName;
        FileTypes = new ReadOnlyCollection<IFileType>(fileTypes);
    }

    public IDocument NewDocument() => new TModel();
    public IDocument? OpenDocument(string filePath)
    {
        foreach (IFileType fileType in GetMatchingFileTypes(filePath))
        {
            object? load = fileType.Load(filePath);
            if (load is not TData data)
                continue;

            IDocument model = data.ToModel();
            if (model is IFileDocument fileDocument)
            {
                fileDocument.FilePath = filePath;
            }

            return model;
        }

        return null;
    }

    public bool CanSaveDocument(IFileDocument fileDocument)
    {
        return GetMatchingFileTypes(fileDocument.FilePath).Any(x => x.CanSave);
    }

    public void SaveDocument(IFileDocument fileDocument)
    {
        GetMatchingFileTypes(fileDocument.FilePath).FirstOrDefault(x => x.CanSave)?.Save(fileDocument.ToData(), fileDocument.FilePath!);
    }

    private IEnumerable<IFileType> GetMatchingFileTypes(string? filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return FileTypes;

        string fileExtension = Path.GetExtension(filePath)[1..];
        return FileTypes.Where(fileType => fileType.SupportExtension(fileExtension));
    }
}