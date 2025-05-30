using System;
using System.ComponentModel;

namespace TestModelDriven.Framework;

public abstract class OneForOneViewModelBase<TModel> : ViewModelBase, IOneForOneViewModel, IDisposable
    where TModel : INotifyPropertyChanged
{
    public TModel Model { get; }
    object IOneForOneViewModel.Model => Model;

    public OneForOneViewModelBase(TModel model)
    {
        Model = model;
        Model.PropertyChanged += OnPropertyChanged;
    }

    public void Dispose()
    {
        Model.PropertyChanged -= OnPropertyChanged;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnModelPropertyChanged(e.PropertyName);
    }

    protected abstract void OnModelPropertyChanged(string? propertyName);
}