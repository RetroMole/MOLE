using System;
using System.Linq;
using System.Numerics;
using ImGuiNET;
namespace RetroMole;
public static partial class Gui
{       
	private static bool _showImGuiDemoWindow;
	private static bool _showAsarWindow;
	private static string AsarInitString = String.Empty;
	public static Window[] Windows = new Window[]
	{
		new Window("Test @ UI", 400, 250)
	};
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
				ImGui.MenuItem("Asar", "", ref _showAsarWindow);
				ImGui.EndMenu();
			}
			Core.Hooks.UI.TriggerMainMenuBar();
			ImGui.EndMainMenuBar();
		}

		// Demo Window
		if (_showImGuiDemoWindow)
		{
			ImGui.SetNextWindowPos(new Vector2(650, 20), ImGuiCond.FirstUseEver);
			ImGui.ShowDemoWindow(ref _showImGuiDemoWindow);
		}

		// Asar Window
		if (_showAsarWindow && ImGui.Begin("Asar"))
		{
			if (ImGui.Button("Init"))
				if (Asar.Init())
					AsarInitString = "Init Successful";
				else
					AsarInitString = "Init Failed";
			ImGui.Text(AsarInitString);
			ImGui.Text($"Asar Version: {Asar.Ver2Str(Asar.Version())}");
					
			ImGui.End();
		}

		// Draw Windows
		foreach (var w in Windows)
		{
			w.Draw();
		}

		// Finish up
		ImGuiIOPtr io = ImGui.GetIO();
		io.DeltaTime = 2f;
	}
}
