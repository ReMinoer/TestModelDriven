using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestModelDriven.Framework.UndoRedo;

public class UndoRedoBatch : IUndoRedoBatch
{
    private readonly List<IUndoRedo> _batch;
    public IReadOnlyList<IUndoRedo> Batch { get; }

    private string? _description;
    public string Description
    {
        get => _description ?? (_batch.Count == 1 ? _batch[0].Description : $"[Batch of {_batch.Count} undo-redo]");
        set => _description = value;
    }

    public bool IsDone { get; private set; }

    public UndoRedoBatch(string? description = null)
    {
        _description = description;

        _batch = new List<IUndoRedo>();
        Batch = _batch.AsReadOnly();
    }

    public virtual Task PushAsync(IUndoRedo undoRedo)
    {
        _batch.Add(undoRedo);
        return Task.CompletedTask;
    }

    public virtual async Task RedoAsync()
    {
        foreach (IUndoRedo undoRedo in _batch)
            await undoRedo.RedoAsync().ConfigureAwait(false);

        IsDone = true;
    }

    public virtual async Task UndoAsync()
    {
        foreach (IUndoRedo undoRedo in _batch.Reverse<IUndoRedo>())
            await undoRedo.UndoAsync().ConfigureAwait(false);

        IsDone = false;
    }

    public virtual async ValueTask DisposeAsync()
    {
        foreach (IUndoRedo undoRedo in _batch)
            await undoRedo.DisposeAsync().ConfigureAwait(false);
    }
}