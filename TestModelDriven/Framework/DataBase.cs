namespace TestModelDriven.Framework;

public abstract class DataBase<TModel> : IData<TModel>
    where TModel : notnull
{
    public abstract TModel ToModel();
    object IData.ToModel() => ToModel();
}