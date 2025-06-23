using System;

namespace TestModelDriven.Framework;

public interface IAsyncEvent
{
    IDisposable Subscribe(AsyncEventHandler handler);
}

public interface IAsyncEvent<out T>
{
    IDisposable Subscribe(AsyncEventHandler<T> handler);
}