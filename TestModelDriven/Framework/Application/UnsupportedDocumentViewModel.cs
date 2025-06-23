using System.Threading.Tasks;
using TestModelDriven.Framework.Application.Base;

namespace TestModelDriven.Framework.Application;

public class UnsupportedDocumentViewModel : DocumentViewModelBase<IDocument>
{
    public UnsupportedDocumentViewModel(IDocument document)
        : base(document)
    {
    }

    protected override Task OnModelPropertyChangedAsync(string propertyName) => Task.CompletedTask;
    protected override Task OnIsDirtyChangedAsync() => Task.CompletedTask;
    public override Task<bool> PresentAsync(PresenterSubject subject) => Task.FromResult(false);
}