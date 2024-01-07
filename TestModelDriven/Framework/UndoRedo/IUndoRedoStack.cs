using System;

namespace TestModelDriven.Framework.UndoRedo;

public interface IUndoRedoStack : IDisposable
{
    void Push(IUndoRedo undoRedo);
}