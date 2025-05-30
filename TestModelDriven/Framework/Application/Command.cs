using System;
using System.Windows.Input;

namespace TestModelDriven.Framework.Application;

public class Command : ICommand
{
    private readonly Func<object?, bool>? _canExecute;
    private readonly Action<object?> _execute;

    public event EventHandler? CanExecuteChanged;
    
    public Command(Action<object?> execute)
    {
        _execute = execute;
        CommandManager.RequerySuggested += OnRequerySuggested;
    }

    public Command(Func<object?, bool> canExecute, Action<object?> execute)
        : this(execute)
    {
        _canExecute = canExecute;
    }

    private void OnRequerySuggested(object? sender, EventArgs e)
    {
        CanExecuteChanged?.Invoke(this, e);
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke(parameter) ?? true;
    }

    public void Execute(object? parameter)
    {
        _execute(parameter);
    }
}

public class Command<T> : ICommand
{
    private readonly Func<T, bool>? _canExecute;
    private readonly Action<T> _execute;

    public event EventHandler? CanExecuteChanged;

    public Command(Action<T> execute)
    {
        _execute = execute;
        CommandManager.RequerySuggested += OnRequerySuggested;
    }

    public Command(Func<T, bool> canExecute, Action<T> execute)
        : this(execute)
    {
        _canExecute = canExecute;
    }

    private void OnRequerySuggested(object? sender, EventArgs e)
    {
        CanExecuteChanged?.Invoke(this, e);
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke((T)parameter) ?? true;
    }

    public void Execute(object? parameter)
    {
        _execute((T)parameter);
    }
}