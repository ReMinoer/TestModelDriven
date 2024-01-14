using System.IO;
using System.Linq;
using System.Windows.Input;
using TestModelDriven.Framework;
using TestModelDriven.Framework.UndoRedo;
using TestModelDriven.Models;

namespace TestModelDriven.ViewModels;

public class ContactManagerViewModel : OneForOneViewModelBase<ContactManager>, IPresenter
{
    public UndoRedoStackViewModel UndoRedoStack { get; }
    public UndoRedoRecorder UndoRedoRecorder { get; }
    
    private string? _filePath;
    public string? FilePath
    {
        get => _filePath;
        set
        {
            if (!Set(ref _filePath, value))
                return;

            FileName = Path.GetFileNameWithoutExtension(FilePath);
        }
    }

    private string? _fileName;
    public string? FileName
    {
        get => _fileName;
        private set
        {
            if (!Set(ref _fileName, value))
                return;

            Header = FileName ?? DefaultHeader;
        }
    }

    private const string DefaultHeader = "*New*";
    private string _header = DefaultHeader;
    public string Header
    {
        get => _header;
        private set => Set(ref _header, value);
    }

    public ViewModelCollection<Contact, ContactViewModel> Contacts { get; }

    private ContactViewModel? _selectedContact;
    public ContactViewModel? SelectedContact
    {
        get => _selectedContact;
        set => Set(ref _selectedContact, value);
    }

    public ICommand AddCommand { get; }
    public ICommand RemoveCommand { get; }

    public ContactManagerViewModel(ContactManager model)
        : base(model)
    {
        UndoRedoStack = new UndoRedoStackViewModel();
        UndoRedoRecorder = new UndoRedoRecorder(UndoRedoStack)
        {
            Presenter = this
        };

        UndoRedoRecorder.Subscribe(Model);

        Contacts = new ViewModelCollection<Contact, ContactViewModel>(Model.Contacts, x => new ContactViewModel(x), x => x.Model);

        AddCommand = new Command(_ => Add());
        RemoveCommand = new Command(_ => Remove());
    }

    private void Add()
    {
        UndoRedoRecorder.Batch("Add new contact");
        Model.Contacts.Add(new Contact());
    }

    private void Remove()
    {
        ContactViewModel? selectedContact = SelectedContact;
        if (selectedContact is null)
            return;

        SelectedContact = null;

        UndoRedoRecorder.Batch($"Remove contact \"{selectedContact.DisplayName}\"");
        Model.Contacts.Remove(selectedContact.Model);
    }

    public void Present(PresenterSubject subject)
    {
        if (subject.Model is not Contact contact)
            return;

        SelectedContact = Contacts.FirstOrDefault(x => x.Model == contact);
        SelectedContact?.Present(subject);
    }
}