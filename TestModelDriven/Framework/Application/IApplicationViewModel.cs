using System.Collections.ObjectModel;
using System.Windows.Input;

namespace TestModelDriven.Framework.Application;

public interface IApplicationViewModel : IOneForOneViewModel
{
    ViewModelCollection<IDocument, IDocumentViewModel> Documents { get; }
    IDocumentViewModel? SelectedDocument { get; set; }
    ObservableCollection<MenuItemViewModel> MenuItems { get; }
    ICommand CloseCommand { get; }
}