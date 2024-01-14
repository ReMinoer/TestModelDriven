namespace TestModelDriven.Framework;

public interface IPersistentModel<out TData>
{
    TData ToData();
}