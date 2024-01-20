using System;

namespace TestModelDriven.Framework.UndoRedo;

public class DirtyUndoRedoStackViewModel : UndoRedoStackViewModel
{
    private int _savedIndex;
    public int SavedIndex
    {
        get => _savedIndex;
        private set
        {
            bool wasDirty = IsDirty;

            if (!Set(ref _savedIndex, value))
                return;

            if (IsDirty != wasDirty)
                NotifyIsDirtyChanged();
        }
    }

    public bool IsDirty => CurrentIndex != SavedIndex;
    public event EventHandler? IsDirtyChanged;

    public void SaveCurrentIndex()
    {
        SavedIndex = CurrentIndex;
    }

    protected override void SetCurrentIndex(int newIndex, bool skipUndoRedo = false)
    {
        if (newIndex == CurrentIndex)
            return;

        bool wasDirty = IsDirty;

        base.SetCurrentIndex(newIndex, skipUndoRedo);

        if (IsDirty != wasDirty)
            NotifyIsDirtyChanged();
    }

    private void NotifyIsDirtyChanged()
    {
        RaisePropertyChanged(nameof(IsDirty));
        IsDirtyChanged?.Invoke(this, EventArgs.Empty);
    }
}