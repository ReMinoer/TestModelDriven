using System.Threading.Tasks;
using System.Windows;

namespace TestModelDriven.Framework;

public class CommandDispatcherWatcherViewModel : OneForOneViewModelBase<CommandDispatcherWatcher>
{
    public ViewModelCollection<RunningCommand, RunningCommandViewModel> Commands { get; }

    private string _commandDescription = string.Empty;
    private Task RefreshCommandDescriptionAsync(string value)
    {
        _commandDescription = value;
        return RefreshStatusMessageAsync();
    }

    private string _activityEllipsis = "...";
    private Task RefreshActivityEllipsisAsync(string value)
    {
        _activityEllipsis = value;
        return RefreshStatusMessageAsync();
    }

    private string _statusMessage = string.Empty;
    public string StatusMessage => _statusMessage;
    private Task RefreshStatusMessageAsync()
    {
        string value = !string.IsNullOrEmpty(_commandDescription)
            ? $"{_commandDescription}{_activityEllipsis}"
            : string.Empty;

        return SetAsync(ref _statusMessage, value, nameof(StatusMessage));
    }

    private Visibility _progressBarVisibility = Visibility.Hidden;
    public Visibility ProgressBarVisibility => _progressBarVisibility;
    private Task RefreshIsRunningAsync(Visibility value) => SetAsync(ref _progressBarVisibility, value, nameof(ProgressBarVisibility));

    public CommandDispatcherWatcherViewModel(CommandDispatcherWatcher commandDispatcherWatcher)
        : base(commandDispatcherWatcher)
    {
        Commands = new ViewModelCollection<RunningCommand, RunningCommandViewModel>(commandDispatcherWatcher.Commands, CommandViewModelFactory, x => x.Model);
        UpdateStatusMessageEllipsisSizeAsync().CaptureThrow();
    }

    private async Task UpdateStatusMessageEllipsisSizeAsync()
    {
        while (true)
        {
            await Task.Delay(500);
            await RefreshActivityEllipsisAsync(".");
            await Task.Delay(500);
            await RefreshActivityEllipsisAsync("..");
            await Task.Delay(500);
            await RefreshActivityEllipsisAsync("...");
        }
        // ReSharper disable once FunctionNeverReturns
    }

    static private async Task<RunningCommandViewModel> CommandViewModelFactory(RunningCommand model)
    {
        var viewModel = new RunningCommandViewModel(model);
        await viewModel.InitializeAsync();
        return viewModel;
    }

    protected override async Task OnModelPropertyChangedAsync(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(CommandDispatcherWatcher.CurrentCommand):
                await RefreshCommandDescriptionAsync(Model.CurrentCommand?.Description ?? string.Empty);
                await RefreshIsRunningAsync(Model.CurrentCommand is not null ? Visibility.Visible : Visibility.Hidden);
                break;
        }
    }
}