using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using Microsoft.WindowsAPICodePack.Dialogs;
using TestModelDriven.Data;
using TestModelDriven.Framework;
using TestModelDriven.Models;

namespace TestModelDriven.ViewModels;

public class ApplicationViewModel : ViewModelBase, IPresenter
{
    private readonly DataContractSerializer _serializer;

    public ObservableCollection<ContactManagerViewModel> ContactManagers { get; }

    private ContactManagerViewModel? _selectedContactManager;
    public ContactManagerViewModel? SelectedContactManager
    {
        get => _selectedContactManager;
        set => Set(ref _selectedContactManager, value);
    }

    public ICommand NewCommand { get; }
    public ICommand OpenCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand SaveAsCommand { get; }
    public ICommand CloseCommand { get; }

    public ApplicationViewModel()
    {
        _serializer = new DataContractSerializer(typeof(ContactManagerData), Assembly.GetExecutingAssembly().GetTypes());

        ContactManagers = new ObservableCollection<ContactManagerViewModel>();

        NewCommand = new Command(_ => New());
        OpenCommand = new Command(_ => Open());
        SaveCommand = new Command(_ => CanSave(), _ => Save());
        SaveAsCommand = new Command(_ => CanSaveAs(), _ => SaveAs());
        CloseCommand = new Command(Close);
    }

    private void New()
    {
        var contactManagerViewModel = new ContactManagerViewModel(new ContactManager());
        ContactManagers.Add(contactManagerViewModel);
        SelectedContactManager = contactManagerViewModel;
    }

    private void Open()
    {
        var openFileDialog = new CommonOpenFileDialog
        {
            Filters = { new CommonFileDialogFilter("Contact Manager", ".contactmanager") }
        };

        CommonFileDialogResult dialogResult = openFileDialog.ShowDialog();
        if (dialogResult != CommonFileDialogResult.Ok)
            return;

        using FileStream fileStream = File.OpenRead(openFileDialog.FileName);

        if (_serializer.ReadObject(fileStream) is not ContactManagerData data)
        {
            MessageBox.Show($"Loaded data is not a {nameof(ContactManagerData)}.", "File loading error");
            return;
        }

        var contactManagerViewModel = new ContactManagerViewModel(data.ToModel())
        {
            FilePath = openFileDialog.FileName
        };

        ContactManagers.Add(contactManagerViewModel);
        SelectedContactManager = contactManagerViewModel;
    }

    private bool CanSave() => SelectedContactManager?.UndoRedoStack.IsDirty ?? false;
    private void Save(bool saveAs = false)
    {
        if (SelectedContactManager is null || !CanSave())
            return;

        if (saveAs || SelectedContactManager.FilePath is null)
        {
            var saveFileDialog = new CommonSaveFileDialog
            {
                DefaultFileName = SelectedContactManager.FilePath,
                Filters = { new CommonFileDialogFilter("Contact Manager", ".contactmanager") }
            };

            CommonFileDialogResult dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult != CommonFileDialogResult.Ok)
                return;

            SelectedContactManager.FilePath = saveFileDialog.FileName;
        }

        using FileStream fileStream = File.Create(SelectedContactManager.FilePath);
        using var xmlTextWriter = new XmlTextWriter(fileStream, Encoding.Default);
        xmlTextWriter.Formatting = Formatting.Indented;

        _serializer.WriteObject(xmlTextWriter, SelectedContactManager.Model.ToData());

        SelectedContactManager.UndoRedoStack.SaveCurrentIndex();
    }

    private bool CanSaveAs() => SelectedContactManager is not null;
    private void SaveAs() => Save(saveAs: true);

    private void Close(object? tabViewModel)
    {
        if (tabViewModel is not ContactManagerViewModel contactManager)
            return;

        if (SelectedContactManager == contactManager)
            SelectedContactManager = null;

        ContactManagers.Remove(contactManager);
    }

    public void Present(PresenterSubject subject)
    {
        // TODO: Implement model parenting
        ContactManagerViewModel? contactManagerViewModel = ContactManagers.FirstOrDefault(cm => cm.Contacts.Any(x => x.Model == subject.Model));
        if (contactManagerViewModel is null)
            return;

        SelectedContactManager = contactManagerViewModel;
        SelectedContactManager.Present(subject);
    }
}