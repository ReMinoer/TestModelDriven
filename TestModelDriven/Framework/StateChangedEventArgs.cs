using System.ComponentModel;

namespace TestModelDriven.Framework;

public class StateChangedEventArgs : PropertyChangedEventArgs
{
    public object? OldValue { get; }
    public object? NewValue { get; }

    public StateChangedEventArgs(string propertyName, object? oldValue, object? newValue)
        : base(propertyName)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}