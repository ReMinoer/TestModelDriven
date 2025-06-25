using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public class PropertyChangeNotifierBase : IPropertyChangeNotifier, INotifyPropertyChanged
{
    private readonly AsyncEvent<PropertyChange> _propertyChangedAsync = new();
    public IAsyncEvent<PropertyChange> PropertyChangedAsync => _propertyChangedAsync.Public;
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual async Task RaisePropertyChangeAsync<T>(T oldValue, T newValue, string? propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        if (propertyName is null)
            return;

        var propertyChanged = new PropertyChange(this, propertyName);
        await _propertyChangedAsync.RaiseAsync(propertyChanged);
    }

    protected Task<bool> SetAsync<T>(ref T field, T value, string? propertyName)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return Task.FromResult(false);

        T oldValue = field;
        field = value;
        
        // Hack to keep the "ref" keyword in signature
        return EndSetAsync(oldValue, value, propertyName);
    }

    private async Task<bool> EndSetAsync<T>(T oldValue, T value, string? propertyName)
    {
        await RaisePropertyChangeAsync(oldValue, value, propertyName);
        return true;
    }

    protected ChangeScope<T> SetPropertyScope<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        T oldValue = field;
        field = value;
        return new ChangeScope<T>(this, oldValue, value, propertyName);
    }

    public class ChangeScope<T> : IAsyncDisposable
    {
        private readonly PropertyChangeNotifierBase _notifier;
        private readonly T _oldValue;
        private readonly T _newValue;
        private readonly string? _propertyName;

        public ChangeScope(PropertyChangeNotifierBase notifier, T oldValue, T newValue, string? propertyName)
        {
            _notifier = notifier;
            _oldValue = oldValue;
            _newValue = newValue;
            _propertyName = propertyName;
        }

        public async ValueTask DisposeAsync()
        {
            await _notifier.RaisePropertyChangeAsync(_oldValue, _newValue, _propertyName);
        }
    }
}