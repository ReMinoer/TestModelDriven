using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public class CommandDispatcherViewModel : ViewModelBase
{
    public ObservableCollection<RunningCommandViewModel> Commands { get; } = new();

    public CommandDispatcherViewModel()
    {
        CommandDispatcher.Current.CommandPushed.Subscribe(OnCommandChanged);
    }

    private async Task OnCommandChanged(RunningCommand runningCommand)
    {
        var viewModel = new RunningCommandViewModel(runningCommand);
        await viewModel.InitializeAsync();

        Commands.Add(viewModel);
    }
}