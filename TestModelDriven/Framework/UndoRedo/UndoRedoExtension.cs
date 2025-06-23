using System.Threading.Tasks;

namespace TestModelDriven.Framework.UndoRedo;

static public class UndoRedoExtension
{
    static public IUndoRedo PostDescription(this IUndoRedo undoRedo, string description)
    {
        return new PostDescriptionUndoRedo(undoRedo, description);
    }

    private class PostDescriptionUndoRedo : IUndoRedo
    {
        private readonly IUndoRedo _undoRedo;
        public string Description { get; }

        public bool IsDone => _undoRedo.IsDone;

        public PostDescriptionUndoRedo(IUndoRedo undoRedo, string description)
        {
            Description = description;
            _undoRedo = undoRedo;
        }
        
        public async Task RedoAsync() => await _undoRedo.RedoAsync().ConfigureAwait(false);
        public async Task UndoAsync() => await _undoRedo.UndoAsync().ConfigureAwait(false);
        public async ValueTask DisposeAsync() => await _undoRedo.DisposeAsync().ConfigureAwait(false);
    }
}