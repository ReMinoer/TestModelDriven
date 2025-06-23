using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestModelDriven.Framework.Application.Base;

public abstract class ApplicationViewModelBase<TApplication> : OneForOneViewModelBase<TApplication>, IViewModelFactory<IDocumentViewModel?>, IApplicationViewModel, IPresenter
    where TApplication : Application
{
    private IDocumentViewModel? _selectedDocument;

    public ViewModelCollection<IDocument, IDocumentViewModel> Documents { get; }
    public IDocumentViewModel? SelectedDocument
    {
        get => _selectedDocument;
        set => PushPropertyTwoWay(
            $"Focus document {value}",
            () => Model.SelectedDocument,
            value?.Model,
            () => Model.SetSelectedDocumentAsync(value?.Model),
            () => _selectedDocument?.Model,
            async x => _selectedDocument = x is not null ? await CreateDocumentViewModelAsync(x) : null);
    }

    public ObservableCollection<MenuItemViewModel> MenuItems { get; }
    public ICommand CloseCommand { get; }

    protected ApplicationViewModelBase(TApplication application)
        : base(application)
    {
        Documents = new ViewModelCollection<IDocument, IDocumentViewModel>(application.Documents, CreateDocumentViewModelAsync, x => x.Model);
        MenuItems = new ObservableCollection<MenuItemViewModel>();

        CloseCommand = new CommandDispatcherCommand<IDocumentViewModel>("Close document", CloseAsync);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await Documents.InitializeAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        Documents.Dispose();
        await base.DisposeAsync();
    }

    private async Task CloseAsync(IDocumentViewModel document)
    {
        await Model.CloseDocumentAsync(document.Model);
    }

    private async Task<IDocumentViewModel> CreateDocumentViewModelAsync(IDocument document)
    {
        IDocumentViewModel? documentViewModel = await CreateViewModelAsync(document);
        if (documentViewModel is null)
            return new UnsupportedDocumentViewModel(document);
        if (!documentViewModel.IsInitialized)
            await documentViewModel.InitializeAsync();

        if (document is IFileDocument { FilePath: null })
            await documentViewModel.UndoRedoStack.Model.ForceDirtyAsync();

        return documentViewModel;
    }

    protected override async Task OnModelPropertyChangedAsync(string propertyName)
    {
        if (propertyName == nameof(Application.SelectedDocument))
            await SetAsync(ref _selectedDocument, Documents.GetViewModel(Model.SelectedDocument), nameof(SelectedDocument));
    }

    public abstract Task<IDocumentViewModel?> CreateViewModelAsync(object model);
    public abstract Task<bool> PresentAsync(PresenterSubject subject);
}