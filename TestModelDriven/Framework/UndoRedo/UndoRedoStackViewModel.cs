﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using TestModelDriven.Framework.Application;

namespace TestModelDriven.Framework.UndoRedo;

public class UndoRedoStackViewModel : ViewModelBase, IUndoRedoStack
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

    public override void OnLoaded() {}
    public override void OnUnloaded() {}

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

        // TODO: Wait for current batch before undo
        SetCurrentIndex(CurrentIndex - 1);
    }

    public void Redo()
    {
        if (!CanRedo)
            return;

        // TODO: Wait for current batch before redo
        SetCurrentIndex(CurrentIndex + 1);
    }

    public void Dispose()
    {
        foreach (var undoRedo in _stack)
            undoRedo.Dispose();
    }

    protected virtual void SetCurrentIndex(int newIndex, bool skipUndoRedo = false)
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