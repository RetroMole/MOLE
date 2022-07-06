using System;
using System.Linq;
using System.Numerics;
using ImGuiNET;
namespace RetroMole;
public static partial class Gui
{
	public static Window[] Windows = new Window[]
	{
		new WindowTypes.FileDialog("OpenFile Dialog", Core.Utility.CommonDirectories.Home, WindowTypes.FileDialog.FileMode.OpenFile, "*.*"),
		new WindowTypes.FileDialog("SaveFile Dialog", Core.Utility.CommonDirectories.Home, WindowTypes.FileDialog.FileMode.SaveFile, "*.*"),
		new WindowTypes.FileDialog("OpenFolder Dialog", Core.Utility.CommonDirectories.Home, WindowTypes.FileDialog.FileMode.OpenFolder, "*.*"),
		new WindowTypes.FileDialog("SaveFolder Dialog", Core.Utility.CommonDirectories.Home, WindowTypes.FileDialog.FileMode.SaveFolder, "*.*")
	};
	public static string[] testPaths = new string[] {String.Empty, String.Empty, String.Empty, String.Empty};
	private static bool _showImGuiDemoWindow;
	public static void ApplyHooks()
	{
		Windows[0].Close += (path) => testPaths[0] = (string)path;
		Windows[1].Close += (path) => testPaths[1] = (string)path;
		Windows[2].Close += (path) => testPaths[2] = (string)path;
		Windows[3].Close += (path) => testPaths[3] = (string)path;
	}
	public static void UI()
	{
		// Initialize
		ImGui.DockSpaceOverViewport();

		// Main Menu Bar
		if (ImGui.BeginMainMenuBar())
		{
			if (ImGui.BeginMenu("Debug"))
			{
				ImGui.MenuItem("Demo", "", ref _showImGuiDemoWindow);
				foreach (var w in Windows)
					ImGui.MenuItem(w.GetName(),"", ref w.IsOpen);
				ImGui.EndMenu();
			}
			Core.Hooks.UI.TriggerMainMenuBar();
			ImGui.EndMainMenuBar();
		}

		foreach(var s in testPaths)
			ImGui.Text(s);

		// Demo Window
		if (_showImGuiDemoWindow)
		{
			ImGui.SetNextWindowPos(new Vector2(650, 20), ImGuiCond.FirstUseEver);
			ImGui.ShowDemoWindow(ref _showImGuiDemoWindow);
		}

		// Draw Windows
		foreach (var w in Windows)
			w.Draw();

		// Finish up
		ImGuiIOPtr io = ImGui.GetIO();
		io.DeltaTime = 2f;
	}
}
