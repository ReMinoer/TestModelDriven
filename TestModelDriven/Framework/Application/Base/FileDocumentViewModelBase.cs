using System;
using System.ComponentModel;
using System.IO;

namespace TestModelDriven.Framework.Application.Base;

public abstract class FileDocumentViewModelBase<TModel> : DocumentViewModelBase<TModel>
    where TModel : IFileDocument, INotifyPropertyChanged
{
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

            RefreshHeader();
        }
    }

    public FileDocumentViewModelBase(TModel model)
        : base(model)
    {
        FilePath = Model.FilePath;
        RefreshHeader();
    }

    protected override void OnModelPropertyChanged(string? propertyName)
    {
        base.OnModelPropertyChanged(propertyName);

        if (propertyName == nameof(IFileDocument.FilePath))
            FilePath = Model.FilePath;
    }

    protected override void OnIsDirtyChanged(object? sender, EventArgs e) => RefreshHeader();
    private void RefreshHeader()
    {
        Header = $"{FileName ?? "New"}{(UndoRedoStack.IsDirty ? "*" : "")}";
    }
}