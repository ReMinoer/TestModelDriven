using System.ComponentModel;

namespace TestModelDriven.Framework.Application;

public interface IDocument : INotifyStateChanged, INotifyPropertyChanged
{
    string Header { get; }
}