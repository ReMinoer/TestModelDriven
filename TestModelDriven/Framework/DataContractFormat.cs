using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
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

    public void Save(object data, Stream stream)
    {
        using var xmlTextWriter = new XmlTextWriter(stream, Encoding.Default);
        xmlTextWriter.Formatting = Formatting.Indented;

        _serializer.WriteObject(xmlTextWriter, data);
    }

    public object? Load(Stream stream)
    {
        return _serializer.ReadObject(stream) as TData;
    }
}