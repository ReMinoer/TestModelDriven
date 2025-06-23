using System;
using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public abstract class OneForOneViewModelBase<TModel> : ViewModelBase, IOneForOneViewModel
    where TModel : IPropertyChangeNotifier
{
    private readonly IDisposable _propertyChangeSubscription;

    public TModel Model { get; }
    object IOneForOneViewModel.Model => Model;

    public OneForOneViewModelBase(TModel model)
    {
        Model = model;
        _propertyChangeSubscription = Model.PropertyChangedAsync.Subscribe(OnPropertyChangeAsync);
    }

    public override ValueTask DisposeAsync()
    {
        _propertyChangeSubscription.Dispose();
        return ValueTask.CompletedTask;
    }
    
    private Task OnPropertyChangeAsync(PropertyChange change) => OnModelPropertyChangedAsync(change.PropertyName);
    protected abstract Task OnModelPropertyChangedAsync(string propertyName);
}