using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

    public Task<IDocument> NewDocumentAsync(CancellationToken cancellationToken) => Task.FromResult<IDocument>(new TModel());
    public async Task<IDocument?> OpenDocumentAsync(string filePath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (IFileType fileType in GetMatchingFileTypes(filePath))
        {
            object? load = await fileType.LoadAsync(filePath, cancellationToken);
            if (load is not TData data)
                continue;

            IDocument model = await data.ToModelAsync();
            if (model is IFileDocument fileDocument)
            {
                await fileDocument.SetFilePathAsync(filePath);
            }

            return model;
        }

        return null;
    }

    public bool CanSaveDocument(IFileDocument fileDocument)
    {
        return GetMatchingFileTypes(fileDocument.FilePath).Any(x => x.CanSave);
    }

    public async Task SaveDocumentAsync(IFileDocument fileDocument, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IFileType? matchingFileType = GetMatchingFileTypes(fileDocument.FilePath).FirstOrDefault(x => x.CanSave);
        if (matchingFileType is null)
            return;

        await matchingFileType.SaveAsync(fileDocument.ToData(), fileDocument.FilePath!, cancellationToken);
    }

    private IEnumerable<IFileType> GetMatchingFileTypes(string? filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return FileTypes;

        string fileExtension = Path.GetExtension(filePath)[1..];
        return FileTypes.Where(fileType => fileType.SupportExtension(fileExtension));
    }
}