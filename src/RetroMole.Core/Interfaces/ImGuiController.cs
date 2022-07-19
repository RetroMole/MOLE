using System.Numerics;

namespace RetroMole.Core.Interfaces;
public abstract class ImGuiController
{
    public abstract void Main(Action DrawUI);
    public abstract void RenderImDrawData(params object[] args);
    public abstract IntPtr GetOrCreateImgGuiBinding(Core.Interfaces.Texture Texture);
    public abstract void UpdateImGuiInput(params object[] args);
}
