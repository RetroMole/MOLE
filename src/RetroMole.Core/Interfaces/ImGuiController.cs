using ImGuiNET;
using System.Numerics;

namespace RetroMole.Core.Interfaces;
public abstract class ImGuiController
{
//---------------------------Vars------------------------------
    protected bool _frameBegun = false;
    protected int _windowHeight;
    protected int _windowWidth;
    protected Vector2 _scaleFactor = Vector2.One;

//----------------------Texture stuff-------------------------
    protected Dictionary<IntPtr, object> _textures;
    public abstract IntPtr BindTexture(object Texture);
    public abstract void UnBindTexture(IntPtr ID);
    public abstract void CreateFontDeviceTexture();
    
//---------------------------General---------------------------
    public abstract void Main(Action DrawUI);
    public abstract void RenderImDrawData(ImDrawDataPtr draw_data);
    public void Render()
    {
        if (_frameBegun)
        {
            _frameBegun = false;
            ImGui.Render();
            RenderImDrawData(ImGui.GetDrawData());
        }
    }
    public void Update(float deltaSeconds)
    {
        if (_frameBegun)
        {
            ImGui.Render();
        }

        ImGuiIOPtr io = ImGui.GetIO();
        io.DisplaySize = new Vector2(
            _windowWidth / _scaleFactor.X,
            _windowHeight / _scaleFactor.Y);
        io.DisplayFramebufferScale = _scaleFactor;
        io.DeltaTime = deltaSeconds;

        UpdateImGuiInput();

        _frameBegun = true;
        ImGui.NewFrame();
    }

//---------------------------Input---------------------------
    public abstract void UpdateImGuiInput();
}