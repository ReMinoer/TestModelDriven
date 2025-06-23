using System;
using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public interface ICommandDispatcher
{
    IAsyncEvent<RunningCommand> CommandPushed { get; }
    void Push(string description, Func<Task> executeTask, Func<bool> canExecute);
}