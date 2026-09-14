namespace RetroMole;

using System.Numerics;
using System.Reflection;
using ImGuiNET;
using QuickImGuiNET;
using RetroMole.Core;

public partial class Gui
{
    private bool showDemoWindow;
    private readonly Context ctx;

    public Gui(Context ctx, Package[] packages)
    {
        this.ctx = ctx;

        // Auto-register Widgets
        ctx.Logger.Information("Initializing RetroMole Gui v0.1");
        new QuickImGuiNET.Widgets.FileManager(ctx, "FileManager##FileManager00")
        {
            RenderMode = WidgetRenderMode.Modal,
            Size = new Vector2(500, 500),
            SizeCond = ImGuiCond.FirstUseEver,
            Position = ImGui.GetMainViewport().GetWorkCenter() - new Vector2(250, 250),
            PositionCond = ImGuiCond.FirstUseEver,

            Mode = QuickImGuiNET.Widgets.FileManager.SelectionMode.SaveFile,
            CurrentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? Path.GetFullPath("~"),
            ShowHiddenFiles = true,
            ShowSystemFiles = true,
            FileTypeQueries = new Dictionary<string, List<string>>
            {
                { "Images", new List<string> { ".png", ".jpg", ".jpeg" } },
                { "All", new List<string> { "*" } },
            },
            CurrentFTQuery = "All",
        };
        new QuickImGuiNET.Widgets.MemoryEditor(ctx, "MemoryView##MemoryView00")
        {
            RenderMode = WidgetRenderMode.Window,
            Size = new Vector2(500, 360),
            SizeCond = ImGuiCond.FirstUseEver,
            Position = new Vector2(100, 380),
            PositionCond = ImGuiCond.FirstUseEver,
            WindowFlags = ImGuiWindowFlags.NoScrollbar,

            ReadOnly = false,
            Cols = 16,
            OptShowOptions = true,
            OptShowAscii = true,
            OptGreyOutZeroes = true,
            OptUpperCaseHex = true,
            OptMidColsCount = 8,
            OptAddrDigitsCount = 0,
            OptFooterExtraHeight = 0,
            HighlightColor = 0xFF_FFFF32,
            Data = new byte[]
            {
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
            },
        };
        new Widgets.PackageManager(ctx, "PackageManager##PackageManager00")
        {
            RenderMode = WidgetRenderMode.Window,
            Size = new Vector2(300, 300),
            SizeCond = ImGuiCond.FirstUseEver,
            PositionCond = ImGuiCond.FirstUseEver,
            Position = new Vector2(100, 380),

            Packages = packages,
        };
        new Widgets.About(ctx, "About##About00")
        {
            RenderMode = WidgetRenderMode.Window,
            Size = new Vector2(300, 300),
            SizeCond = ImGuiCond.FirstUseEver,
            Position = new Vector2(100, 380),
            PositionCond = ImGuiCond.FirstUseEver,
        };
    }

    public void Draw()
    {
        ImGui.DockSpaceOverViewport();
        if (ImGui.BeginMainMenuBar())
        {
            if (ctx.Config["debug"]["showMenu"] && ImGui.BeginMenu("Debug"))
            {
                ImGui.MenuItem("Open ImGui Demo Window", string.Empty, ref showDemoWindow);
                ctx.Events["onMainMenuBar"]["Debug"].Invoke();
                ImGui.EndMenu();
            }

            ctx.Events["onMainMenuBar"].Invoke();
            ImGui.EndMainMenuBar();
        }

        if (showDemoWindow)
        {
            ImGui.ShowDemoWindow(ref showDemoWindow);
        }

        foreach (var widget in ctx.WidgetReg.Values)
        {
            widget.Render();
        }
    }

    public void Update(float deltaSeconds)
    {
        foreach (var widget in ctx.WidgetReg.Values)
        {
            widget.Update(deltaSeconds);
        }
    }
}
