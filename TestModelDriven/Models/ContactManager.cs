using System.Collections.ObjectModel;
using TestModelDriven.Data;
using TestModelDriven.Framework;

namespace TestModelDriven.Models;

public class ContactManager : PersistentModelBase<ContactManagerData>
{
    [State]
    public ObservableCollection<Contact> Contacts { get; } = new();

    public override ContactManagerData ToData()
    {
        var data = new ContactManagerData();

        foreach (Contact contact in Contacts)
            data.Contacts.Add(contact.ToData());

        return data;
    }
}