using System.Windows.Input;
using TestModelDriven.Framework;
using TestModelDriven.Framework.UndoRedo;
using TestModelDriven.Models;

namespace TestModelDriven.ViewModels;

public class ContactManagerViewModel : ViewModelBase
{
    public ContactManager Model { get; }
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
    {
        Model = model;
        Contacts = new ViewModelCollection<Contact, ContactViewModel>(Model.Contacts, x => new ContactViewModel(x), x => x.Model);

        AddCommand = new Command(_ => Add());
        RemoveCommand = new Command(_ => Remove());
    }

    private void Add()
    {
        UndoRedoRecorder.Batch("Add new contact");
        Model.Contacts.Add(new Contact
        {
            FirstName = "John",
            LastName = "Doe"
        });
    }

    private void Remove()
    {
        UndoRedoRecorder.Batch("Remove selected contact");
        if (SelectedContact is not null)
        {
            Model.Contacts.Remove(SelectedContact.Model);
        }
    }
}