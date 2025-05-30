using System;

namespace TestModelDriven.Framework.UndoRedo;

public class DirtyUndoRedoStackViewModel : UndoRedoStackViewModel
{
    private int _savedIndex;
    private bool _forceDirty;

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

    public bool IsDirty => _forceDirty || CurrentIndex != SavedIndex;
    public event EventHandler? IsDirtyChanged;

    public void ForceDirty()
    {
        bool wasDirty = IsDirty;

        _forceDirty = true;

        if (IsDirty != wasDirty)
            NotifyIsDirtyChanged();
    }

    public void SaveCurrentIndex()
    {
        bool wasDirty = IsDirty;

        _forceDirty = false;
        Set(ref _savedIndex, CurrentIndex);

        if (IsDirty != wasDirty)
            NotifyIsDirtyChanged();
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