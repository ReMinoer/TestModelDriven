using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public abstract class StateChangeNotifierBase : PropertyChangeNotifierBase, IStateChangeNotifier
{
    static private readonly ConcurrentDictionary<Type, HashSet<string>> StatePropertyNames = new();

    private readonly AsyncEvent<StateChange> _stateChangedAsync = new();
    public IAsyncEvent<StateChange> StateChangedAsync => _stateChangedAsync.Public;

    protected override async Task RaisePropertyChangeAsync<T>(T oldValue, T newValue, string? propertyName)
    {
        await base.RaisePropertyChangeAsync(oldValue, newValue, propertyName);

        if (propertyName is null)
            return;

        if (StatePropertyNames.GetOrAdd(GetType(), GetStatePropertyNames).Contains(propertyName))
        {
            var stateChanged = new StateChange(this, propertyName, oldValue, newValue);
            await _stateChangedAsync.RaiseAsync(stateChanged);
        }
    }

    static private HashSet<string> GetStatePropertyNames(Type type)
    {
        return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(x => x.GetCustomAttribute<StateAttribute>() is not null)
            .Select(x => x.Name)
            .ToHashSet();
    }
}