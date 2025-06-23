using System.Threading.Tasks;

namespace TestModelDriven.Framework.Application.Base;

public class Application : ModelBase
{
    [StateCollection]
    public AsyncList<IDocument> Documents { get; }

    private IDocument? _selectedDocument;
    [State(nameof(SetSelectedDocumentAsync))]
    public IDocument? SelectedDocument => _selectedDocument;
    public Task SetSelectedDocumentAsync(IDocument? value) => SetAsync(ref _selectedDocument, value, nameof(SelectedDocument));

    public Application()
    {
        Documents = new AsyncList<IDocument>();
    }

    public async Task AddDocumentAsync(IDocument document)
    {
        await Documents.AddAsync(document);
        await SetSelectedDocumentAsync(document);
    }

    public async Task CloseDocumentAsync(IDocument document)
    {
        if (SelectedDocument == document)
            await SetSelectedDocumentAsync(null);

        await Documents.RemoveAsync(document);
    }
}