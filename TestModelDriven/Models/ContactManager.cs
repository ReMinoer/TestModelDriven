using System.Collections.ObjectModel;
using System.Reflection;
using TestModelDriven.Data;
using TestModelDriven.Framework;
using TestModelDriven.Framework.Application;
using TestModelDriven.Framework.Application.Base;

namespace TestModelDriven.Models;

public class ContactManager : FileDocumentBase<ContactManagerData>
{
    static public readonly IFileDocumentType DocumentType = new FileDocumentType<ContactManagerData, ContactManager>(
        "Contact Manager",
        new SingleFileType(
            "Contact Manager",
            new[] { "contactmanager", "cm" },
            new DataContractFormat<ContactManagerData>(Assembly.GetExecutingAssembly().GetTypes()))
    );

    [State]
    public ObservableCollection<Contact> Contacts { get; } = new();

    private Contact? _selectedContact;
    public Contact? SelectedContact
    {
        get => _selectedContact;
        set => Set(ref _selectedContact, value);
    }

    public ContactManager()
        : base(DocumentType)
    {
    }

    public void AddContact(Contact contact)
    {
        Contacts.Add(contact);
        SelectedContact = contact;
    }

    public void RemoveContact(Contact contact)
    {
        if (SelectedContact == contact)
            SelectedContact = null;

        Contacts.Remove(contact);
    }

    public override ContactManagerData ToData()
    {
        var data = new ContactManagerData();

        foreach (Contact contact in Contacts)
            data.Contacts.Add(contact.ToData());

        return data;
    }
}