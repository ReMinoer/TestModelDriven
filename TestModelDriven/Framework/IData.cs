using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public interface IData
{
    Task<object> ToModelAsync();
}

public interface IData<TModel> : IData
    where TModel : notnull
{
    new Task<TModel> ToModelAsync();
}