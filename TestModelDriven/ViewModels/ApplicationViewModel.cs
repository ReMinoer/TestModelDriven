using System.Linq;
using TestModelDriven.Framework;
using TestModelDriven.Framework.UndoRedo;
using TestModelDriven.Models;

namespace TestModelDriven.ViewModels;

public class ApplicationViewModel : ViewModelBase, IFocuser
{
    public UndoRedoStackViewModel UndoRedoStack { get; }
    public UndoRedoRecorder UndoRedoRecorder { get; }

    public ContactManagerViewModel ContactManager { get; }

    public ApplicationViewModel()
    {
        ContactManager = new ContactManagerViewModel(new ContactManager());

        UndoRedoStack = new UndoRedoStackViewModel();
        UndoRedoRecorder = new UndoRedoRecorder(UndoRedoStack)
        {
            Focuser = this
        };

        UndoRedoRecorder.Subscribe(ContactManager.Model);
    }

    public void Focus(object? model, string? propertyName = null)
    {
        switch (model)
        {
            case Contact contact:
                var contactViewModel = ContactManager.Contacts.First(x => x.Model == contact);
                ContactManager.SelectedContact = contactViewModel;

                switch (propertyName)
                {
                    case nameof(Contact.FirstName):
                        //contactViewModel.IsFirstNameFocused = true;
                        break;
                    case nameof(Contact.LastName):
                        //contactViewModel.IsLastNameFocused = true;
                        break;
                }
                break;
        }
    }
}