using System;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;

namespace TestModelDriven.Framework;

public class CommandDispatcherCommand : ICommand
{
    private readonly Func<string> _description;
    private readonly AsyncCommand _command;

    public event EventHandler? CanExecuteChanged
    {
        add => _command.CanExecuteChanged += value;
        remove => _command.CanExecuteChanged -= value;
    }

    public CommandDispatcherCommand(string description, Func<Task> execute, Func<bool>? canExecute = null)
        : this(() => description, execute, canExecute)
    {
    }

    public CommandDispatcherCommand(Func<string> description, Func<Task> execute, Func<bool>? canExecute = null)
    {
        _description = description;
        _command = canExecute is not null
            ? new AsyncCommand(execute, _ => canExecute())
            : new AsyncCommand(execute);
    }

    public bool CanExecute() => _command.CanExecute(null);
    public void Execute()
    {
        CommandDispatcher.Current.Push(_description(), _command.ExecuteAsync, () => _command.CanExecute(null));
    }

    bool ICommand.CanExecute(object? _) => CanExecute();
    void ICommand.Execute(object? _) => Execute();
}

public class CommandDispatcherCommand<T> : ICommand
{
    private readonly Func<string> _description;
    private readonly AsyncCommand<T> _command;

    public event EventHandler? CanExecuteChanged
    {
        add => _command.CanExecuteChanged += value;
        remove => _command.CanExecuteChanged -= value;
    }

    public CommandDispatcherCommand(string description, Func<T, Task> execute, Func<bool>? canExecute = null)
        : this(() => description, execute, canExecute)
    {
    }

    public CommandDispatcherCommand(Func<string> description, Func<T, Task> execute, Func<bool>? canExecute = null)
    {
        _description = description;
        _command = canExecute is not null
            ? new AsyncCommand<T>(x => execute(x!), _ => canExecute())
            : new AsyncCommand<T>(x => execute(x!));
    }

    public bool CanExecute(T parameter) => _command.CanExecute(parameter);
    public void Execute(T parameter)
    {
        CommandDispatcher.Current.Push(_description(), () => _command.ExecuteAsync(parameter), () => _command.CanExecute(parameter));
    }

    bool ICommand.CanExecute(object? parameter) => CanExecute((T)parameter!);
    void ICommand.Execute(object? parameter) => Execute((T)parameter!);
}