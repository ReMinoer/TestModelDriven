using System.Collections.Generic;
using System.Collections.ObjectModel;
using TestModelDriven.Framework.Application;
using TestModelDriven.Framework.Application.Base;

namespace TestModelDriven.Models;

public class TestApplication : FileApplicationBase
{
    public override IReadOnlyList<IFileDocumentType> FileDocumentTypes { get; }

    public TestApplication()
    {
        FileDocumentTypes = new ReadOnlyCollection<IFileDocumentType>(new []
        {
            ContactManager.DocumentType
        });
    }
}