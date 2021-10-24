using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly List<Action> _undoActions = new();
        private readonly List<Action> _redoActions = new();
        private readonly int _maxCount;

        public UndoRedo(int maxCount)
            => _maxCount = maxCount;

        /// <summary>
        /// Push an Action to the stack
        /// </summary>
        /// <param name="a">Action</param>
        public void Push(Action a)
        {
            _undoActions.Add(a);
            if (_undoActions.Count > _maxCount)
                _undoActions.RemoveAt(0);
        }

        /// <summary>
        /// Undo latest action
        /// </summary>
        public void Undo()
        {
            _redoActions.Add(_undoActions.Last());
            _undoActions.RemoveAt(_undoActions.Count - 1);
            if (_redoActions.Count > _maxCount)
                _redoActions.RemoveAt(0);
        }

        /// <summary>
        /// Redo latest action
        /// </summary>
        public void Redo()
        {
            _undoActions.Add(_redoActions.Last());
            _redoActions.RemoveAt(_redoActions.Count - 1);
            if (_undoActions.Count > _maxCount)
                _undoActions.RemoveAt(0);
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
                sb = Switch(sb, a);
                sb.Append("\n");
            }
            foreach (Action a in _redoActions)
            {
                sb.Append("Redo");
                sb.Append(" | ");
                sb.Append(a.Type.ToString());
                sb = Switch(sb, a);
                sb.Append("\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Loads stack for project
        /// </summary>
        /// <param name="lines">Linues</param>
        /// <returns>Stack instane</returns>
        /// <exception cref="Exception">Invalid stack member type</exception>
        public static UndoRedo LoadForProject(string[] lines)
        {
            var ur = new UndoRedo(80);
            foreach (string str in lines)
            {
                var split = str.Split(" | ");
                switch (split[0])
                {
                    case "Undo":
                        ur._undoActions.Add(SwitchLoad(split[2], 
                            new Action { Type = Enum.Parse<Action.ActionType>(split[1])}));
                        break;
                    case "Redo":
                        ur._redoActions.Add(SwitchLoad(split[2], 
                            new Action { Type = Enum.Parse<Action.ActionType>(split[1])}));
                        break;
                    default:
                        throw new Exception("Invalid stack member type!");
                }
            }
            return ur;
        }
        
        private static Action SwitchLoad(string s, Action a)
        {
            switch (a.Type) { }
            return a;
        }

        private StringBuilder Switch(StringBuilder sb, Action a)
        {
            switch (a.Type) { }
            return sb;
        }
    }
}