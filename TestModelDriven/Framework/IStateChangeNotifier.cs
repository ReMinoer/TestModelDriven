namespace TestModelDriven.Framework;

public interface IStateChangeNotifier
{
    IAsyncEvent<StateChange> StateChangedAsync { get; }
}