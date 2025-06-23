using System.Threading.Tasks;

namespace TestModelDriven.Framework.Application.Base;

public abstract class FileDocumentBase<TData> : PersistentModelBase<TData>, IFileDocument<TData>
    where TData : IData
{
    private string? _filePath;
    [State(nameof(SetFilePathAsync))]
    public string? FilePath => _filePath;
    public Task SetFilePathAsync(string? value) => SetAsync(ref _filePath, value, nameof(FilePath));

    public IFileDocumentType FileDocumentType { get; }

    protected FileDocumentBase(IFileDocumentType fileDocumentType)
    {
        FileDocumentType = fileDocumentType;
    }
}