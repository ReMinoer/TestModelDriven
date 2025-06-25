using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public class AsyncEvent : IAsyncEvent
{
    private readonly List<AsyncEventHandler> _subscribers = new();
    public IAsyncEvent Public { get; }

    public AsyncEvent()
    {
        Public = new Subscribable(this);
    }

    public IDisposable Subscribe(AsyncEventHandler handler)
    {
        return new Subscription(this, handler);
    }

    public async Task RaiseAsync()
    {
        foreach (AsyncEventHandler subscriber in _subscribers)
            await subscriber();
    }

    private class Subscription : IDisposable
    {
        private readonly AsyncEvent _owner;
        private readonly AsyncEventHandler _handlers;

        public Subscription(AsyncEvent owner, AsyncEventHandler handlers)
        {
            _owner = owner;
            _handlers = handlers;

            _owner._subscribers.Add(_handlers);
        }

        public void Dispose()
        {
            _owner._subscribers.Remove(_handlers);
        }
    }

    private class Subscribable : IAsyncEvent
    {
        private readonly AsyncEvent _asyncEvent;

        public Subscribable(AsyncEvent asyncEvent)
        {
            _asyncEvent = asyncEvent;
        }

        public IDisposable Subscribe(AsyncEventHandler handler) => _asyncEvent.Subscribe(handler);
    }
}

public class AsyncEvent<T> : IAsyncEvent<T>
{
    private readonly List<AsyncEventHandler<T>> _subscribers = new();
    public IAsyncEvent<T> Public { get; }

    public AsyncEvent()
    {
        Public = new Subscribable(this);
    }

    public IDisposable Subscribe(AsyncEventHandler<T> handler)
    {
        return new Subscription(this, handler);
    }

    public async Task RaiseAsync(T change)
    {
        foreach (AsyncEventHandler<T> subscriber in _subscribers.ToArray())
            await subscriber(change);
    }

    private class Subscription : IDisposable
    {
        private readonly AsyncEvent<T> _owner;
        private readonly AsyncEventHandler<T> _handlers;

        public Subscription(AsyncEvent<T> owner, AsyncEventHandler<T> handlers)
        {
            _owner = owner;
            _handlers = handlers;

            _owner._subscribers.Add(_handlers);
        }

        public void Dispose()
        {
            _owner._subscribers.Remove(_handlers);
        }
    }

    private class Subscribable : IAsyncEvent<T>
    {
        private readonly AsyncEvent<T> _asyncEvent;

        public Subscribable(AsyncEvent<T> asyncEvent)
        {
            _asyncEvent = asyncEvent;
        }

        public IDisposable Subscribe(AsyncEventHandler<T> handler) => _asyncEvent.Subscribe(handler);
    }
}