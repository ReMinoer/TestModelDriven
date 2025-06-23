using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace TestModelDriven.Framework;

public class DataContractFormat<TData> : ISaveLoadFormat
    where TData : class
{
    private readonly DataContractSerializer _serializer;

    public DataContractFormat(IEnumerable<Type> knownTypes)
    {
        _serializer = new DataContractSerializer(typeof(TData), knownTypes);
    }

    public async Task SaveAsync(object data, Stream stream, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await using var xmlTextWriter = new XmlTextWriter(stream, Encoding.Default);
        xmlTextWriter.Formatting = Formatting.Indented;

        await Task.Run(() => _serializer.WriteObject(xmlTextWriter, data), cancellationToken);
    }

    public async Task<object?> LoadAsync(Stream stream, CancellationToken cancellationToken)
    {
        return await Task.Run(() => _serializer.ReadObject(stream) as TData, cancellationToken);
    }
}