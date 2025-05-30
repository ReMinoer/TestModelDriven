namespace TestModelDriven.Framework;

public abstract class PersistentModelBase<TData> : ModelBase, IPersistentModel<TData>
    where TData : IData
{
    public abstract TData ToData();
    IData IPersistentModel.ToData() => ToData();
}