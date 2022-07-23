#pragma warning disable CS8604

using ImGuiNET;
using Serilog;
using SixLabors.ImageSharp.PixelFormats;
using RetroMole.Core.Interfaces;
using RetroMole.Core.Utility;

namespace RetroMole;

public static partial class Gui
{
    public partial class Widgets
    {
        public class TextureCanvas : IWidget
        {
            private Texture TextureObj;
            private readonly UndoRedo.TextureUR UndoRedoObj;
            public TextureCanvas(string FilePath) : base()
            {
                TextureObj = Texture.Bind(FilePath, Core.GLOBALS.CurrentController);
                UndoRedoObj = new(TextureObj);
            }
            //public TextureCanvas(uint[,] RawPixelData) : base() { }
            public void Draw()
            {
                ImGui.Image(TextureObj.ID, new(TextureObj.Width*32, TextureObj.Height*32));
                Utility.WithSameLine(
                    () => {
                        if (ImGui.Button("Reverse"))
                        {
                            for (int i = 0; i < TextureObj.Pixels.Length; i++)
                            {
                                var p = TextureObj.Pixels[i];
                                TextureObj.Pixels[i] = new Rgba32(p.A, p.B, p.G, p.R);
                            }
                            Log.Debug($"Data: \n" +
                                $"{String.Join(", ", TextureObj.Pixels.Select(p => $"{p.Rgba:X8}"))}"
                            );
                        }
                    },
                    () => {
                        if (ImGui.Button("Undo") && UndoRedoObj.CanUndo)
                            UndoRedoObj.Undo();
                    },
                    () => {
                        if (ImGui.Button("Redo") && UndoRedoObj.CanRedo)
                            UndoRedoObj.Redo();
                    }
                );     
            }
        }
    }
}

#pragma warning restore CS8604
