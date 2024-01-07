using System.ComponentModel;
using TestModelDriven.Framework;
using TestModelDriven.Framework.UndoRedo;
using TestModelDriven.Models;

namespace TestModelDriven.ViewModels;

public class ContactViewModel : ViewModelBase
{
    public Contact Model { get; }

    public string FirstName
    {
        get => Model.FirstName;
        set
        {
            using var _ = UndoRedo.SetDescription($"Set first name to {value}");
            Model.FirstName = value;
        }
    }

    public string LastName
    {
        get => Model.LastName;
        set
        {
            using var _ = UndoRedo.SetDescription($"Set last name to {value}");
            Model.LastName = value;
        }
    }

    public string DisplayName => $"{Model.FirstName} {Model.LastName}";

    public ContactViewModel(Contact model)
    {
        Model = model;
    }

    public override void OnLoaded()
    {
        base.OnLoaded();
        Model.PropertyChanged += OnPropertyChanged;
    }

    public override void OnUnloaded()
    {
        Model.PropertyChanged -= OnPropertyChanged;
        base.OnUnloaded();
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
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
}