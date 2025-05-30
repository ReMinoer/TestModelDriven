using System.IO;

namespace TestModelDriven.Framework;

public interface ILoadFormat
{
    object? Load(Stream stream);
}