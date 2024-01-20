using System;
using System.Windows.Input;

namespace TestModelDriven.Framework;

public class DisabledCommand : ICommand
{
    static private DisabledCommand? _instance;
    static public DisabledCommand Instance => _instance ??= new DisabledCommand();

    private DisabledCommand() {}
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter) => false;
    public void Execute(object? parameter) {}
}