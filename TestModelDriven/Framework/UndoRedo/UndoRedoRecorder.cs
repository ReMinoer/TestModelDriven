using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

namespace TestModelDriven.Framework.UndoRedo;

public class UndoRedoRecorder
{
    private readonly IUndoRedoStack _undoRedoStack;
    private static bool _isObservingStateChange;

    public IFocuser? Focuser { get; set; }

    public UndoRedoRecorder(IUndoRedoStack undoRedoStack)
    {
        _undoRedoStack = undoRedoStack;
    }

    public void Subscribe(INotifyStateChanged state)
    {
        state.StateChanged += OnStateChanged;

        foreach (PropertyInfo statePropertyInfo in GetStateProperties(state.GetType()))
        {
            var value = statePropertyInfo.GetValue(state);

            if (value is INotifyStateChanged subState)
            {
                Subscribe(subState);
            }

            if (value is IEnumerable subStateEnumerable)
            {
                foreach (INotifyStateChanged subStateItem in subStateEnumerable.OfType<INotifyStateChanged>())
                {
                    Subscribe(subStateItem);
                }
            }

            if (value is INotifyCollectionChanged subStateNotifyCollectionChanged)
            {
                subStateNotifyCollectionChanged.CollectionChanged += OnStateCollectionChanged;
            }
        }
    }

    public void Unsubscribe(INotifyStateChanged state)
    {
        foreach (PropertyInfo statePropertyInfo in GetStateProperties(state.GetType()))
        {
            var value = statePropertyInfo.GetValue(state);

            if (value is INotifyStateChanged subState)
            {
                Unsubscribe(subState);
            }

            if (value is IEnumerable subStateEnumerable)
            {
                foreach (INotifyStateChanged subStateItem in subStateEnumerable.OfType<INotifyStateChanged>())
                {
                    Unsubscribe(subStateItem);
                }
            }

            if (value is INotifyCollectionChanged subStateNotifyCollectionChanged)
            {
                subStateNotifyCollectionChanged.CollectionChanged -= OnStateCollectionChanged;
            }
        }

        state.StateChanged -= OnStateChanged;
    }

    private static IEnumerable<PropertyInfo> GetStateProperties(Type type)
    {
        return type.GetProperties().Where(x => x.GetCustomAttribute<StateAttribute>() is not null);
    }

    private void OnStateChanged(object sender, StateChangedEventArgs e)
    {
        if (_isObservingStateChange)
            return;

        if (e.PropertyName is null)
            return;

        PropertyInfo? propertyInfo = sender.GetType().GetProperty(e.PropertyName);
        if (propertyInfo is null)
            throw new InvalidOperationException();

        var stateAttribute = propertyInfo.GetCustomAttribute<StateAttribute>();
        if (stateAttribute is null)
            return;

        if (stateAttribute.Ownership == StateOwnership.Owner)
        {
            if (e.OldValue is INotifyStateChanged oldState)
            {
                Unsubscribe(oldState);
            }

            if (e.NewValue is INotifyStateChanged newState)
            {
                Subscribe(newState);
            }
        }

        _undoRedoStack.Push(new RecorderUndoRedo($"Set {e.PropertyName} of {sender} to \"{e.NewValue}\"",
            () =>
            {
                if (stateAttribute.Ownership == StateOwnership.Owner)
                    (e.NewValue as IRestorable)?.Restore();

                propertyInfo.SetValue(sender, e.NewValue);

                if (stateAttribute.Ownership == StateOwnership.Owner)
                    (e.OldValue as IRestorable)?.Store();

                Focuser?.Focus(sender, e.PropertyName);
            },
            () =>
            {
                if (stateAttribute.Ownership == StateOwnership.Owner)
                    (e.OldValue as IRestorable)?.Restore();
                
                propertyInfo.SetValue(sender, e.OldValue);

                if (stateAttribute.Ownership == StateOwnership.Owner)
                    (e.NewValue as IRestorable)?.Store();

                Focuser?.Focus(sender, e.PropertyName);
            },
            doneDispose: () =>
            {
                if (stateAttribute.Ownership == StateOwnership.Owner)
                    (e.OldValue as IDisposable)?.Dispose();
            },
            undoneDispose: () =>
            {
                if (stateAttribute.Ownership == StateOwnership.Owner)
                    (e.NewValue as IDisposable)?.Dispose();
            }));
    }

    private void OnStateCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_isObservingStateChange)
            return;
        
        if (sender is not IList list)
            return;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:

                //if (stateAttribute.Ownership == StateOwnership.Owner)
                {
                    foreach (object? newItem in e.NewItems!)
                    {
                        if (newItem is INotifyStateChanged newState)
                        {
                            Subscribe(newState);
                        }
                    }
                }

                _undoRedoStack.Push(new RecorderUndoRedo($"Add {e.NewItems.Count} item(s) to {sender}",
                        () =>
                        {
                            for (int i = 0; i < e.NewItems.Count; i++)
                            {
                                var item = e.NewItems[i];

                                //if (stateAttribute.Ownership == StateOwnership.Owner)
                                    (item as IRestorable)?.Restore();

                                list.Insert(e.NewStartingIndex + i, item);

                                Focuser?.Focus(item);
                            }
                        },
                        () =>
                        {
                            for (int i = e.NewItems.Count - 1; i >= 0; i--)
                            {
                                var item = list[e.NewStartingIndex + i];

                                list.RemoveAt(e.NewStartingIndex + i);

                                //if (stateAttribute.Ownership == StateOwnership.Owner)
                                    (item as IRestorable)?.Store();

                                Focuser?.Focus(sender);
                            }
                        },
                        undoneDispose: () =>
                        {
                            //if (stateAttribute.Ownership == StateOwnership.Owner)
                            {
                                for (int i = 0; i < e.NewItems.Count; i++)
                                    (e.NewItems[i] as IDisposable)?.Dispose();
                            }
                        }));

                break;
            case NotifyCollectionChangedAction.Remove:

                //if (stateAttribute.Ownership == StateOwnership.Owner)
                {
                    foreach (object? oldItem in e.OldItems!)
                    {
                        if (oldItem is INotifyStateChanged oldState)
                        {
                            Unsubscribe(oldState);
                        }
                    }
                }

                _undoRedoStack.Push(new RecorderUndoRedo($"Remove {e.OldItems.Count} item(s) from {sender}",
                    () =>
                    {
                        for (int i = e.OldItems.Count - 1; i >= 0; i--)
                        {
                            var item = list[e.OldStartingIndex + i];

                            list.RemoveAt(e.OldStartingIndex + i);

                            //if (stateAttribute.Ownership == StateOwnership.Owner)
                                (item as IRestorable)?.Store();

                            Focuser?.Focus(sender);
                        }
                    },
                    () =>
                    {
                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            var item = e.OldItems[i];

                            //if (stateAttribute.Ownership == StateOwnership.Owner)
                                (item as IRestorable)?.Restore();

                            list.Insert(e.OldStartingIndex + i, item);

                            Focuser?.Focus(item);
                        }
                    },
                    doneDispose: () =>
                    {
                        //if (stateAttribute.Ownership == StateOwnership.Owner)
                        {
                            for (int i = 0; i < e.OldItems.Count; i++)
                                (e.OldItems[i] as IDisposable)?.Dispose();
                        }
                    }));

                break;
            case NotifyCollectionChangedAction.Replace:
                throw new NotSupportedException();
            case NotifyCollectionChangedAction.Move:
                throw new NotSupportedException();
            case NotifyCollectionChangedAction.Reset:
                throw new NotSupportedException();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private class RecorderUndoRedo : UndoRedo
    {
        public RecorderUndoRedo(string description, Action redo, Action undo, Action? doneDispose = null, Action? undoneDispose = null)
            : base(description, redo, undo, doneDispose, undoneDispose)
        {
        }

        public override void Redo()
        {
            _isObservingStateChange = true;
            try
            {
                base.Redo();
            }
            finally
            {
                _isObservingStateChange = false;
            }
        }

        public override void Undo()
        {
            _isObservingStateChange = true;
            try
            {
                base.Undo();
            }
            finally
            {
                _isObservingStateChange = false;
            }
        }
    }
}