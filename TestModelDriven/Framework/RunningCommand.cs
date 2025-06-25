using System;
using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public class RunningCommand : PropertyChangeNotifierBase
{
    private bool _isRunning;
    private bool _isCompleted;
    private bool _isCancelled;

    public string Description { get; }
    public bool IsRunning => _isRunning;
    public bool IsCompleted => _isCompleted;
    public bool IsCancelled => _isCancelled;
    public bool IsCompletedOrCancelled => IsCompleted || IsCancelled;

    public RunningCommand(string description)
    {
        Description = description;
    }

    public async Task BeginAsync()
    {
        if (IsRunning)
            return;
        if (IsCompleted)
            throw new InvalidOperationException("Already completed");
        if (IsCancelled)
            throw new InvalidOperationException("Already cancelled");

        _isRunning = true;
        await RaisePropertyChangeAsync(!IsRunning, IsRunning, nameof(IsRunning));
    }

    public async Task CompleteAsync()
    {
        if (IsCompleted)
            return;
        if (!IsRunning)
            throw new InvalidOperationException("Not running");
        if (IsCancelled)
            throw new InvalidOperationException("Already cancelled");

        _isRunning = false;
        _isCompleted = true;

        await RaisePropertyChangeAsync(!IsRunning, IsRunning, nameof(IsRunning));
        await RaisePropertyChangeAsync(!IsCompleted, IsCompleted, nameof(IsCompleted));
        await RaisePropertyChangeAsync(!IsCompletedOrCancelled, IsCompletedOrCancelled, nameof(IsCompletedOrCancelled));
    }

    public async Task CancelAsync()
    {
        if (IsCancelled)
            return;
        if (IsRunning)
            throw new InvalidOperationException("Already running");
        if (IsCompleted)
            throw new InvalidOperationException("Already completed");

        _isRunning = false;
        _isCancelled = true;

        await RaisePropertyChangeAsync(!IsRunning, IsRunning, nameof(IsRunning));
        await RaisePropertyChangeAsync(!IsCancelled, IsCancelled, nameof(IsCancelled));
        await RaisePropertyChangeAsync(!IsCompletedOrCancelled, IsCompletedOrCancelled, nameof(IsCompletedOrCancelled));
    }
}