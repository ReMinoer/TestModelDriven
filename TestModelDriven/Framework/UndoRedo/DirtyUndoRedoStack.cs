using System.Threading.Tasks;

namespace TestModelDriven.Framework.UndoRedo;

public class DirtyUndoRedoStack : UndoRedoStack
{
    private int _savedIndex;
    private bool _forceDirty;

    public int SavedIndex => _savedIndex;
    public bool IsDirty => _forceDirty || CurrentIndex != SavedIndex;

    private readonly AsyncEvent _isDirtyChangedAsync = new();
    public IAsyncEvent IsDirtyChangedAsync => _isDirtyChangedAsync.Public;

    public async Task SaveCurrentIndexAsync()
    {
        bool wasDirty = IsDirty;

        _forceDirty = false;
        await SetAsync(ref _savedIndex, CurrentIndex, nameof(SavedIndex));
        await NotifyIsDirtyChangedAsync(wasDirty);
    }

    public async Task ForceDirtyAsync()
    {
        bool wasDirty = IsDirty;

        _forceDirty = true;
        await NotifyIsDirtyChangedAsync(wasDirty);
    }

    public override async Task SetCurrentIndexAsync(int newIndex, bool skipUndoRedo = false)
    {
        if (newIndex == CurrentIndex)
            return;

        bool wasDirty = IsDirty;

        await base.SetCurrentIndexAsync(newIndex, skipUndoRedo);
        await NotifyIsDirtyChangedAsync(wasDirty);
    }

    private async Task NotifyIsDirtyChangedAsync(bool previousValue)
    {
        if (IsDirty == previousValue)
            return;

        await RaisePropertyChangeAsync(previousValue, IsDirty, nameof(IsDirty));
        await _isDirtyChangedAsync.RaiseAsync();
    }
}