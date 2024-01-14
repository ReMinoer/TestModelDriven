namespace TestModelDriven.Framework;

public interface IData<out TModel>
{
    TModel ToModel();
}