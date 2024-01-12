using System.ComponentModel;

namespace TestModelDriven.Framework;

public class OneForOneViewModel<TModel> : ViewModelBase
    where TModel : INotifyPropertyChanged
{
    public TModel Model { get; }

    public OneForOneViewModel(TModel model)
    {
        Model = model;
    }

    public override void OnLoaded()
    {
        base.OnLoaded();
        Model.PropertyChanged += OnPropertyChanged;
    }

    public override void OnUnloaded()
    {
        Model.PropertyChanged -= OnPropertyChanged;
        base.OnUnloaded();
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnModelPropertyChanged(e.PropertyName);
    }

    protected virtual void OnModelPropertyChanged(string? propertyName) {}
}