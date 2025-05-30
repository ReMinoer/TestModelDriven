using System;
using TestModelDriven.Framework.Application.Base;

namespace TestModelDriven.Framework.Application;

public class UnsupportedDocumentViewModel : DocumentViewModelBase<IDocument>
{
    public UnsupportedDocumentViewModel(IDocument document)
        : base(document)
    {

    }

    protected override void OnIsDirtyChanged(object? sender, EventArgs e) {}
    public override void Present(PresenterSubject subject) {}
}