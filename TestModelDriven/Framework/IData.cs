namespace TestModelDriven.Framework;

public interface IData
{
    object ToModel();
}

public interface IData<out TModel> : IData
    where TModel : notnull
{
    new TModel ToModel();
}