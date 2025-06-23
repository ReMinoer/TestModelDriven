using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TestModelDriven.Framework.UndoRedo;

public class UndoRedoRecorder : IDisposable
{
    static private string? _batchDescription;
    static private bool _needNewBatch;
    static private bool _isObservingStateChange;

    private readonly IUndoRedoStack _undoRedoStack;
    private UndoRedoRecorderBatch _currentBatch = new();

    private readonly Subject<UndoRedoRecorderBatch?> _batchSubject;
    private readonly IDisposable _batchSubscription;

    private readonly Dictionary<IStateChangeNotifier, IDisposable> _stateSubscriptions = new();
    private readonly Dictionary<ICollectionChangeNotifier, IDisposable> _collectionSubscriptions = new();
    private readonly Dictionary<IAsyncList, StateCollectionAttribute> _collectionAttributes = new();

    private IUndoRedoStack ActiveStack
    {
        get
        {
            _batchSubject.OnNext(_currentBatch);
            return _currentBatch;
        }
    }

    public IPresenter? Presenter { get; set; }

    public UndoRedoRecorder(IUndoRedoStack undoRedoStack)
    {
        _undoRedoStack = undoRedoStack;

        _batchSubject = new Subject<UndoRedoRecorderBatch?>();
        _batchSubscription = _batchSubject
            .Throttle(TimeSpan.FromMilliseconds(500))
            .ObserveOn(SynchronizationContext.Current!)
            .Subscribe(OnBatchFree);
    }

    public void Dispose()
    {
        _batchSubscription.Dispose();
        _batchSubject.Dispose();

        // TODO: Dispose current batch properly
    }

    static public void Batch(string description)
    {
        _batchDescription = description;
        _needNewBatch = true;
    }

    private void OnBatchFree(UndoRedoBatch? batch) => OnBatchFreeAsync(batch).CaptureThrow();
    private async Task OnBatchFreeAsync(UndoRedoBatch? batch)
    {
        if (batch is null)
            return;

        if (batch.Batch.Count == 1)
        {
            IUndoRedo uniqueUndoRedo = batch.Batch[0];
            if (_batchDescription is not null)
                uniqueUndoRedo = uniqueUndoRedo.PostDescription(_batchDescription);

            await _undoRedoStack.PushAsync(uniqueUndoRedo);
        }
        else
        {
            await _undoRedoStack.PushAsync(batch);
        }

        _currentBatch = new UndoRedoRecorderBatch();
        _batchDescription = null;
    }

    private class UndoRedoRecorderBatch : UndoRedoBatch
    {
        private UndoRedoBatch? _currentSubBatch;

        public override async Task PushAsync(IUndoRedo undoRedo)
        {
            if (_currentSubBatch is null || _needNewBatch)
            {
                _currentSubBatch = new UndoRedoBatch(_batchDescription);
                await base.PushAsync(_currentSubBatch);

                _needNewBatch = false;
            }

            await _currentSubBatch.PushAsync(undoRedo);
        }
    }

    public void Subscribe(IStateChangeNotifier state)
    {
        _stateSubscriptions.Add(state, state.StateChangedAsync.Subscribe(OnChangeAsync));

        foreach (PropertyInfo statePropertyInfo in state.GetType().GetProperties())
        {
            var stateAttribute = statePropertyInfo.GetCustomAttribute<StateAttribute>();
            if (stateAttribute is not null)
            {
                object? value = statePropertyInfo.GetValue(state);
                if (value is IStateChangeNotifier subState)
                {
                    Subscribe(subState);
                }
            }

            var stateCollectionAttribute = statePropertyInfo.GetCustomAttribute<StateCollectionAttribute>();
            if (stateCollectionAttribute is not null)
            {
                object? value = statePropertyInfo.GetValue(state);

                if (value is ICollectionChangeNotifier collectionChangeNotifier and IAsyncList subStateList)
                {
                    _collectionAttributes.Add(subStateList, stateCollectionAttribute);

                    IDisposable subscription = collectionChangeNotifier.CollectionChangedAsync.Subscribe(OnChangeAsync);
                    _collectionSubscriptions.Add(collectionChangeNotifier, subscription);
                }

                if (value is IEnumerable subStateEnumerable)
                {
                    foreach (IStateChangeNotifier subStateItem in subStateEnumerable.OfType<IStateChangeNotifier>())
                    {
                        Subscribe(subStateItem);
                    }
                }
            }
        }
    }

    public void Unsubscribe(IStateChangeNotifier state)
    {
        foreach (PropertyInfo statePropertyInfo in state.GetType().GetProperties())
        {
            var stateAttribute = statePropertyInfo.GetCustomAttribute<StateAttribute>();
            if (stateAttribute is not null)
            {
                object? value = statePropertyInfo.GetValue(state);

                if (value is IStateChangeNotifier subState)
                {
                    Unsubscribe(subState);
                }
            }

            var stateCollectionAttribute = statePropertyInfo.GetCustomAttribute<StateCollectionAttribute>();
            if (stateCollectionAttribute is not null)
            {
                object? value = statePropertyInfo.GetValue(state);

                if (value is IEnumerable subStateEnumerable)
                {
                    foreach (IStateChangeNotifier subStateItem in subStateEnumerable.OfType<IStateChangeNotifier>())
                    {
                        Unsubscribe(subStateItem);
                    }
                }

                if (value is ICollectionChangeNotifier collectionChangeNotifier and IAsyncList subStateList)
                {
                    _collectionSubscriptions[collectionChangeNotifier].Dispose();
                    _collectionSubscriptions.Remove(collectionChangeNotifier);

                    _collectionAttributes.Remove(subStateList);
                }
            }
        }

        _stateSubscriptions[state].Dispose();
        _stateSubscriptions.Remove(state);
    }
    
    private async Task OnChangeAsync(StateChange change)
    {
        if (_isObservingStateChange)
            return;

        Type type = change.Owner.GetType();
        PropertyInfo? propertyInfo = type.GetProperty(change.PropertyName);
        if (propertyInfo is null)
            throw new InvalidOperationException($"Cannot resolve the property {change.PropertyName}.");

        var stateAttribute = propertyInfo.GetCustomAttribute<StateAttribute>();
        if (stateAttribute is null)
            throw new InvalidOperationException($"Cannot find a {nameof(StateAttribute)} on property {propertyInfo.Name}.");

        MethodInfo? asyncSetterInfo = type.GetMethod(stateAttribute.AsyncSetterName, new[] {propertyInfo.PropertyType});
        if (asyncSetterInfo is null)
            throw new InvalidOperationException($"Cannot resolve the async setter \"{stateAttribute.AsyncSetterName}\".");
        if (asyncSetterInfo.ReturnType != typeof(Task) && asyncSetterInfo.ReturnType != typeof(ValueTask))
            throw new InvalidOperationException($"Async setter \"{stateAttribute.AsyncSetterName}\" does not return a {nameof(Task)} nor a {nameof(ValueTask)}.");

        if (stateAttribute.IsOwner)
        {
            if (change.OldValue is IStateChangeNotifier oldState)
            {
                Unsubscribe(oldState);
            }

            if (change.NewValue is IStateChangeNotifier newState)
            {
                Subscribe(newState);
            }
        }

        await ActiveStack.PushAsync(new RecorderUndoRedo($"Set {change.PropertyName} of {change.Owner} to \"{change.NewValue}\"",
            async () =>
            {
                if (stateAttribute.IsOwner)
                    await RestoreAsync(change.NewValue);

                object? result = asyncSetterInfo.Invoke(change.Owner, new[] { change.NewValue });

                if (result is Task task)
                    await task;
                else if (result is ValueTask valueTask)
                    await valueTask;
                else
                    throw new InvalidOperationException($"Async setter \"{stateAttribute.AsyncSetterName}\" did not return a {nameof(Task)} nor a {nameof(ValueTask)}.");

                if (stateAttribute.IsOwner)
                    await StoreAsync(change.OldValue);

                await PresentAsync(new PresenterSubject(change.Owner, change.PropertyName));
            },
            async () =>
            {
                if (stateAttribute.IsOwner)
                    await RestoreAsync(change.OldValue);

                object? result = asyncSetterInfo.Invoke(change.Owner, new[] { change.OldValue });

                if (result is Task task)
                    await task;
                else if (result is ValueTask valueTask)
                    await valueTask;
                else
                    throw new InvalidOperationException($"Async setter \"{stateAttribute.AsyncSetterName}\" did not return a {nameof(Task)} nor a {nameof(ValueTask)}.");

                if (stateAttribute.IsOwner)
                    await StoreAsync(change.NewValue);

                await PresentAsync(new PresenterSubject(change.Owner, change.PropertyName));
            },
            doneDispose: async () =>
            {
                if (stateAttribute.IsOwner)
                    await DisposeAsync(change.OldValue);
            },
            undoneDispose: async () =>
            {
                if (stateAttribute.IsOwner)
                    await DisposeAsync(change.NewValue);
            }));
    }

    private async Task OnChangeAsync(CollectionChange change)
    {
        if (_isObservingStateChange)
            return;

        IAsyncList list = change.Owner;

        if (!_collectionAttributes.TryGetValue(list, out StateCollectionAttribute? stateAttribute))
            throw new InvalidOperationException("State collection was not registered correctly.");

        switch (change.Action)
        {
            case CollectionChangeAction.Add:

                if (change.NewItems is null)
                    throw new InvalidOperationException("New items were not provided.");
                if (change.NewStartingIndex < 0)
                    throw new InvalidOperationException("New items starting index was not provided.");

                if (stateAttribute.IsOwner)
                {
                    foreach (object? newItem in change.NewItems)
                    {
                        if (newItem is IStateChangeNotifier newState)
                        {
                            Subscribe(newState);
                        }
                    }
                }

                await ActiveStack.PushAsync(new RecorderUndoRedo($"Add {change.NewItems.Count} item(s) to {list}",
                        async () =>
                        {
                            for (int i = 0; i < change.NewItems.Count; i++)
                            {
                                var item = change.NewItems[i];

                                if (stateAttribute.Ownership == StateOwnership.Owner)
                                    await RestoreAsync(item);

                                await list.InsertAsync(change.NewStartingIndex + i, item);

                                await PresentAsync(new PresenterSubject(item));
                            }
                        },
                        async () =>
                        {
                            for (int i = change.NewItems.Count - 1; i >= 0; i--)
                            {
                                object? item = list[change.NewStartingIndex + i];

                                await list.RemoveAtAsync(change.NewStartingIndex + i);

                                if (stateAttribute.Ownership == StateOwnership.Owner)
                                    await StoreAsync(item);

                                await PresentAsync(new PresenterSubject(item));
                            }
                        },
                        undoneDispose: async () =>
                        {
                            if (stateAttribute.Ownership == StateOwnership.Owner)
                            {
                                for (int i = 0; i < change.NewItems.Count; i++)
                                    await DisposeAsync(change.NewItems[i]);
                            }
                        }));

                break;
            case CollectionChangeAction.Remove:

                if (change.OldItems is null)
                    throw new InvalidOperationException("Old items were not provided");
                if (change.OldStartingIndex < 0)
                    throw new InvalidOperationException("Old items starting index was not provided.");

                if (stateAttribute.IsOwner)
                {
                    foreach (object? oldItem in change.OldItems)
                    {
                        if (oldItem is IStateChangeNotifier oldState)
                        {
                            Unsubscribe(oldState);
                        }
                    }
                }

                await ActiveStack.PushAsync(new RecorderUndoRedo($"Remove {change.OldItems.Count} item(s) from {list}",
                    async () =>
                    {
                        for (int i = change.OldItems.Count - 1; i >= 0; i--)
                        {
                            var item = list[change.OldStartingIndex + i];

                            await list.RemoveAtAsync(change.OldStartingIndex + i);

                            if (stateAttribute.Ownership == StateOwnership.Owner)
                                await StoreAsync(item);

                            await PresentAsync(new PresenterSubject(item));
                        }
                    },
                    async () =>
                    {
                        for (int i = 0; i < change.OldItems.Count; i++)
                        {
                            var item = change.OldItems[i];

                            if (stateAttribute.Ownership == StateOwnership.Owner)
                                await RestoreAsync(item);

                            await list.InsertAsync(change.OldStartingIndex + i, item);

                            await PresentAsync(new PresenterSubject(item));
                        }
                    },
                    doneDispose: async () =>
                    {
                        if (stateAttribute.Ownership == StateOwnership.Owner)
                        {
                            for (int i = 0; i < change.OldItems.Count; i++)
                                await DisposeAsync(change.OldItems[i]);
                        }
                    }));

                break;
            case CollectionChangeAction.Replace:
                throw new NotSupportedException();
            case CollectionChangeAction.Move:
                throw new NotSupportedException();
            case CollectionChangeAction.Reset:
                throw new NotSupportedException();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task PresentAsync(PresenterSubject subject)
    {
        if (Presenter is null)
            return;

        await Presenter.PresentAsync(subject).ConfigureAwait(false);
    }

    private async Task StoreAsync(object? obj)
    {
        switch (obj)
        {
            case IAsyncRestorable asyncRestorable:
                await asyncRestorable.StoreAsync().ConfigureAwait(false);
                break;
            case IRestorable restorable:
                restorable.Store();
                break;
        }
    }

    private async Task RestoreAsync(object? obj)
    {
        switch (obj)
        {
            case IAsyncRestorable asyncRestorable:
                await asyncRestorable.RestoreAsync().ConfigureAwait(false);
                break;
            case IRestorable restorable:
                restorable.Restore();
                break;
        }
    }

    private async Task DisposeAsync(object? obj)
    {
        switch (obj)
        {
            case IAsyncDisposable asyncDisposable:
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                break;
            case IDisposable disposable:
                disposable.Dispose();
                break;
        }
    }

    private class RecorderUndoRedo : UndoRedo
    {
        public RecorderUndoRedo(string description, Func<Task> redo, Func<Task> undo, Func<Task>? doneDispose = null, Func<Task>? undoneDispose = null)
            : base(description, redo, undo, doneDispose, undoneDispose)
        {
        }

        public override async Task RedoAsync()
        {
            _isObservingStateChange = true;
            try
            {
                await base.RedoAsync();
            }
            finally
            {
                _isObservingStateChange = false;
            }
        }

        public override async Task UndoAsync()
        {
            _isObservingStateChange = true;
            try
            {
                await base.UndoAsync();
            }
            finally
            {
                _isObservingStateChange = false;
            }
        }
    }
}