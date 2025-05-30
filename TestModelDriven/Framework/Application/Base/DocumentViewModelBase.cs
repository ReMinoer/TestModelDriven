using System;
using TestModelDriven.Framework.UndoRedo;

namespace TestModelDriven.Framework.Application.Base;

public abstract class DocumentViewModelBase<TModel> : OneForOneViewModelBase<TModel>, IDocumentViewModel
    where TModel : IDocument
{
    new public TModel Model { get; }
    IDocument IDocumentViewModel.Model => Model;

    public DirtyUndoRedoStackViewModel UndoRedoStack { get; }
    public UndoRedoRecorder UndoRedoRecorder { get; }
    
    private string _header = string.Empty;
    public string Header
    {
        get => _header;
        protected set => Set(ref _header, value);
    }

    public DocumentViewModelBase(TModel model)
        : base(model)
    {
        Model = model;

        UndoRedoStack = new DirtyUndoRedoStackViewModel();
        UndoRedoStack.IsDirtyChanged += OnIsDirtyChanged;

        UndoRedoRecorder = new UndoRedoRecorder(UndoRedoStack)
        {
            Presenter = this
        };

        UndoRedoRecorder.Subscribe(Model);
    }

    protected override void OnModelPropertyChanged(string? propertyName)
    {
        if (propertyName == nameof(IDocument.Header))
            Header = Model.Header;
    }

    protected abstract void OnIsDirtyChanged(object? sender, EventArgs e);
    public abstract void Present(PresenterSubject subject);
}