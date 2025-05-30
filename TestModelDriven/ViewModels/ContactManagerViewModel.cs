using System.Windows.Input;
using TestModelDriven.Framework;
using TestModelDriven.Framework.Application;
using TestModelDriven.Framework.Application.Base;
using TestModelDriven.Framework.UndoRedo;
using TestModelDriven.Models;

namespace TestModelDriven.ViewModels;

public class ContactManagerViewModel : FileDocumentViewModelBase<ContactManager>
{
    private ContactViewModel? _selectedContact;

    public ViewModelCollection<Contact, ContactViewModel> Contacts { get; }
    public ContactViewModel? SelectedContact
    {
        get => _selectedContact;
        set => Model.SelectedContact = value?.Model;
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
        Model.AddContact(new Contact { FirstName = "New", LastName = "Contact" });
    }

    private void Remove()
    {
        ContactViewModel? selectedContact = SelectedContact;
        if (selectedContact is null)
            return;
        
        UndoRedoRecorder.Batch($"Remove contact \"{selectedContact.DisplayName}\"");
        Model.RemoveContact(selectedContact.Model);
    }

    protected override void OnModelPropertyChanged(string? propertyName)
    {
        base.OnModelPropertyChanged(propertyName);

        if (propertyName == nameof(ContactManager.SelectedContact))
            Set(ref _selectedContact, Model.SelectedContact is not null ? Contacts.GetViewModel(Model.SelectedContact) : null, nameof(SelectedContact));
    }

    public override void Present(PresenterSubject subject)
    {
        if (subject.Model is not Contact contact)
            return;

        SelectedContact = Contacts.GetViewModel(contact);
        SelectedContact?.Present(subject);
    }
}