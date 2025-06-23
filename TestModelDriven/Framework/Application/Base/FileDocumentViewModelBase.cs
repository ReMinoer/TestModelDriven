using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace TestModelDriven.Framework.Application.Base;

public abstract class FileDocumentViewModelBase<TModel> : DocumentViewModelBase<TModel>
    where TModel : IFileDocument, INotifyPropertyChanged
{
    private string? _filePath;
    public string? FilePath => _filePath;
    protected async Task RefreshFilePathAsync() => await RefreshFilePathAsync(Model.FilePath);
    protected async Task RefreshFilePathAsync(string? value)
    {
        if (_filePath == value)
            return;

        await using (SetPropertyScope(ref _filePath, value))
        {
            _filePath = value;
            await RefreshFileNameAsync();
        }
    }

    private string? _fileName;
    public string? FileName => _fileName;
    private Task RefreshFileNameAsync() => RefreshFileNameAsync(Path.GetFileNameWithoutExtension(FilePath));
    private async Task RefreshFileNameAsync(string? value)
    {
        if (_fileName == value)
            return;

        await using (SetPropertyScope(ref _fileName, value))
        {
            _fileName = value;
            await RefreshHeaderAsync();
        }
    }

    private Task RefreshHeaderAsync() => RefreshHeaderAsync($"{FileName ?? "New"}{(UndoRedoStack.Model.IsDirty ? "*" : "")}");

    protected FileDocumentViewModelBase(TModel model)
        : base(model)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await RefreshFilePathAsync();
    }

    protected override async Task OnModelPropertyChangedAsync(string propertyName)
    {
        if (propertyName == nameof(IFileDocument.FilePath))
            await RefreshFilePathAsync();
    }

    protected override async Task OnIsDirtyChangedAsync()
    {
        await RefreshHeaderAsync();
    }
}