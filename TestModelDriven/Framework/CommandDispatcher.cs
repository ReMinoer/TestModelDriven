using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using Nito.AsyncEx;
using TestModelDriven.Framework.UndoRedo;

namespace TestModelDriven.Framework;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly Dispatcher _dispatcher;
    private readonly AsyncLock _asyncLock = new AsyncLock();

    private readonly AsyncEvent<RunningCommand> _commandChanged = new();
    public IAsyncEvent<RunningCommand> CommandPushed => _commandChanged.Public;

    static private CommandDispatcher? _current;
    static public ICommandDispatcher Current => _current ?? throw new InvalidOperationException("CommandDispatcher not initialized.");

    static public void Init(Dispatcher dispatcher)
    {
        _current = new CommandDispatcher(dispatcher);
    }

    private CommandDispatcher(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    public void Push(string description, Func<Task> executeTask, Func<bool> canExecute) => PushAsync(description, executeTask, canExecute).CaptureThrow();
    
    private async Task PushAsync(string description, Func<Task> executeTask, Func<bool> canExecute)
    {
        if (!_dispatcher.CheckAccess())
            throw new InvalidOperationException("Command must be pushed from the correct dispatcher.");

        var runningCommand = new RunningCommand(description);
        await _commandChanged.RaiseAsync(runningCommand);

        using var _ = await _asyncLock.LockAsync();

        if (!canExecute())
        {
            runningCommand.IsCancelled = true;
            return;
        }

        UndoRedoRecorder.Batch(description);
        await executeTask();
        runningCommand.IsCompleted = true;
    }
}