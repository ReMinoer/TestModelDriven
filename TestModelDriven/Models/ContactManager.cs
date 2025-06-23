using System.Reflection;
using System.Threading.Tasks;
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

    [StateCollection]
    public AsyncList<Contact> Contacts { get; } = new();

    private Contact? _selectedContact;
    public Contact? SelectedContact => _selectedContact;
    public Task SetSelectedContactAsync(Contact? value) => SetAsync(ref _selectedContact, value, nameof(SelectedContact));

    public ContactManager()
        : base(DocumentType)
    {
    }

    public async Task AddContactAsync(Contact contact)
    {
        await Contacts.AddAsync(contact);
        await SetSelectedContactAsync(contact);
    }

    public async Task RemoveContactAsync(Contact contact)
    {
        if (SelectedContact == contact)
            await SetSelectedContactAsync(null);

        await Contacts.RemoveAsync(contact);
    }

    public override ContactManagerData ToData()
    {
        var data = new ContactManagerData();

        foreach (Contact contact in Contacts)
            data.Contacts.Add(contact.ToData());

        return data;
    }
}