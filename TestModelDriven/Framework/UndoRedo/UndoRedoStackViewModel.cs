using System.Threading.Tasks;
using System.Windows.Input;

namespace TestModelDriven.Framework.UndoRedo;

public class UndoRedoStackViewModel : OneForOneViewModelBase<UndoRedoStack>
{
    private int _currentIndex;
    public int CurrentIndex
    {
        get => _currentIndex;
        set => PushPropertyTwoWay(
            "Set undo redo index",
            () => Model.CurrentIndex,
            value,
            () => Model.SetCurrentIndexAsync(value),
            () => _currentIndex,
            x => _currentIndex = x);
    }

    public ICommand UndoCommand { get; }
    public ICommand RedoCommand { get; }

    public UndoRedoStackViewModel(UndoRedoStack model)
        : base(model)
    {
        UndoCommand = new CommandDispatcherCommand("Undo last change", Model.UndoAsync, () => Model.CanUndo);
        RedoCommand = new CommandDispatcherCommand("Redo last undone change", Model.RedoAsync, () => Model.CanRedo);
    }

    protected override async Task OnModelPropertyChangedAsync(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(UndoRedoStack.CurrentIndex):
                await SetAsync(ref _currentIndex, Model.CurrentIndex, nameof(CurrentIndex));
                break;
        }
    }
}