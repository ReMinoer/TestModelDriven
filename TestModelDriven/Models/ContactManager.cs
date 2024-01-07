using System.Collections.ObjectModel;
using TestModelDriven.Framework;

namespace TestModelDriven.Models;

public class ContactManager : ModelBase
{
    [State]
    public ObservableCollection<Contact> Contacts { get; } = new();
}