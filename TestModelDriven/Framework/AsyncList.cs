using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public interface IReadOnlyCollection : IEnumerable
{
    int Count { get; }
}

public interface IReadOnlyList : IReadOnlyCollection
{
    object? this[int index] { get; }
}

public interface IAsyncList : IReadOnlyList
{
    bool IsReadOnly { get; }
    Task AddAsync(object? item);
    Task<bool> RemoveAsync(object? item);
    Task SetAsync(int index, object? item);
    Task InsertAsync(int index, object? item);
    Task RemoveAtAsync(int index);
    Task ClearAsync();
    bool Contains(object? item);
    int IndexOf(object? item);
    void CopyTo(object?[] array, int arrayIndex);
}

public interface IAsyncList<T> : IReadOnlyList<T>
{
    bool IsReadOnly { get; }
    Task AddAsync(T item);
    Task<bool> RemoveAsync(T item);
    Task SetAsync(int index, T item);
    Task InsertAsync(int index, T item);
    Task RemoveAtAsync(int index);
    Task ClearAsync();
    bool Contains(T item);
    int IndexOf(T item);
    void CopyTo(T[] array, int arrayIndex);
}

public enum CollectionChangeAction
{
    Add, Remove, Replace, Move, Reset
}

public class CollectionChange
{
    public IAsyncList Owner { get; }
    public CollectionChangeAction Action { get; }
    public IList? NewItems { get; }
    public IList? OldItems { get; }
    public int NewStartingIndex { get; }
    public int OldStartingIndex { get; }

    private CollectionChange(IAsyncList owner, CollectionChangeAction action, IList? newItems, IList? oldItems, int newStartingIndex, int oldStartingIndex)
    {
        Owner = owner;
        Action = action;
        NewItems = newItems;
        OldItems = oldItems;
        NewStartingIndex = newStartingIndex;
        OldStartingIndex = oldStartingIndex;
    }

    static public CollectionChange Add(IAsyncList owner, IList newItems, int newStartingIndex)
        => new CollectionChange(owner, CollectionChangeAction.Add, newItems, null, newStartingIndex, -1);
    static public CollectionChange Remove(IAsyncList owner, IList oldItems, int oldStartingIndex)
        => new CollectionChange(owner, CollectionChangeAction.Remove, null, oldItems, -1, oldStartingIndex);
    static public CollectionChange Replace(IAsyncList owner, IList oldItems, IList newItems, int newStartingIndex)
        => new CollectionChange(owner, CollectionChangeAction.Replace, newItems, oldItems, newStartingIndex, newStartingIndex);
    static public CollectionChange Move(IAsyncList owner, IList items, int newStartingIndex, int oldStartingIndex)
        => new CollectionChange(owner, CollectionChangeAction.Move, items, null, newStartingIndex, oldStartingIndex);
    static public CollectionChange Reset(IAsyncList owner)
        => new CollectionChange(owner, CollectionChangeAction.Reset, null, null, -1, -1);

    public NotifyCollectionChangedEventArgs ToEventArgs()
    {
        switch (Action)
        {
            case CollectionChangeAction.Add:
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, NewItems, NewStartingIndex);
            case CollectionChangeAction.Remove:
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, OldItems, OldStartingIndex);
            case CollectionChangeAction.Replace:
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, NewItems!, OldItems!, NewStartingIndex);
            case CollectionChangeAction.Move:
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, NewItems, NewStartingIndex, OldStartingIndex);
            case CollectionChangeAction.Reset:
                return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            default: throw new ArgumentOutOfRangeException();
        }
    }
}

public class AsyncList<T> : PropertyChangeNotifierBase, IAsyncList<T>, IAsyncList, ICollectionChangeNotifier, INotifyCollectionChanged
{
    private readonly List<T> _list;

    public int Count => _list.Count;
    public bool IsReadOnly => false;
    public T this[int index] => _list[index];

    private readonly AsyncEvent<CollectionChange> _collectionChangedAsync = new();
    public IAsyncEvent<CollectionChange> CollectionChangedAsync => _collectionChangedAsync.Public;
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public AsyncList()
    {
        _list = new List<T>();
    }

    public AsyncList(int capacity)
    {
        _list = new List<T>(capacity);
    }

    public Task AddAsync(T item) => InsertAsync(Count, item);
    public async Task InsertAsync(int index, T item)
    {
        _list.Insert(index, item);

        await RaiseCountChangeAsync(Count - 1);

        CollectionChange collectionChange = CollectionChange.Add(this, new[] { item }, index);
        await RaiseCollectionChange(collectionChange);
    }

    public async Task RemoveAtAsync(int index)
    {
        T previousItem = _list[index];
        _list.RemoveAt(index);

        await RaiseCountChangeAsync(Count + 1);

        CollectionChange collectionChange = CollectionChange.Remove(this, new[] { previousItem }, index);
        await RaiseCollectionChange(collectionChange);
    }

    public async Task<bool> RemoveAsync(T item)
    {
        int index = IndexOf(item);
        if (index == -1)
            return false;

        await RemoveAtAsync(index);
        return true;
    }

    public async Task SetAsync(int index, T item)
    {
        T previousItem = _list[index];
        _list[index] = item;

        await RaiseCountChangeAsync(Count - 1);

        CollectionChange collectionChange = CollectionChange.Replace(this, new[] { item }, new[] { previousItem }, index);
        await RaiseCollectionChange(collectionChange);
    }

    public async Task ClearAsync()
    {
        int oldCount = Count;
        _list.Clear();

        await RaiseCountChangeAsync(oldCount);

        CollectionChange collectionChange = CollectionChange.Reset(this);
        await RaiseCollectionChange(collectionChange);
    }

    private async Task RaiseCollectionChange(CollectionChange collectionChange)
    {
        await _collectionChangedAsync.RaiseAsync(collectionChange);
        CollectionChanged?.Invoke(this, collectionChange.ToEventArgs());
    }

    private Task RaiseCountChangeAsync(int oldCount) => RaisePropertyChangeAsync(oldCount, Count, nameof(Count));

    public bool Contains(T item) => _list.Contains(item);
    public int IndexOf(T item) => _list.IndexOf(item);
    public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_list).GetEnumerator();

    Task IAsyncList.AddAsync(object? item) => AddAsync((T)item);
    Task<bool> IAsyncList.RemoveAsync(object? item) => RemoveAsync((T)item);
    Task IAsyncList.SetAsync(int index, object? item) => SetAsync(index, (T)item);
    Task IAsyncList.InsertAsync(int index, object? item) => InsertAsync(index, (T)item);
    bool IAsyncList.Contains(object? item) => Contains((T)item);
    int IAsyncList.IndexOf(object? item) => IndexOf((T)item);
    void IAsyncList.CopyTo(object?[] array, int arrayIndex) => Array.Copy(_list.ToArray(), arrayIndex, array, 0, _list.Count);
    object? IReadOnlyList.this[int index] => this[index];
}