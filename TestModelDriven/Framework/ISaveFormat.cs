using System.IO;

namespace TestModelDriven.Framework;

public interface ISaveFormat
{
    void Save(object data, Stream stream);
}