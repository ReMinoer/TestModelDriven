using System;
using System.Windows.Input;

namespace TestModelDriven.Framework.Application;

public class TargetedCommand<TTarget> : ICommand
{
    private readonly Func<TTarget> _targetSelector;
    private readonly Func<TTarget, ICommand?> _commandSelector;

    public event EventHandler? CanExecuteChanged;

    public TargetedCommand(Func<TTarget> targetSelector, Func<TTarget, ICommand?> commandSelector)
    {
        _targetSelector = targetSelector;
        _commandSelector = commandSelector;
        CommandManager.RequerySuggested += OnRequerySuggested;
    }

    private void OnRequerySuggested(object? sender, EventArgs e)
    {
        CanExecuteChanged?.Invoke(this, e);
    }

    public bool CanExecute(object? parameter)
    {
        return GetCommand()?.CanExecute(parameter) ?? false;
    }

    public void Execute(object? parameter)
    {
        GetCommand()?.Execute(parameter);
    }

    private ICommand? GetCommand()
    {
        TTarget target = _targetSelector();
        if (target is null)
            return null;

        return _commandSelector(target);
    }
}