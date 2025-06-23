using System;

namespace TestModelDriven.Framework;

public interface IViewModel : IAsyncInitializable, IAsyncDisposable
{
    bool IsDisposed { get; }
}