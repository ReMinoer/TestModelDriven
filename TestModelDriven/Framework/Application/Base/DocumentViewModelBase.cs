using System.Threading.Tasks;
using TestModelDriven.Framework.UndoRedo;

namespace TestModelDriven.Framework.Application.Base;

public abstract class DocumentViewModelBase<TModel> : OneForOneViewModelBase<TModel>, IDocumentViewModel
    where TModel : IDocument
{
    new public TModel Model { get; }
    IDocument IDocumentViewModel.Model => Model;

    public DirtyUndoRedoStackViewModel UndoRedoStack { get; }
    public UndoRedoRecorder UndoRedoRecorder { get; }
    public CommandDispatcherViewModel CommandDispatcher { get; }

    private string _header = string.Empty;
    public string Header => _header;
    protected Task RefreshHeaderAsync(string value) => SetAsync(ref _header, value, nameof(Header));

    protected DocumentViewModelBase(TModel model)
        : base(model)
    {
        Model = model;

        var undoRedoStack = new DirtyUndoRedoStack();
        undoRedoStack.IsDirtyChangedAsync.Subscribe(OnIsDirtyChangedAsync);

        UndoRedoStack = new DirtyUndoRedoStackViewModel(undoRedoStack);
        UndoRedoRecorder = new UndoRedoRecorder(undoRedoStack)
        {
            Presenter = this
        };

        CommandDispatcher = new CommandDispatcherViewModel();

        UndoRedoRecorder.Subscribe(Model);
    }

    protected abstract Task OnIsDirtyChangedAsync();
    public abstract Task<bool> PresentAsync(PresenterSubject subject);
}