using ImGuiNET;
using System.Numerics;

namespace RetroMole.Core.Interfaces;
public abstract class ImGuiController
{
    public abstract void Main(Action DrawUI);
    public abstract void RenderImDrawData(ImDrawDataPtr draw_data);
    public abstract IntPtr GetOrCreateImgGuiBinding(params object[] args);
    public abstract void UpdateImGuiInput(params object[] args);
}