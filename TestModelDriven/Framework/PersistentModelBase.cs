namespace TestModelDriven.Framework;

public abstract class PersistentModelBase<TData> : ModelBase, IPersistentModel<TData>
{
    public abstract TData ToData();
}