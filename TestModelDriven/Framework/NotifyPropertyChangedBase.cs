using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestModelDriven.Framework;

public class NotifyPropertyChangedBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void RaisePropertyChange([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        RaisePropertyChange(propertyName);
        return true;
    }
}