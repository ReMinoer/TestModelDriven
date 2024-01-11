using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace TestModelDriven.Framework;

public class ViewModelCollection<TModel, TViewModel> : IReadOnlyList<TViewModel>, INotifyPropertyChanged, INotifyCollectionChanged, IDisposable
{
    private readonly ObservableCollection<TViewModel> _list;
    private readonly INotifyPropertyChanged _propertyChangedSource;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public int Count => _list.Count;
    public TViewModel this[int index] => _list[index];

    private readonly ObservableCollection<TModel> _modelList;
    private readonly Func<TModel, TViewModel> _viewModelFactory;
    private readonly Func<TViewModel, TModel> _modelGetter;

    public ViewModelCollection(ObservableCollection<TModel> modelList, Func<TModel, TViewModel> viewModelFactory, Func<TViewModel, TModel> modelGetter)
    {
        _list = new ObservableCollection<TViewModel>();
        _propertyChangedSource = _list;

        _propertyChangedSource.PropertyChanged += OnPropertyChanged;
        _list.CollectionChanged += OnCollectionChanged;

        _modelList = modelList;
        _viewModelFactory = viewModelFactory;
        _modelGetter = modelGetter;

        for (int i = 0; i < _modelList.Count; i++)
        {
            _list.Insert(i, _viewModelFactory(_modelList[i]));
        }

        _modelList.CollectionChanged += OnModelCollectionChanged;
    }

    public void Dispose()
    {
        _modelList.CollectionChanged -= OnModelCollectionChanged;

        _list.CollectionChanged -= OnCollectionChanged;
        _propertyChangedSource.PropertyChanged -= OnPropertyChanged;
    }

    private void OnModelCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                for (int i = 0; i < e.NewItems!.Count; i++)
                {
                    var newModelItem = (TModel)e.NewItems[i];
                    TViewModel newViewModelItem = _viewModelFactory(newModelItem);
                    _list.Insert(e.NewStartingIndex + i, newViewModelItem);
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                for (int i = 0; i < e.OldItems!.Count; i++)
                {
                    var oldModelItem = (TModel)e.OldItems[i];
                    TViewModel oldViewModelItem = this.First(x => Equals(_modelGetter(x), oldModelItem));
                    _list.Remove(oldViewModelItem);
                }
                break;
            case NotifyCollectionChangedAction.Replace:
                throw new NotSupportedException();
            case NotifyCollectionChangedAction.Move:
                throw new NotSupportedException();
            case NotifyCollectionChangedAction.Reset:
                _list.Clear();
                for (int i = 0; i < _modelList.Count; i++)
                {
                    _list.Insert(i, _viewModelFactory(_modelList[i]));
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Count))
            PropertyChanged?.Invoke(this, e);
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
    }

    public IEnumerator<TViewModel> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}