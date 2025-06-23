namespace TestModelDriven.Framework;

public interface IPropertyChangeNotifier
{
    IAsyncEvent<PropertyChange> PropertyChangedAsync { get; }
}