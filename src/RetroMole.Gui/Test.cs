using System;
using System.Linq;
using System.Numerics;
using ImGuiNET;
namespace RetroMole.Gui;
public static class Main
{       
	private static bool _showImGuiDemoWindow;
	public static void TestUI()
	{
		ImGui.DockSpaceOverViewport();

		if (ImGui.BeginMainMenuBar())
		{
			if (ImGui.BeginMenu("Debug"))
			{
				ImGui.MenuItem("Demo", "", ref _showImGuiDemoWindow);
				ImGui.EndMenu();
			}
			Core.Hooks.UI.TriggerMainMenuBar();
			ImGui.EndMainMenuBar();
		}

		if (_showImGuiDemoWindow)
		{
			ImGui.SetNextWindowPos(new Vector2(650, 20), ImGuiCond.FirstUseEver);
			ImGui.ShowDemoWindow(ref _showImGuiDemoWindow);
		}

		ImGuiIOPtr io = ImGui.GetIO();
		io.DeltaTime = 2f;
	}
}
