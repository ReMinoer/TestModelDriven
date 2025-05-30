using System.Collections.ObjectModel;

namespace TestModelDriven.Framework.Application.Base;

public class Application : ModelBase
{
    [State]
    public ObservableCollection<IDocument> Documents { get; }

    private IDocument? _selectedDocument;
    public IDocument? SelectedDocument
    {
        get => _selectedDocument;
        set => Set(ref _selectedDocument, value);
    }

    public Application()
    {
        Documents = new ObservableCollection<IDocument>();
    }

    public void AddDocument(IDocument document)
    {
        Documents.Add(document);
        SelectedDocument = document;
    }

    public void CloseDocument(IDocument document)
    {
        if (SelectedDocument == document)
            SelectedDocument = null;

        Documents.Remove(document);
    }
}