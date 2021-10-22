using System.Collections.Generic;
using System.Text;

namespace Mole.Shared.Util
{
    /// <summary>
    /// Undo/Redo System
    /// </summary>
    public class UndoRedo
    {
        public class Action
        {
            public enum ActionType
            {
                TestAction
            }

            public ActionType Type;
        }

        private readonly Stack<Action> _undoActions = new();
        private readonly Stack<Action> _redoActions = new();

        /// <summary>
        /// Push an Action to the stack
        /// </summary>
        /// <param name="a">Action</param>
        public void Push(Action a) => _undoActions.Push(a);

        /// <summary>
        /// Undo latest action
        /// </summary>
        public void Undo()
        {
            _redoActions.Push(_undoActions.Peek());
            _undoActions.Pop();
        }

        /// <summary>
        /// Redo latest action
        /// </summary>
        public void Redo()
        {
            _undoActions.Push(_redoActions.Peek());
            _redoActions.Pop();
        }

        /// <summary>
        /// Flushes current stack
        /// </summary>
        public void Flush()
        {
            
        }

        /// <summary>
        /// Exports stack for project
        /// </summary>
        /// <returns>Current stack</returns>
        public string ExportForProject()
        {
            var sb = new StringBuilder();
            foreach (Action a in _undoActions)
            {
                sb.Append("Undo");
                sb.Append(" | ");
                sb.Append(a.Type.ToString());
                switch (a.Type) { }
                sb.Append("\n");
            }
            foreach (Action a in _redoActions)
            {
                sb.Append("Redo");
                sb.Append(" | ");
                sb.Append(a.Type.ToString());
                switch (a.Type) { }
                sb.Append("\n");
            }
            return sb.ToString();
        }
    }
}