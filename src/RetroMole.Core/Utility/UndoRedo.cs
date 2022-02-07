namespace RetroMole.Core.Utility
{
    public abstract class Command
    {
        public abstract void Execute();
        public abstract void UnExecute();
    }
    public class UndoRedo
    {
        public Stack<Command> UndoCommands = new();
        public Stack<Command> RedoCommands = new();
        public void Redo(int levels)
        {
            for (int i = 1; i <= levels; i++)
            {
                if (RedoCommands.Count != 0)
                {
                    Command command = RedoCommands.Pop();
                    command.Execute();
                    UndoCommands.Push(command);
                }

            }
        }

        public void Undo(int levels)
        {
            for (int i = 1; i <= levels; i++)
            {
                if (UndoCommands.Count == 0)
                    continue;

                Command command = UndoCommands.Pop();
                command.UnExecute();
                RedoCommands.Push(command);

            }
        }

        public void Do(Command cmd)
        {
            cmd.Execute();
            UndoCommands.Push(cmd);
            RedoCommands.Clear();
        }
    }
}
