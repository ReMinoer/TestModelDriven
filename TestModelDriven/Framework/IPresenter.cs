using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public interface IPresenter
{
    Task<bool> PresentAsync(PresenterSubject subject);
}