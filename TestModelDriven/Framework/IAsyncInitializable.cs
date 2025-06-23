using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public interface IAsyncInitializable
{
    bool IsInitialized { get; }
    Task InitializeAsync();
}