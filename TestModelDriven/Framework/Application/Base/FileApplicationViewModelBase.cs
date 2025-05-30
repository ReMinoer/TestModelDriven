using System.Linq;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace TestModelDriven.Framework.Application.Base;

public abstract class FileApplicationViewModelBase<TApplication> : ApplicationViewModelBase<TApplication>
    where TApplication : FileApplicationBase
{
    public ICommand NewCommand { get; }
    public ICommand OpenCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand SaveAsCommand { get; }
    public ICommand UndoCommand { get; }
    public ICommand RedoCommand { get; }

    protected FileApplicationViewModelBase(TApplication application)
        : base(application)
    {
        NewCommand = new Command<IFileDocumentType>(New);
        OpenCommand = new Command(_ => Open());
        SaveCommand = new Command(_ => CanSave(), _ => Save());
        SaveAsCommand = new Command(_ => CanSaveAs(), _ => SaveAs());
        UndoCommand = new Command(_ => CanUndo(), _ => Undo());
        RedoCommand = new Command(_ => CanRedo(), _ => Redo());

        var newMenu = new MenuItemViewModel("New", NewCommand);
        foreach (IFileDocumentType fileDocumentType in Model.FileDocumentTypes)
        {
            newMenu.SubItems.Add(new MenuItemViewModel(fileDocumentType.DisplayName, NewCommand, fileDocumentType));
        }
        
        MenuItems.Add(new MenuItemViewModel("File")
        {
            SubItems =
            {
                newMenu,
                new MenuItemViewModel("Open...", OpenCommand),
                new MenuItemViewModel("Save", SaveCommand),
                new MenuItemViewModel("Save As...", SaveAsCommand),
            }
        });
        MenuItems.Add(new MenuItemViewModel("Edit")
        {
            SubItems =
            {
                new MenuItemViewModel("Undo", UndoCommand),
                new MenuItemViewModel("Redo", RedoCommand)
            }
        });
    }
    
    private void New(IFileDocumentType documentType)
    {
        Model.NewDocument(documentType);
    }

    private void Open()
    {
        var openFileDialog = new CommonOpenFileDialog();

        foreach (IFileType fileType in Model.FileDocumentTypes.SelectMany(x => x.FileTypes))
        {
            openFileDialog.Filters.Add(fileType.GetFileDialogFilter());
        }

        CommonFileDialogResult dialogResult = openFileDialog.ShowDialog();
        if (dialogResult != CommonFileDialogResult.Ok)
            return;
        
        Model.OpenFile(openFileDialog.FileName);
    }

    private bool CanSave()
    {
        return SelectedDocument is not null
            && SelectedDocument.UndoRedoStack.IsDirty
            && SelectedDocument.Model is IFileDocument fileDocument
            && fileDocument.FileDocumentType.CanSaveDocument(fileDocument);
    }

    private void Save(bool saveAs = false)
    {
        if (SelectedDocument?.Model is not IFileDocument fileDocument || !CanSave())
            return;

        if (saveAs || fileDocument.FilePath is null)
        {
            var saveFileDialog = new CommonSaveFileDialog
            {
                DefaultFileName = fileDocument.FilePath
            };

            foreach (IFileType fileType in fileDocument.FileDocumentType.FileTypes)
            {
                saveFileDialog.Filters.Add(fileType.GetFileDialogFilter());
            }

            CommonFileDialogResult dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult != CommonFileDialogResult.Ok)
                return;

            fileDocument.FilePath = saveFileDialog.FileName;
        }

        Model.SaveDocument(fileDocument);
        SelectedDocument.UndoRedoStack.SaveCurrentIndex();
    }

    private bool CanSaveAs() => CanSave();
    private void SaveAs() => Save(saveAs: true);

    private bool CanUndo() => SelectedDocument?.UndoRedoStack.CanUndo ?? false;
    private bool CanRedo() => SelectedDocument?.UndoRedoStack.CanRedo ?? false;
    private void Undo() => SelectedDocument?.UndoRedoStack.Undo();
    private void Redo() => SelectedDocument?.UndoRedoStack.Redo();
}