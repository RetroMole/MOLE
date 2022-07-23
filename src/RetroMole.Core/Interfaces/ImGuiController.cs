using System.Numerics;

namespace RetroMole.Core.Interfaces;
public abstract class ImGuiController
{
    public abstract void Main(Action DrawUI);
    public abstract void RenderImDrawData(params object[] args);
    public abstract IntPtr BindTexture(Core.Interfaces.Texture texture);
    public abstract IntPtr UpdateTexture(IntPtr ID, Core.Interfaces.Texture texture);
    public abstract void FreeTexture(IntPtr ID);
    public abstract void UpdateInput(params object[] args);
    public List<Binding> Textures = new();
    public Binding FontTexture;
    public struct Binding
    {
        public IntPtr ID;
        public object Texture;
    }
    
}
