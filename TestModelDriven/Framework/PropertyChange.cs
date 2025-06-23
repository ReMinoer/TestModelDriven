using System.ComponentModel;

namespace TestModelDriven.Framework;

public class PropertyChange
{
    public object Owner { get; }
    public string PropertyName { get; }

    public PropertyChange(object owner, string propertyName)
    {
        Owner = owner;
        PropertyName = propertyName;
    }

    public PropertyChangedEventArgs ToEventArgs()
    {
        return new PropertyChangedEventArgs(PropertyName);
    }
}