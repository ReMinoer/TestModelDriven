using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public abstract class DataBase<TModel> : IData<TModel>
    where TModel : notnull
{
    public abstract Task<TModel> ToModelAsync();
    async Task<object> IData.ToModelAsync() => await ToModelAsync();
}