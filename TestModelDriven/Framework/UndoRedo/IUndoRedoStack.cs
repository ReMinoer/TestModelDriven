using System;
using System.Threading.Tasks;

namespace TestModelDriven.Framework.UndoRedo;

public interface IUndoRedoStack : IAsyncDisposable
{
    Task PushAsync(IUndoRedo undoRedo);
}