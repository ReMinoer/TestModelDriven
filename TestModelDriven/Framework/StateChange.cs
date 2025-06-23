namespace TestModelDriven.Framework;

public class StateChange
{
    public object Owner { get; }
    public string PropertyName { get; }
    public object? OldValue { get; }
    public object? NewValue { get; }

    public StateChange(object owner, string propertyName, object? oldValue, object? newValue)
    {
        Owner = owner;
        PropertyName = propertyName;
        OldValue = oldValue;
        NewValue = newValue;
    }
}