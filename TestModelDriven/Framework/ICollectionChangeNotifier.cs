namespace TestModelDriven.Framework;

public interface ICollectionChangeNotifier
{
    IAsyncEvent<CollectionChange> CollectionChangedAsync { get; }
}