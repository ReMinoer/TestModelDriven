using System;
using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public class CommandDispatcherWatcher : ModelBase, IDisposable
{
    public AsyncList<RunningCommand> Commands { get; } = new();
    private readonly IDisposable _commandPushes;

    private int? _currentCommandIndex;
    private RunningCommand? _currentCommand;
    private IDisposable? _currentCommandChanges;

    public RunningCommand? CurrentCommand => _currentCommand;
    private async Task RefreshCurrentCommandAsync(RunningCommand? value) => await SetAsync(ref _currentCommand, value, nameof(CurrentCommand));

    public CommandDispatcherWatcher(ICommandDispatcher commandDispatcher)
    {
        _commandPushes = commandDispatcher.CommandPushed.Subscribe(OnCommandChanged);
    }

    public void Dispose()
    {
        _currentCommandChanges?.Dispose();
        _commandPushes.Dispose();
    }

    private async Task OnCommandChanged(RunningCommand runningCommand)
    {
        await Commands.AddAsync(runningCommand);
        await FindCurrentCommandAsync();
    }

    private async Task FindCurrentCommandAsync()
    {
        int currentCommandIndex = -1;
        for (int i = _currentCommandIndex ?? 0; i < Commands.Count; i++)
        {
            RunningCommand command = Commands[i];
            if (!command.IsCompletedOrCancelled)
            {
                currentCommandIndex = i;
                break;
            }
        }

        if (_currentCommandIndex == currentCommandIndex)
            return;

        if (currentCommandIndex == -1)
        {
            await RefreshCurrentCommandAsync(null);
            return;
        }

        _currentCommandIndex = currentCommandIndex;
        
        RunningCommand currentCommand = Commands[currentCommandIndex];
        _currentCommandChanges = currentCommand.PropertyChangedAsync.Subscribe(OnCurrentCommandPropertyChanged);

        await RefreshCurrentCommandAsync(currentCommand);
    }

    private async Task OnCurrentCommandPropertyChanged(PropertyChange change)
    {
        var currentCommand = (RunningCommand)change.Owner;
        if (!currentCommand.IsCompletedOrCancelled)
            return;

        _currentCommandChanges!.Dispose();
        await FindCurrentCommandAsync();
    }
}