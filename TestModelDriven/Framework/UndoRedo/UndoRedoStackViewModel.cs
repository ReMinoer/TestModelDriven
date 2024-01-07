using System.Collections.ObjectModel;
using System.Windows.Input;

namespace TestModelDriven.Framework.UndoRedo;

public class UndoRedoStackViewModel : NotifyPropertyChangedBase, IUndoRedoStack
{
    private readonly ObservableCollection<IUndoRedo> _stack;
    public ReadOnlyObservableCollection<IUndoRedo> Stack { get; }

    private int _currentIndex;
    public int CurrentIndex
    {
        get => _currentIndex;
        set => SetCurrentIndex(value);
    }
    
    public bool CanUndo => CurrentIndex > 0;
    public bool CanRedo => CurrentIndex < Stack.Count;

    public ICommand UndoCommand { get; }
    public ICommand RedoCommand { get; }

    public UndoRedoStackViewModel()
    {
        _stack = new ObservableCollection<IUndoRedo>();
        Stack = new ReadOnlyObservableCollection<IUndoRedo>(_stack);

        UndoCommand = new Command(_ => CanUndo, _ => Undo());
        RedoCommand = new Command(_ => CanRedo, _ => Redo());
    }

    public void Push(IUndoRedo undoRedo)
    {
        while (CurrentIndex < Stack.Count)
        {
            _stack[Stack.Count - 1].Dispose();
            _stack.RemoveAt(Stack.Count - 1);
        }

        _stack.Add(undoRedo);
        SetCurrentIndex(CurrentIndex + 1, skipUndoRedo: true);
    }

    public void Undo()
    {
        if (!CanUndo)
            return;

        SetCurrentIndex(CurrentIndex - 1);
    }

    public void Redo()
    {
        if (!CanRedo)
            return;

        SetCurrentIndex(CurrentIndex + 1);
    }

    public void Dispose()
    {
        for (int i = CurrentIndex; i < _stack.Count; i++)
        {
            _stack[i].Dispose();
        }
    }

    private void SetCurrentIndex(int newIndex, bool skipUndoRedo = false)
    {
        if (newIndex == _currentIndex)
            return;

        int previousIndex = _currentIndex;
        bool couldUndo = CanUndo;
        bool couldRedo = CanRedo;

        if (!skipUndoRedo)
        {
            while (newIndex < previousIndex)
            {
                previousIndex--;
                _stack[previousIndex].Undo();
            }
            while (newIndex > previousIndex)
            {
                _stack[previousIndex].Redo();
                previousIndex++;
            }
        }

        _currentIndex = newIndex;
        RaisePropertyChanged(nameof(CurrentIndex));

        if (CanUndo != couldUndo)
            RaisePropertyChanged(nameof(CanUndo));
        if (CanRedo != couldRedo)
            RaisePropertyChanged(nameof(CanRedo));
    }
}