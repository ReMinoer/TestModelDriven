using System.Threading.Tasks;

namespace TestModelDriven.Framework.Application;

public interface IFileDocument : IDocument, IPersistentModel
{
    string? FilePath { get; }
    Task SetFilePathAsync(string? value);
    IFileDocumentType FileDocumentType { get; }
}

public interface IFileDocument<out TData> : IFileDocument, IPersistentModel<TData>
    where TData : IData
{
}