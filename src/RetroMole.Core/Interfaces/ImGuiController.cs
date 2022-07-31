using System.Numerics;
using RetroMole.Core;

namespace RetroMole.Core.Interfaces;
public abstract class ImGuiController
{
    public abstract void Main(Action DrawUI);
    public abstract void RenderImDrawData(params object[] args);
    public abstract IntPtr BindTexture(Core.Texture texture);
    public abstract IntPtr UpdateTexture(Core.Texture texture);
    public abstract void FreeTexture(IntPtr ID);
    public abstract void UpdateInput(params object[] args);
    public Dictionary<IntPtr, object> Textures = new();
    public (IntPtr ID, object Texture) FontTexture;
}
