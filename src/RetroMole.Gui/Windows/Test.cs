namespace RetroMole;

public static partial class Gui
{
    public partial class WindowTypes
    {
        public class Test : RetroMole.Gui.Window
        {
            public Test(string Name) : base(Name, 300, 300) { }
            Widgets.TextureCanvas TestCanvas = new(Path.GetFullPath(Path.Join(Core.GLOBALS.ExecPath, "Icon.bmp")));
            public override void Draw(string Name, int W, int H) => TestCanvas.Draw();
        }
    }
}
