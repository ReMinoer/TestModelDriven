using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public abstract class ViewModelBase : PropertyChangeNotifierBase, IViewModel
{
    public bool IsInitialized { get; private set; }
    public bool IsDisposed { get; private set; }

    public virtual Task InitializeAsync()
    {
        IsInitialized = true;
        return Task.CompletedTask;
    }

    public virtual ValueTask DisposeAsync()
    {
        IsDisposed = true;
        return ValueTask.CompletedTask;
    }

    protected void PushCommand(string description, Func<Task> executeTask, Func<bool>? canExecute = null)
    {
        CommandDispatcher.Current.Push(description, executeTask, () => !IsDisposed && (canExecute?.Invoke() ?? true));
    }

    protected void PushProperty<T>(string description, Func<T> modelGetter, T newValue, Func<Task> executeTask, Func<bool>? canExecute = null)
    {
        T oldValue = modelGetter();
        if (Equals(oldValue, newValue))
            return;

        PushCommand(description, executeTask, () =>
            {
                T currentValue = modelGetter();
                if (Equals(currentValue, newValue))
                    return false; // Current value is already the new value.
                if (!Equals(currentValue, oldValue))
                    return false; // Current value is not as it should be. Invalidate the command.

                return canExecute?.Invoke() ?? true;
            }
        );
    }

    protected void PushPropertyTwoWay<T>(string description, Func<T> modelGetter, T newValue, Func<Task> executeTask, Func<T> viewModelFieldGetter, Action<T> viewModelFieldSetter, Func<bool>? canExecute = null, [CallerMemberName] string? propertyName = null)
    {
        T viewModelOldValue = viewModelFieldGetter();
        if (Equals(viewModelOldValue, newValue))
            return;

        Task setViewModelField = SetViewModelField();

        async Task SetViewModelField()
        {
            viewModelFieldSetter(newValue);
            await RaisePropertyChangeAsync(viewModelOldValue, newValue, propertyName);
        }

        async Task WaitViewModelAndExecute()
        {
            await setViewModelField;
            await executeTask();
        }

        PushProperty(description, () => viewModelOldValue, newValue, WaitViewModelAndExecute, canExecute);
    }

    protected void PushPropertyTwoWay<T>(string description, Func<T> modelGetter, T newValue, Func<Task> executeTask, Func<T> viewModelFieldGetter, Func<T, Task> viewModelFieldSetter, Func<bool>? canExecute = null, [CallerMemberName] string? propertyName = null)
    {
        T viewModelOldValue = viewModelFieldGetter();
        if (Equals(viewModelOldValue, newValue))
            return;
        
        Task setViewModelField = SetViewModelField();

        async Task SetViewModelField()
        {
            await viewModelFieldSetter(newValue);
            await RaisePropertyChangeAsync(viewModelOldValue, newValue, propertyName);
        }

        async Task WaitViewModelAndExecute()
        {
            await setViewModelField;
            await executeTask();
        }

        PushProperty(description, () => viewModelOldValue, newValue, WaitViewModelAndExecute, canExecute);
    }

    public virtual void OnLoaded() {}
    public virtual void OnUnloaded() { }
}