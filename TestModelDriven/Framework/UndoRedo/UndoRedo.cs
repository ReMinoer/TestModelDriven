using System;

namespace TestModelDriven.Framework.UndoRedo;

public class UndoRedo : IUndoRedo
{
    private readonly Action _redo;
    private readonly Action _undo;
    private readonly Action? _doneDispose;
    private readonly Action? _undoneDispose;

    public string Description { get; }
    public bool IsDone { get; private set; }

    public UndoRedo(string description, Action redo, Action undo, Action? doneDispose = null, Action? undoneDispose = null)
    {
        Description = description;
        _redo = redo;
        _undo = undo;
        _doneDispose = doneDispose;
        _undoneDispose = undoneDispose;
    }

    public virtual void Redo()
    {
        _redo();
        IsDone = true;
    }

    public virtual void Undo()
    {
        _undo();
        IsDone = false;
    }

    public virtual void Dispose()
    {
        if (IsDone)
            _doneDispose?.Invoke();
        else
            _undoneDispose?.Invoke();
    }
}