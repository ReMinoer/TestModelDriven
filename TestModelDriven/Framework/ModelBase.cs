using System.Runtime.CompilerServices;

namespace TestModelDriven.Framework;

public abstract class ModelBase : NotifyPropertyChangedBase, INotifyStateChanged
{
    public event StateChangedPropertyHandler? StateChanged;

    protected override void RaisePropertyChanged(object? oldValue, object? newValue, [CallerMemberName] string? propertyName = null)
    {
        base.RaisePropertyChanged(oldValue, newValue, propertyName);
        StateChanged?.Invoke(this, new StateChangedEventArgs(propertyName, oldValue, newValue));
    }
}