namespace TestModelDriven.Framework.Application.Base;

public abstract class FileDocumentBase<TData> : PersistentModelBase<TData>, IFileDocument<TData>
    where TData : IData
{
    private string _header = string.Empty;
    private string? _filePath;

    [State]
    public string Header
    {
        get => _header;
        protected set => Set(ref _header, value);
    }

    [State]
    public string? FilePath
    {
        get => _filePath;
        set => Set(ref _filePath, value);
    }

    public IFileDocumentType FileDocumentType { get; }

    protected FileDocumentBase(IFileDocumentType fileDocumentType)
    {
        FileDocumentType = fileDocumentType;
    }
}