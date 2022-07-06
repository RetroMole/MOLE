using System.Numerics;
using ImGuiNET;

namespace RetroMole;

public static partial class Gui
{
    public abstract class Window : IWidget
    {
        public string GetName() => Name;
        public Vector2 GetSize() => new Vector2(W, H);
        private string Name;
        private int W;
        private int H;
        private int X;
        private int Y;
        private bool ShouldClose;
        public bool IsOpen;
        public bool IsPopUp;
        public ImGuiWindowFlags Flags;
        public event Action<object> Close;
        public void CloseWindow(object args) => Close?.DynamicInvoke(args);
        public Window(string Name, int W, int H, ImGuiWindowFlags Flags, int X = 0, int Y = 0, bool IsOpen = false, bool IsPopUp = false)
        {
            this.Name = Name;
            this.W = W;
            this.W = H;
            this.X = X;
            this.Y = Y;
            this.IsOpen = IsOpen;
            this.IsPopUp = IsPopUp;
            this.Flags = Flags;
            Close += (_) => {
                ShouldClose = true;
            };
        }
        public virtual void Draw()
        {
            if (!IsOpen)
                return;

            ImGui.SetNextWindowSize(new(H, W));
            ImGui.SetNextWindowPos(new(X, Y), ImGuiCond.Appearing);

            if (IsPopUp && !ShouldClose)
                ImGui.OpenPopup($"{Name}_{GetHashCode()}");

            if (IsPopUp
                ? !ImGui.BeginPopup($"{Name}_{GetHashCode()}", Flags)
                : !ImGui.Begin(Name, ref IsOpen, Flags)
            ) return;

            if (IsPopUp && ShouldClose)
                ImGui.CloseCurrentPopup();

            if (ShouldClose)
                 IsOpen = ShouldClose = false;
            else this.Draw(Name, W, H);

            if (IsPopUp)
                 ImGui.EndPopup();
            else ImGui.End();
        }
        public abstract void Draw(string Name, int w, int h);
    }
}
