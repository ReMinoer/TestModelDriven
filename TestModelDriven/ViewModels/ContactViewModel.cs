using System.Threading.Tasks;
using TestModelDriven.Framework;
using TestModelDriven.Models;

namespace TestModelDriven.ViewModels;

public class ContactViewModel : OneForOneViewModelBase<Contact>, IPresenter
{
    private string _firstName;
    private string _lastName;
    private string _displayName;

    public string FirstName
    {
        get => _firstName;
        set => PushPropertyTwoWay(
            $"Set first name to {value}",
            () => Model.FirstName,
            value,
            () => Model.SetFirstNameAsync(value),
            () => _firstName,
            x => _firstName = x);
    }

    public string LastName
    {
        get => _lastName;
        set => PushPropertyTwoWay(
            $"Set last name to {value}",
            () => Model.LastName,
            value,
            () => Model.SetLastNameAsync(value),
            () => _lastName,
            x => _lastName = x);
    }

    public string DisplayName => _displayName;

    public ContactViewModel(Contact model)
        : base(model)
    {
        _firstName = model.FirstName;
        _lastName = model.LastName;
        _displayName = ComputeDisplayNameAsync();
    }

    protected override async Task OnModelPropertyChangedAsync(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(Contact.FirstName):
                await SetAsync(ref _firstName, Model.FirstName, nameof(FirstName));
                await RefreshDisplayNameAsync();
                break;
            case nameof(Contact.LastName):
                await SetAsync(ref _lastName, Model.LastName, nameof(LastName));
                await RefreshDisplayNameAsync();
                break;
        }
    }

    private Task RefreshDisplayNameAsync()
    {
        return SetAsync(ref _displayName, ComputeDisplayNameAsync(), nameof(DisplayName));
    }

    private string ComputeDisplayNameAsync()
    {
        return $"{Model.FirstName} {Model.LastName}".Trim();
    }

    public Task<bool> PresentAsync(PresenterSubject subject)
    {
        if (subject.Model != this)
            return Task.FromResult(false);

        switch (subject.PropertyName)
        {
            case nameof(Contact.FirstName):
                //contactViewModel.IsFirstNameFocused = true;
                break;
            case nameof(Contact.LastName):
                //contactViewModel.IsLastNameFocused = true;
                break;
        }

        return Task.FromResult(true);
    }
}