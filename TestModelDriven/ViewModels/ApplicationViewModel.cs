using TestModelDriven.Framework;
using TestModelDriven.Framework.UndoRedo;
using TestModelDriven.Models;

namespace TestModelDriven.ViewModels;

public class ApplicationViewModel : ViewModelBase, IPresenter
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
            Presenter = this
        };

        UndoRedoRecorder.Subscribe(ContactManager.Model);
    }

    public void Present(PresenterSubject subject)
    {
        ContactManager.Present(subject);
    }
}