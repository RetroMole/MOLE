using SixLabors.ImageSharp.PixelFormats;

namespace RetroMole.Core;

public static partial class UndoRedo
{
    public class TextureUR : Interfaces.IUndoRedo
    {
        public TextureUR(Texture texture)
        {
            state = texture;
            state.OnChanged += CatchDo;
        }
        private Stack<(int oldW, int oldH, int newW, int newH, List<(int i, Rgba32 oldPixel, Rgba32 newPixel)>)> UndoStack =
            new Stack<(int oldW, int oldH, int newW, int newH, List<(int i,  Rgba32 oldPixel, Rgba32 newPixel)>)>();
        private Stack<(int oldW, int oldH, int newW, int newH, List<(int i, Rgba32 oldPixel, Rgba32 newPixel)>)> RedoStack =
            new Stack<(int oldW, int oldH, int newW, int newH, List<(int i, Rgba32 oldPixel, Rgba32 newPixel)>)>();
        public void CatchDo(object args)
        {
            var newState = ((Texture.OnTextureChangedEventArgs)args).Texture;
            var diff = state.Pixels
                .Where((p, i) => p != newState.Pixels[i])
                .Select((p, i) => (i, p, newState.Pixels[i]))
                .ToList();

            UndoStack.Push((state.Width, state.Height, newState.Width, newState.Height, diff));
            RedoStack.Clear();

            state = newState;
        }
        public void Undo()
        {
            var (oldW, oldH, newW, newH, diff) = UndoStack.Pop();
            RedoStack.Push((oldW, oldH, newW, newH, diff));

            state.Height = oldH;
            state.Width = oldW;

            state.OnChanged -= CatchDo;
            diff.Select(d => state.Pixels[d.i] = d.oldPixel);
            state.OnChanged += CatchDo;
        }
        public void Redo()
        {
            var (oldW, oldH, newW, newH, diff) = RedoStack.Pop();
            UndoStack.Push((oldW, oldH, newW, newH, diff));

            state.Height = newH;
            state.Width = newW;

            state.OnChanged -= CatchDo;
            diff.Select(d => state.Pixels[d.i] = d.newPixel);
            state.OnChanged += CatchDo;
        }
        public void Clear()
        {
            UndoStack.Clear();
            RedoStack.Clear();
        }
        public bool CanUndo => UndoStack.Count > 0;
        public bool CanRedo => RedoStack.Count > 0;
        private Texture state;
    }
}