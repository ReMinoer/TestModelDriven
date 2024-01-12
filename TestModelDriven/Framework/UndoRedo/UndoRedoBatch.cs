using System.Collections.Generic;
using System.Linq;

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

    public virtual void Push(IUndoRedo undoRedo)
    {
        _batch.Add(undoRedo);
    }

    public virtual void Redo()
    {
        foreach (IUndoRedo undoRedo in _batch)
            undoRedo.Redo();

        IsDone = true;
    }

    public virtual void Undo()
    {
        foreach (IUndoRedo undoRedo in _batch.Reverse<IUndoRedo>())
            undoRedo.Undo();

        IsDone = false;
    }

    public virtual void Dispose()
    {
        foreach (IUndoRedo undoRedo in _batch)
            undoRedo.Dispose();
    }
}