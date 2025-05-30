using System.Collections.ObjectModel;
using System.Windows.Input;

namespace TestModelDriven.Framework.Application;

public class MenuItemViewModel
{
    public string Header { get; }
    public ICommand? Command { get; }
    public object? CommandParameter { get; }
    public ObservableCollection<MenuItemViewModel> SubItems { get; }

    public MenuItemViewModel(string header, ICommand? command = null, object? commandParameter = null)
    {
        Header = header;
        Command = command;
        CommandParameter = commandParameter;
        SubItems = new ObservableCollection<MenuItemViewModel>();
    }
}