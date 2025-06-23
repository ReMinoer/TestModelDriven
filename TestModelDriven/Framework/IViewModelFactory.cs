using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public interface IViewModelFactory<T>
    where T : IViewModel?
{
    Task<T> CreateViewModelAsync(object model);
}