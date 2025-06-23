using System;
using System.Threading.Tasks;

namespace TestModelDriven.Framework.UndoRedo;

public class UndoRedo : IUndoRedo
{
    private readonly Func<Task> _redo;
    private readonly Func<Task> _undo;
    private readonly Func<Task>? _doneDispose;
    private readonly Func<Task>? _undoneDispose;

    public string Description { get; }
    public bool IsDone { get; private set; }

    public UndoRedo(string description, Func<Task> redo, Func<Task> undo, Func<Task>? doneDispose = null, Func<Task>? undoneDispose = null)
    {
        Description = description;
        _redo = redo;
        _undo = undo;
        _doneDispose = doneDispose;
        _undoneDispose = undoneDispose;
    }

    public virtual async Task RedoAsync()
    {
        await _redo().ConfigureAwait(false);
        IsDone = true;
    }

    public virtual async Task UndoAsync()
    {
        await _undo().ConfigureAwait(false);
        IsDone = false;
    }

    public virtual async ValueTask DisposeAsync()
    {
        if (IsDone && _doneDispose is not null)
            await _doneDispose.Invoke().ConfigureAwait(false);
        else if (!IsDone && _undoneDispose is not null)
            await _undoneDispose.Invoke().ConfigureAwait(false);
    }
}