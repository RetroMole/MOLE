#pragma warning disable CS8604

using ImGuiNET;
using Serilog;
using RetroMole.Core;
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
                TextureObj = Texture.Bind(FilePath, GLOBAL.CurrentController);
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
                            TextureObj.Pixels = TextureObj.Pixels.Reverse().ToArray();
                            Log.Debug($"Data: \n" +
                                $"{String.Join(", ", TextureObj.Pixels.Select(p => $"{p.Rgba:X8}"))}"
                            );
                        }
                    },
                    () => Utility.WithDisabled(!UndoRedoObj.CanUndo,
                        () => { if (ImGui.Button("Undo")) UndoRedoObj.Undo(); }
                    ),
                    () => Utility.WithDisabled(!UndoRedoObj.CanRedo,
                        () => { if (ImGui.Button("Redo")) UndoRedoObj.Redo(); }
                    )
                );     
            }
        }
    }
}

#pragma warning restore CS8604
