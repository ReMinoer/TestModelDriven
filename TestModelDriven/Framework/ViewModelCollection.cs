using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public class ViewModelCollection<TModel, TViewModel>
    : IReadOnlyList<TViewModel>, IPropertyChangeNotifier, ICollectionChangeNotifier,
        INotifyPropertyChanged, INotifyCollectionChanged, IDisposable
{
    private readonly AsyncList<TViewModel> _list;
    private readonly IDisposable _propertyChangedSubscription;
    private readonly IDisposable _collectionChangedSubscription;

    private readonly AsyncList<TModel> _modelList;
    private readonly IDisposable _modelListSubscription;
    private readonly Func<TModel, Task<TViewModel>> _viewModelFactory;
    private readonly Func<TViewModel, TModel> _modelGetter;

    public int Count => _list.Count;
    public TViewModel this[int index] => _list[index];

    private readonly AsyncEvent<PropertyChange> _propertyChangedAsync = new();
    public IAsyncEvent<PropertyChange> PropertyChangedAsync => _propertyChangedAsync.Public;
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly AsyncEvent<CollectionChange> _collectionChangedAsync = new();
    public IAsyncEvent<CollectionChange> CollectionChangedAsync => _collectionChangedAsync.Public;
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public ViewModelCollection(
        AsyncList<TModel> modelList,
        Func<TModel, Task<TViewModel>> viewModelFactory,
        Func<TViewModel, TModel> modelGetter)
    {
        _list = new AsyncList<TViewModel>();
        _propertyChangedSubscription = _list.PropertyChangedAsync.Subscribe(OnPropertyChangedAsync);
        _collectionChangedSubscription = _list.CollectionChangedAsync.Subscribe(OnCollectionChangedAsync);

        _modelList = modelList;
        _viewModelFactory = viewModelFactory;
        _modelGetter = modelGetter;

        _modelListSubscription = _modelList.CollectionChangedAsync.Subscribe(OnModelCollectionChangedAsync);
    }

    public async Task InitializeAsync()
    {
        for (int i = 0; i < _modelList.Count; i++)
        {
            await _list.InsertAsync(i, await _viewModelFactory(_modelList[i]));
        }
    }

    public void Dispose()
    {
        _modelListSubscription.Dispose();

        _collectionChangedSubscription.Dispose();
        _propertyChangedSubscription.Dispose();
    }

    public bool HasViewModel(object? model)
    {
        if (model is null)
            return false;

        return _list.Any(x => Equals(_modelGetter(x), model));
    }

    public TViewModel? GetViewModel(object? model)
    {
        if (model is null)
            return default;

        return _list.FirstOrDefault(x => Equals(_modelGetter(x), model));
    }

    private async Task OnModelCollectionChangedAsync(CollectionChange change)
    {
        switch (change.Action)
        {
            case CollectionChangeAction.Add:
                for (int i = 0; i < change.NewItems!.Count; i++)
                {
                    var newModelItem = (TModel)change.NewItems[i];
                    TViewModel newViewModelItem = await _viewModelFactory(newModelItem);
                    await _list.InsertAsync(change.NewStartingIndex + i, newViewModelItem);
                }
                break;
            case CollectionChangeAction.Remove:
                for (int i = 0; i < change.OldItems!.Count; i++)
                {
                    var oldModelItem = (TModel)change.OldItems[i];
                    TViewModel oldViewModelItem = this.First(x => Equals(_modelGetter(x), oldModelItem));
                    await _list.RemoveAsync(oldViewModelItem);
                }
                break;
            case CollectionChangeAction.Replace:
                throw new NotSupportedException();
            case CollectionChangeAction.Move:
                throw new NotSupportedException();
            case CollectionChangeAction.Reset:
                await _list.ClearAsync();
                for (int i = 0; i < _modelList.Count; i++)
                {
                    await _list.InsertAsync(i, await _viewModelFactory(_modelList[i]));
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task OnPropertyChangedAsync(PropertyChange change)
    {
        await _propertyChangedAsync.RaiseAsync(change);
        PropertyChanged?.Invoke(this, change.ToEventArgs());
    }

    private async Task OnCollectionChangedAsync(CollectionChange change)
    {
        await _collectionChangedAsync.RaiseAsync(change);
        CollectionChanged?.Invoke(this, change.ToEventArgs());
    }

    public IEnumerator<TViewModel> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}