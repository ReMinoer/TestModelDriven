namespace TestModelDriven.Framework;

public interface INotifyStateChanged
{
    event StateChangedPropertyHandler? StateChanged;
}