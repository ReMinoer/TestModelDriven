using System.Collections.Generic;

namespace TestModelDriven.Framework.UndoRedo;

public interface IUndoRedoBatch : IUndoRedoStack, IUndoRedo
{
    public IReadOnlyList<IUndoRedo> Batch { get; }
}