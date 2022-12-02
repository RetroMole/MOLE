namespace RetroMole.Core;

public interface IUndoRedo
{
    public bool CanUndo { get; }
    public bool CanRedo { get; }
    public void CatchDo(object args);
    public void Undo();
    public void Redo();
    public void Clear();
}