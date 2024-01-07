using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestModelDriven.Framework;

public class NotifyPropertyChangedBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void RaisePropertyChanged(object? oldValue, object? newValue, [CallerMemberName] string? propertyName = null)
    {
        RaisePropertyChanged(propertyName);
    }

    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        T oldValue = field;
        field = value;

        RaisePropertyChanged(oldValue, value, propertyName);
        return true;
    }
}