namespace TestModelDriven.Framework;

public interface IPersistentModel
{
    IData ToData();
}

public interface IPersistentModel<out TData> : IPersistentModel
    where TData : IData
{
    new TData ToData();
}