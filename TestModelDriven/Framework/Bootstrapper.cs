using System.Windows.Threading;

namespace TestModelDriven.Framework;

public class Bootstrapper
{
    public Bootstrapper()
    {
        CommandDispatcher.Init(Dispatcher.CurrentDispatcher);
    }
}