namespace TestModelDriven.Framework;

public class RunningCommand : PropertyChangeNotifierBase
{
    private bool _isCompleted;
    private bool _isCancelled;

    public string Description { get; }

    public bool IsCompleted
    {
        get => _isCompleted;
        set => SetProperty(ref _isCompleted, value);
    }

    public bool IsCancelled
    {
        get => _isCancelled;
        set => SetProperty(ref _isCancelled, value);
    }

    public RunningCommand(string description)
    {
        Description = description;
    }
}