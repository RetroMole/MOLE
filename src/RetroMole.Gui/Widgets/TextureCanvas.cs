#pragma warning disable CS8604

using ImGuiNET;
using RetroMole.Core.Interfaces;

namespace RetroMole;

public static partial class Gui
{
    public partial class Widgets
    {
        public class TextureCanvas : IWidget
        {
            public Texture TextureObj {get; private set; }
            public TextureCanvas(string FilePath) : base()
            {
                TextureObj = Texture.Bind(FilePath, Core.GLOBALS.CurrentController);
            }
            //public TextureCanvas(uint[,] RawPixelData) : base() { }
            public void Draw() => ImGui.Image(TextureObj.ID, new(TextureObj.Width, TextureObj.Height));
        }
    }
}

#pragma warning restore CS8604
