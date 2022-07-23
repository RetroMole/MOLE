namespace RetroMole.Core.Interfaces
{
    public interface IUndoRedo
    {
        protected void CatchDo(object args);
        public void Undo();
        public void Redo();
        public void Clear();
        public bool CanUndo { get; }
        public bool CanRedo { get; }
    
    }
    public class OnChangedEventArgs : EventArgs
    {
        public OnChangedEventArgs(object args) { }
    }
}