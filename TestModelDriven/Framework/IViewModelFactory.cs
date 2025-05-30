namespace TestModelDriven.Framework;

public interface IViewModelFactory<out T>
    where T : IViewModel
{
    T? CreateViewModel(object model);
}