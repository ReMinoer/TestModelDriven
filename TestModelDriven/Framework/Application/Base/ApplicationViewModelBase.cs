using System.Collections.ObjectModel;
using System.Windows.Input;

namespace TestModelDriven.Framework.Application.Base;

public abstract class ApplicationViewModelBase<TApplication> : OneForOneViewModelBase<TApplication>, IViewModelFactory<IDocumentViewModel>, IApplicationViewModel, IPresenter
    where TApplication : Application
{
    private IDocumentViewModel? _selectedDocument;

    public ViewModelCollection<IDocument, IDocumentViewModel> Documents { get; }
    public IDocumentViewModel? SelectedDocument
    {
        get => _selectedDocument;
        set => Model.SelectedDocument = value?.Model;
    }

    public ObservableCollection<MenuItemViewModel> MenuItems { get; }
    public ICommand CloseCommand { get; }

    protected ApplicationViewModelBase(TApplication application)
        : base(application)
    {
        Documents = new ViewModelCollection<IDocument, IDocumentViewModel>(application.Documents, CreateDocumentViewModel, x => x.Model);
        MenuItems = new ObservableCollection<MenuItemViewModel>();
        CloseCommand = new Command(Close);
    }

    private void Close(object? viewModel)
    {
        if (viewModel is not IDocumentViewModel document)
            return;

        Model.CloseDocument(document.Model);
    }

    private IDocumentViewModel CreateDocumentViewModel(IDocument document)
    {
        IDocumentViewModel? documentViewModel = CreateViewModel(document);
        if (documentViewModel is null)
            return new UnsupportedDocumentViewModel(document);

        if (document is IFileDocument { FilePath: null })
            documentViewModel.UndoRedoStack.ForceDirty();

        return documentViewModel;
    }

    protected override void OnModelPropertyChanged(string? propertyName)
    {
        if (propertyName == nameof(Application.SelectedDocument))
            Set(ref _selectedDocument, Model.SelectedDocument is not null ? Documents.GetViewModel(Model.SelectedDocument) : null, nameof(SelectedDocument));
    }

    public abstract IDocumentViewModel? CreateViewModel(object model);
    public abstract void Present(PresenterSubject subject);
}