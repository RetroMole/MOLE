using System.Numerics;
using System.Reflection;
using ImGuiNET;
using QuickImGuiNET;
using RetroMole.Core;

namespace RetroMole;

public partial class Gui
{
    private bool _showDemoWindow;
    private readonly Backend _backend;
    public Gui(Backend backend, Package[] packages)
    {
        _backend = backend;
        
        // Auto-register Widgets
        backend.Logger.Information("RetroMole Gui v0.1");
        backend.Logger.Information("RetroMole Core (todo)");
        new QuickImGuiNET.Widgets.FileManager(backend, "FileManager##FileManager00")
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
                { "All", new List<string> { "*" } }
            },
            CurrentFTQuery = "All"
        };
        new QuickImGuiNET.Widgets.MemoryEditor(backend, "MemoryView##MemoryView00")
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
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
            }
        };
        new Widgets.PackageManager(backend, "PackageManager##PackageManager00")
        {
            RenderMode = WidgetRenderMode.Window,
            Size = new Vector2(300, 300),
            SizeCond = ImGuiCond.FirstUseEver,
            PositionCond = ImGuiCond.FirstUseEver,
            Position = new Vector2(100, 380),
            
            Packages = packages
        };
        new Widgets.About(backend, "About##About00")
        {
            RenderMode = WidgetRenderMode.Window,
            Size = new Vector2(300, 300),
            SizeCond = ImGuiCond.FirstUseEver,
            Position = new Vector2(100, 380),
            PositionCond = ImGuiCond.FirstUseEver
        };
    }

    public void Draw()
    {
        ImGui.DockSpaceOverViewport();
        if (ImGui.BeginMainMenuBar())
        {
            if (_backend.Config["debug"]["showMenu"] && ImGui.BeginMenu("Debug"))
            {
                ImGui.MenuItem("Open ImGui Demo Window", string.Empty, ref _showDemoWindow);
                _backend.Events["onMainMenuBar"]["Debug"].Invoke();
                ImGui.EndMenu();
            }

            _backend.Events["onMainMenuBar"].Invoke();
            ImGui.EndMainMenuBar();
        }

        if (_showDemoWindow)
            ImGui.ShowDemoWindow(ref _showDemoWindow);

        foreach (var widget in _backend.WidgetReg.Values)
            widget.Render();
    }

    public void Update(float deltaSeconds)
    {
        foreach (var widget in _backend.WidgetReg.Values)
            widget.Update(deltaSeconds);
    }
}