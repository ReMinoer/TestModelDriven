using System.Threading.Tasks;
using System.Windows.Media;

namespace TestModelDriven.Framework;

public class RunningCommandViewModel : OneForOneViewModelBase<RunningCommand>
{
    public string Description => Model.Description;

    private Brush? _textBrush;
    public Brush? TextBrush => _textBrush;
    private async Task RefreshTextBrushAsync()
    {
        Brush? value;
        if (Model.IsCompleted)
            value = Brushes.Black;
        else if (Model.IsCancelled)
            value = Brushes.Gray;
        else
            value = Brushes.RoyalBlue;
        
        await SetAsync(ref _textBrush, value, nameof(TextBrush));
    }

    public RunningCommandViewModel(RunningCommand model)
        : base(model)
    {
    }

    public override async Task InitializeAsync()
    {
        await RefreshTextBrushAsync();
    }

    protected override async Task OnModelPropertyChangedAsync(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(RunningCommand.IsCompleted):
            case nameof(RunningCommand.IsCancelled):
                await RefreshTextBrushAsync();
                break;
        }
    }
}