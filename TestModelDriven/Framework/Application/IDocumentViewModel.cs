using TestModelDriven.Framework.UndoRedo;

namespace TestModelDriven.Framework.Application;

public interface IDocumentViewModel : IOneForOneViewModel, IPresenter
{
    new IDocument Model { get; }
    DirtyUndoRedoStackViewModel UndoRedoStack { get; }
    string Header { get; }
}