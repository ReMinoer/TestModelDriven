using TestModelDriven.Framework;
using TestModelDriven.Framework.UndoRedo;
using TestModelDriven.Models;

namespace TestModelDriven.ViewModels;

public class ContactViewModel : OneForOneViewModelBase<Contact>, IPresenter
{
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

    public string DisplayName => !string.IsNullOrWhiteSpace(Model.FirstName) || !string.IsNullOrWhiteSpace(Model.LastName)
        ? $"{Model.FirstName} {Model.LastName}"
        : "*Anonymous*";

    public ContactViewModel(Contact model)
        : base(model)
    {
    }

    protected override void OnModelPropertyChanged(string? propertyName)
    {
        base.OnModelPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(Contact.FirstName):
                RaisePropertyChanged(nameof(FirstName));
                RaisePropertyChanged(nameof(DisplayName));
                break;
            case nameof(Contact.LastName):
                RaisePropertyChanged(nameof(LastName));
                RaisePropertyChanged(nameof(DisplayName));
                break;
        }
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