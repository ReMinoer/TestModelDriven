using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        NewCommand = new CommandDispatcherCommand<IFileDocumentType>("New document", NewAsync);
        OpenCommand = new CommandDispatcherCommand("Open file", OpenAsync);
        SaveCommand = new CommandDispatcherCommand("Save document", () => SaveAsync(), CanSave);
        SaveAsCommand = new CommandDispatcherCommand("Save document as new file", SaveAsAsync, CanSaveAs);
        UndoCommand = new CommandDispatcherCommand("Undo", UndoAsync, CanUndo);
        RedoCommand = new CommandDispatcherCommand("Redo", RedoAsync, CanRedo);

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
    
    private async Task NewAsync(IFileDocumentType documentType)
    {
        await Model.NewDocumentAsync(documentType, CancellationToken.None);
    }

    private async Task OpenAsync()
    {
        var openFileDialog = new CommonOpenFileDialog();

        foreach (IFileType fileType in Model.FileDocumentTypes.SelectMany(x => x.FileTypes))
        {
            openFileDialog.Filters.Add(fileType.GetFileDialogFilter());
        }

        CommonFileDialogResult dialogResult = openFileDialog.ShowDialog();
        if (dialogResult != CommonFileDialogResult.Ok)
            return;

        await Model.OpenFileAsync(openFileDialog.FileName, CancellationToken.None);
    }

    private bool CanSave()
    {
        return SelectedDocument is not null
            && SelectedDocument.UndoRedoStack.Model.IsDirty
            && SelectedDocument.Model is IFileDocument fileDocument
            && fileDocument.FileDocumentType.CanSaveDocument(fileDocument);
    }

    private async Task SaveAsync(bool saveAs = false)
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

            await fileDocument.SetFilePathAsync(saveFileDialog.FileName);
        }

        await Model.SaveDocumentAsync(fileDocument, CancellationToken.None);
        await SelectedDocument.UndoRedoStack.Model.SaveCurrentIndexAsync();
    }

    private bool CanSaveAs() => CanSave();
    private Task SaveAsAsync() => SaveAsync(saveAs: true);

    private bool CanUndo() => SelectedDocument?.UndoRedoStack.Model.CanUndo ?? false;
    private bool CanRedo() => SelectedDocument?.UndoRedoStack.Model.CanRedo ?? false;

    private async Task UndoAsync()
    {
        if (SelectedDocument is null)
            return;

        await SelectedDocument.UndoRedoStack.Model.UndoAsync();
    }

    private async Task RedoAsync()
    {
        if (SelectedDocument is null)
            return;

        await SelectedDocument.UndoRedoStack.Model.RedoAsync();
    }
}