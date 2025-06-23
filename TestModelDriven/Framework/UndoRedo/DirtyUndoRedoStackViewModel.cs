namespace TestModelDriven.Framework.UndoRedo;

public class DirtyUndoRedoStackViewModel : UndoRedoStackViewModel
{
    new public DirtyUndoRedoStack Model { get; }

    public DirtyUndoRedoStackViewModel(DirtyUndoRedoStack model)
        : base(model)
    {
        Model = model;
    }
}