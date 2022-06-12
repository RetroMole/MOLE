using System;
using System.Linq;
using System.Numerics;
using ImGuiNET;
namespace RetroMole.Gui;
public static class Main
{       
	private static bool _showImGuiDemoWindow = true;
	private static bool _showAnotherWindow = false;
	private static bool _showAnotherOtherWindow = false;
	private static bool _showAnotherOtherOtherWindow = false;
	public static void TestUI()
	{
		ImGui.DockSpaceOverViewport();

		if (ImGui.BeginMainMenuBar())
		{
			if (ImGui.BeginMenu("Debug"))
			{
				ImGui.EndMenu();
			}
			Core.Hooks.UI.TriggerMainMenuBar();
			ImGui.EndMainMenuBar();
		}
		// 1. Show a simple window.
		// Tip: if we don't call ImGui.BeginWindow()/ImGui.EndWindow() the widgets automatically appears in a window called "Debug".
		{
			ImGui.Text("Hello, world!");                                        // Display some text (you can use a format string too)

			ImGui.Text($"Mouse position: {ImGui.GetMousePos()}");
			ImGui.Text($"Mouse down: {ImGui.GetIO().MouseDown[0]}");

			ImGui.Checkbox("ImGui Demo Window", ref _showImGuiDemoWindow);                 // Edit bools storing our windows open/close state
			ImGui.Checkbox("Another Window", ref _showAnotherWindow);
			ImGui.Checkbox("Another Other Window", ref _showAnotherOtherWindow);
			ImGui.Checkbox("Another Other Other Window", ref _showAnotherOtherOtherWindow);

			float framerate = ImGui.GetIO().Framerate;
			ImGui.Text($"Application average {1000.0f / framerate:0.##} ms/frame ({framerate:0.#} FPS)");
		}

		// 2. Show another simple window. In most cases you will use an explicit Begin/End pair to name your windows.
		if (_showAnotherWindow)
		{
			ImGui.Begin("Another Window", ref _showAnotherWindow);
			ImGui.Text("Hello from another window!");
			if (ImGui.Button("Close Me"))
				_showAnotherWindow = false;
			ImGui.End();
		}
		
		if (_showAnotherOtherWindow)
		{
			ImGui.Begin("Another other Window", ref _showAnotherOtherWindow);
			ImGui.Text("Hello from another other window!");
			if (ImGui.Button("Close Me"))
				_showAnotherOtherWindow = false;
			ImGui.End();
		}
		
		if (_showAnotherOtherOtherWindow)
		{
			ImGui.Begin("Another other other Window", ref _showAnotherOtherOtherWindow);
			ImGui.Text("Hello from another other other window!");
			if (ImGui.Button("Close Me"))
				_showAnotherOtherOtherWindow = false;
			ImGui.End();
		}

		// 3. Show the ImGui demo window. Most of the sample code is in ImGui.ShowDemoWindow(). Read its code to learn more about Dear ImGui!
		if (_showImGuiDemoWindow)
		{
			ImGui.SetNextWindowPos(new Vector2(650, 20), ImGuiCond.FirstUseEver);
			ImGui.ShowDemoWindow(ref _showImGuiDemoWindow);
		}

		ImGuiIOPtr io = ImGui.GetIO();
		io.DeltaTime = 2f;
	}
}
