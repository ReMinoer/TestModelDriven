using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TestModelDriven.Framework;

public abstract class ModelBase : NotifyPropertyChangedBase, INotifyStateChanged
{
    static private ConcurrentDictionary<Type, HashSet<string>> _statePropertyNames = new();
    public event StateChangedPropertyHandler? StateChanged;

    protected override void RaisePropertyChanged(object? oldValue, object? newValue, [CallerMemberName] string? propertyName = null)
    {
        base.RaisePropertyChanged(oldValue, newValue, propertyName);

        if (propertyName is null)
            return;
        
        if (_statePropertyNames.GetOrAdd(GetType(), GetStatePropertyNames).Contains(propertyName))
            StateChanged?.Invoke(this, new StateChangedEventArgs(propertyName, oldValue, newValue));
    }

    static private HashSet<string> GetStatePropertyNames(Type type)
    {
        return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(x => x.GetCustomAttribute<StateAttribute>() is not null)
            .Select(x => x.Name)
            .ToHashSet();
    }
}