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
        
        public void Redo() => _undoRedo.Redo();
        public void Undo() => _undoRedo.Undo();
        public void Dispose() => _undoRedo.Dispose();
    }
}