using System.Windows.Input;
using TestModelDriven.Framework;
using TestModelDriven.Framework.UndoRedo;
using TestModelDriven.Models;

namespace TestModelDriven.ViewModels;

public class ContactManagerViewModel : OneForOneViewModel<ContactManager>
{
    public ViewModelCollection<Contact, ContactViewModel> Contacts { get; }

    private ContactViewModel? _selectedContact;
    public ContactViewModel? SelectedContact
    {
        get => _selectedContact;
        set => Set(ref _selectedContact, value);
    }

    public ICommand AddCommand { get; }
    public ICommand RemoveCommand { get; }

    public ContactManagerViewModel(ContactManager model)
        : base(model)
    {
        Contacts = new ViewModelCollection<Contact, ContactViewModel>(Model.Contacts, x => new ContactViewModel(x), x => x.Model);

        AddCommand = new Command(_ => Add());
        RemoveCommand = new Command(_ => Remove());
    }

    private void Add()
    {
        UndoRedoRecorder.Batch("Add new contact");
        Model.Contacts.Add(new Contact());
    }

    private void Remove()
    {
        ContactViewModel? selectedContact = SelectedContact;
        if (selectedContact is null)
            return;

        SelectedContact = null;

        UndoRedoRecorder.Batch($"Remove contact \"{selectedContact.DisplayName}\"");
        Model.Contacts.Remove(selectedContact.Model);
    }
}