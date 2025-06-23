using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public interface IAsyncRestorable
{
    Task StoreAsync();
    Task RestoreAsync();
}