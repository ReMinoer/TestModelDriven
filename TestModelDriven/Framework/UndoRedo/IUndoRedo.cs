using System;
using System.Threading.Tasks;

namespace TestModelDriven.Framework.UndoRedo;

public interface IUndoRedo : IAsyncDisposable
{
    string Description { get; }
    bool IsDone { get; }
    Task RedoAsync();
    Task UndoAsync();
}