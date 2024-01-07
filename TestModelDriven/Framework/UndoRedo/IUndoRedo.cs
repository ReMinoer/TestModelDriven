using System;

namespace TestModelDriven.Framework.UndoRedo;

public interface IUndoRedo : IDisposable
{
    string Description { get; }
    bool IsDone { get; }
    void Redo();
    void Undo();
}