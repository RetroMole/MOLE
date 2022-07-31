using SixLabors.ImageSharp.PixelFormats;

namespace RetroMole.Core.Utility;

public static partial class UndoRedo
{
    public class TextureUR : Interfaces.IUndoRedo
    {
        public TextureUR(Interfaces.Texture texture)
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
            var newT = ((Interfaces.Texture.OnTextureChangedEventArgs)args).newT;
            var oldT = ((Interfaces.Texture.OnTextureChangedEventArgs)args).oldT;

            var diff = oldT.Pixels
                .Where((p, i) => p != newT.Pixels[i])
                .Select((p, i) => (i, p, newT.Pixels[i]))
                .ToList();

            UndoStack.Push((oldT.Width, oldT.Height, newT.Width, newT.Height, diff));
            RedoStack.Clear();
        }
        public void Undo()
        {
            var (oldW, oldH, newW, newH, diff) = UndoStack.Pop();
            RedoStack.Push((oldW, oldH, newW, newH, diff));

            state.Height = oldH;
            state.Width = oldW;

            state.OnChanged -= CatchDo;
            state.Pixels = state.Pixels
                .Select((p, i) => diff
                    .FirstOrDefault(d => d.i == i,
                        (i:i, oldPixel:p, newPixel:p)
                    ).oldPixel
                ).ToArray();
            state.OnChanged += CatchDo;
        }
        public void Redo()
        {
            var (oldW, oldH, newW, newH, diff) = RedoStack.Pop();
            UndoStack.Push((oldW, oldH, newW, newH, diff));

            state.Height = newH;
            state.Width = newW;

            state.OnChanged -= CatchDo;
            state.Pixels = state.Pixels
                .Select((p, i) => diff
                    .FirstOrDefault(d => d.i == i,
                        (i:i, oldPixel:p, newPixel:p)
                    ).newPixel
                ).ToArray();
            state.OnChanged += CatchDo;
        }
        public void Clear()
        {
            UndoStack.Clear();
            RedoStack.Clear();
        }
        public bool CanUndo => UndoStack.Count > 0;
        public bool CanRedo => RedoStack.Count > 0;
        private Interfaces.Texture state;
    }
}