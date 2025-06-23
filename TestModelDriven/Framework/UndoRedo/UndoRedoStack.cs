using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace TestModelDriven.Framework.UndoRedo;

public class UndoRedoStack : ModelBase, IUndoRedoStack
{
    private readonly ObservableCollection<IUndoRedo> _stack;
    public ReadOnlyObservableCollection<IUndoRedo> Stack { get; }
    
    public int CurrentIndex { get; private set; }

    public bool CanUndo => CurrentIndex > 0;
    public bool CanRedo => CurrentIndex < Stack.Count;

    public UndoRedoStack()
    {
        _stack = new ObservableCollection<IUndoRedo>();
        Stack = new ReadOnlyObservableCollection<IUndoRedo>(_stack);
    }

    public async Task PushAsync(IUndoRedo undoRedo)
    {
        while (CurrentIndex < Stack.Count)
        {
            await _stack[Stack.Count - 1].DisposeAsync();
            _stack.RemoveAt(Stack.Count - 1);
        }

        _stack.Add(undoRedo);
        await SetCurrentIndexAsync(CurrentIndex + 1, skipUndoRedo: true);
    }

    public async Task UndoAsync()
    {
        if (!CanUndo)
            return;

        // TODO: Wait for current batch before undo
        await SetCurrentIndexAsync(CurrentIndex - 1);
    }

    public async Task RedoAsync()
    {
        if (!CanRedo)
            return;

        // TODO: Wait for current batch before redo
        await SetCurrentIndexAsync(CurrentIndex + 1);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (IUndoRedo undoRedo in _stack)
            await undoRedo.DisposeAsync();
    }

    public virtual async Task SetCurrentIndexAsync(int newIndex, bool skipUndoRedo = false)
    {
        if (newIndex == CurrentIndex)
            return;

        int previousIndex = CurrentIndex;
        bool couldUndo = CanUndo;
        bool couldRedo = CanRedo;

        if (!skipUndoRedo)
        {
            while (newIndex < previousIndex)
            {
                previousIndex--;
                await _stack[previousIndex].UndoAsync();
            }
            while (newIndex > previousIndex)
            {
                await _stack[previousIndex].RedoAsync();
                previousIndex++;
            }
        }

        CurrentIndex = newIndex;
        await RaisePropertyChangeAsync(previousIndex, newIndex, nameof(CurrentIndex));

        if (CanUndo != couldUndo)
            await RaisePropertyChangeAsync(couldUndo, CanUndo, nameof(CanUndo));
        if (CanRedo != couldRedo)
            await RaisePropertyChangeAsync(couldRedo, CanRedo, nameof(CanRedo));
    }
}