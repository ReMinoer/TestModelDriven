using TestModelDriven.Framework;
using TestModelDriven.Framework.UndoRedo;
using TestModelDriven.Models;

namespace TestModelDriven.ViewModels;

public class ContactViewModel : OneForOneViewModelBase<Contact>, IPresenter
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _displayName = string.Empty;

    public string FirstName
    {
        get => Model.FirstName;
        set
        {
            UndoRedoRecorder.Batch($"Set first name to {value}");
            Model.FirstName = value;
        }
    }

    public string LastName
    {
        get => Model.LastName;
        set
        {
            UndoRedoRecorder.Batch($"Set last name to {value}");
            Model.LastName = value;
        }
    }

    public string DisplayName => $"{Model.FirstName} {Model.LastName}".Trim();

    public ContactViewModel(Contact model)
        : base(model)
    {
    }

    protected override void OnModelPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(Contact.FirstName):
                Set(ref _firstName, Model.FirstName, nameof(FirstName));
                RefreshDisplayName();
                break;
            case nameof(Contact.LastName):
                Set(ref _lastName, Model.LastName, nameof(LastName));
                RefreshDisplayName();
                break;
        }
    }

    private void RefreshDisplayName()
    {
        Set(ref _displayName, $"{Model.FirstName} {Model.LastName}".Trim(), nameof(DisplayName));
    }
    
    public void Present(PresenterSubject subject)
    {
        if (subject.Model != this)
            return;

        switch (subject.PropertyName)
        {
            case nameof(Contact.FirstName):
                //contactViewModel.IsFirstNameFocused = true;
                break;
            case nameof(Contact.LastName):
                //contactViewModel.IsLastNameFocused = true;
                break;
        }
    }
}