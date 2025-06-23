using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TestModelDriven.Framework;
using TestModelDriven.Framework.Application;
using TestModelDriven.Framework.Application.Base;
using TestModelDriven.Models;

namespace TestModelDriven.ViewModels;

public class TestApplicationViewModel : FileApplicationViewModelBase<TestApplication>
{
    public ICommand AddCommand { get; }
    public ICommand RemoveCommand { get; }
    public ICommand PublishCommand { get; }

    public TestApplicationViewModel()
        : base(new TestApplication())
    {
        AddCommand = new TargetedCommand<ContactManagerViewModel?>(() => SelectedDocument as ContactManagerViewModel, x => x?.AddCommand);
        RemoveCommand = new TargetedCommand<ContactManagerViewModel?>(() => SelectedDocument as ContactManagerViewModel, x => x?.RemoveCommand);
        PublishCommand = new TargetedCommand<ContactManagerViewModel?>(() => SelectedDocument as ContactManagerViewModel, x => x?.PublishCommand);

        MenuItems.Add(new MenuItemViewModel("Add contact", AddCommand));
        MenuItems.Add(new MenuItemViewModel("Remove contact", RemoveCommand));
        MenuItems.Add(new MenuItemViewModel("Publish", PublishCommand));
    }

    public override Task<IDocumentViewModel?> CreateViewModelAsync(object model)
    {
        return Task.FromResult<IDocumentViewModel?>(model switch
        {
            ContactManager contactManager => new ContactManagerViewModel(contactManager),
            _ => null
        });
    }

    public override async Task<bool> PresentAsync(PresenterSubject subject)
    {
        if (subject.Model is null)
            return false;

        // TODO: Implement model parenting
        ContactManagerViewModel? contactManagerViewModel = Documents.OfType<ContactManagerViewModel>().FirstOrDefault(cm => cm.Contacts.HasViewModel(subject.Model));
        if (contactManagerViewModel is null)
            return false;

        await Model.SetSelectedDocumentAsync(contactManagerViewModel.Model);
        if (SelectedDocument is not null)
            await SelectedDocument.PresentAsync(subject);

        return true;
    }
}