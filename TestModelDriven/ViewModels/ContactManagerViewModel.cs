using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TestModelDriven.Framework;
using TestModelDriven.Framework.Application.Base;
using TestModelDriven.Models;

namespace TestModelDriven.ViewModels;

public class ContactManagerViewModel : FileDocumentViewModelBase<ContactManager>
{
    public ViewModelCollection<Contact, ContactViewModel> Contacts { get; }

    private ContactViewModel? _selectedContact;
    public ContactViewModel? SelectedContact
    {
        get => _selectedContact;
        set => PushPropertyTwoWay(
            $"Select contact {value?.DisplayName}",
            () => Model.SelectedContact,
            value?.Model,
            () => Model.SetSelectedContactAsync(value?.Model),
            () => _selectedContact?.Model,
            x => _selectedContact = x is not null ? Contacts.GetViewModel(x) : null);
    }

    public ICommand AddCommand { get; }
    public ICommand RemoveCommand { get; }
    public ICommand PublishCommand { get; }

    public ContactManagerViewModel(ContactManager model)
        : base(model)
    {
        Contacts = new ViewModelCollection<Contact, ContactViewModel>(Model.Contacts, ViewModelFactoryAsync, x => x.Model);

        AddCommand = new CommandDispatcherCommand("Add new contact", AddAsync);
        RemoveCommand = new CommandDispatcherCommand(() => $"Remove contact \"{SelectedContact!.DisplayName}\"", RemoveAsync, CanRemove);
        PublishCommand = new CommandDispatcherCommand("Publish", async () => await Task.Delay(5000));
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await Contacts.InitializeAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        Contacts.Dispose();
        await base.DisposeAsync();
    }

    static private async Task<ContactViewModel> ViewModelFactoryAsync(Contact x)
    {
        var viewModel = new ContactViewModel(x);
        await viewModel.InitializeAsync();
        return viewModel;
    }

    private async Task AddAsync()
    {
        var contact = new Contact();
        await contact.SetFirstNameAsync("New");
        await contact.SetLastNameAsync("Contact");

        await Model.AddContactAsync(contact);
    }

    private bool CanRemove() => SelectedContact is not null && Contacts.Contains(SelectedContact);
    private async Task RemoveAsync()
    {
        await Model.RemoveContactAsync(SelectedContact!.Model);
    }

    protected override async Task OnModelPropertyChangedAsync(string propertyName)
    {
        await base.OnModelPropertyChangedAsync(propertyName);

        if (propertyName == nameof(ContactManager.SelectedContact))
            await SetAsync(ref _selectedContact, Contacts.GetViewModel(Model.SelectedContact), nameof(SelectedContact));
    }

    public override async Task<bool> PresentAsync(PresenterSubject subject)
    {
        if (subject.Model is not Contact contact)
            return false;

        await Model.SetSelectedContactAsync(contact);
        if (SelectedContact is not null)
            await SelectedContact.PresentAsync(subject);

        return true;
    }
}