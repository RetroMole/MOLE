using System;
using System.Collections.Generic;
namespace LA_Back
{
	public class UndoRedo
	{
		public Stack<Action> UndoStack { get; } = new Stack<Action>();
		public Stack<Action> RedoStack { get; } = new Stack<Action>();
		public Stack<Action> DoStack { get; } = new Stack<Action>();
		public Stack<Action> BackupStack { get; } = new Stack<Action>();
		
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
		
		public void Undo() 
		{
			var undoAct = UndoStack.Pop();
			undoAct();
			BackupStack.Push(undoAct);
			RedoStack.Push(DoStack.Pop());
		}
		
		public void Redo()
		{
			var redoAct = RedoStack.Pop();
			redoAct();
			DoStack.Push(redoAct);
			UndoStack.Push(BackupStack.Pop());
		}
	}
}