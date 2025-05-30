using System.Linq;
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

    public TestApplicationViewModel()
        : base(new TestApplication())
    {
        AddCommand = new TargetedCommand<ContactManagerViewModel?>(() => SelectedDocument as ContactManagerViewModel, x => x?.AddCommand);
        RemoveCommand = new TargetedCommand<ContactManagerViewModel?>(() => SelectedDocument as ContactManagerViewModel, x => x?.RemoveCommand);

        MenuItems.Add(new MenuItemViewModel("Add contact", AddCommand));
        MenuItems.Add(new MenuItemViewModel("Remove contact", RemoveCommand));
    }

    public override IDocumentViewModel? CreateViewModel(object model)
    {
        return model switch
        {
            ContactManager contactManager => new ContactManagerViewModel(contactManager),
            _ => null
        };
    }

    public override void Present(PresenterSubject subject)
    {
        if (subject.Model is null)
            return;

        // TODO: Implement model parenting
        ContactManagerViewModel? contactManagerViewModel = Documents.OfType<ContactManagerViewModel>().FirstOrDefault(cm => cm.Contacts.HasViewModel(subject.Model));
        if (contactManagerViewModel is null)
            return;

        Model.SelectedDocument = contactManagerViewModel.Model;
        SelectedDocument?.Present(subject);
    }
}