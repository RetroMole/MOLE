using ImGuiNET;

namespace RetroMole;

public static partial class Gui
{
    public class Window : IWidget
    {
        public string Name;
        public int W;
        public int H;
        public Window(string Name, int w, int h)
        {
            this.Name = Name;
            this.W = w;
            this.W = h;
        }
        public virtual void Draw() => Draw(Name, W, H);
        public virtual void Draw(string Name, int w, int h)
        {
            ImGui.SetNextWindowSize(new(w, h));
            ImGui.Begin(Name);
            ImGui.End();
        }
    }
}