using System;
using System.Collections.Generic;
namespace MOLE
{
    /// <summary>
    /// Custom Undo-Redo System
    /// </summary>
    public class UndoRedo
    {
        /// <summary>
        /// Stack of Undo Actions
        /// </summary>
        public Stack<Action> UndoStack { get; } = new Stack<Action>();
        /// <summary>
        /// Stack of Redo Actions
        /// </summary>
        public Stack<Action> RedoStack { get; } = new Stack<Action>();
        /// <summary>
        /// Stack of Do Actions
        /// </summary>
        public Stack<Action> DoStack { get; } = new Stack<Action>();
        /// <summary>
        /// Stack used during juggling to hold backups of Do Actions
        /// </summary>
        public Stack<Action> BackupStack { get; } = new Stack<Action>();

        /// <summary>
        /// Juggles stacks to "Do" something
        /// </summary>
        /// <param name="DoAct">Do Action</param>
        /// <param name="UndoAct">Undo Action</param>
        /// <param name="Entry">Weather there should be a new entry for this action in the stacks</param>
        public void Do(Action DoAct, Action UndoAct, bool Entry)
        {
            if (Entry)
            {
                DoStack.Push(DoAct);
                UndoStack.Push(UndoAct);
                RedoStack.Clear();
                BackupStack.Clear();
            }
            DoAct();
        }

        /// <summary>
        /// Juggles stacks to "Undo" the latest recorded action
        /// </summary>
        public void Undo()
        {
            var undoAct = UndoStack.Pop();
            undoAct();
            BackupStack.Push(undoAct);
            RedoStack.Push(DoStack.Pop());
        }

        /// <summary>
        /// Juggles stacks to "Redo the latest recorded undone action"
        /// </summary>
        public void Redo()
        {
            var redoAct = RedoStack.Pop();
            redoAct();
            DoStack.Push(redoAct);
            UndoStack.Push(BackupStack.Pop());
        }
    }
}