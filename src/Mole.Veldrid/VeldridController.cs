using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Mole.Veldrid
{
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public class VeldridController
    {
        private static ImGuiController _controller;
        private static readonly Vector3 ClearColor = new(0.45f, 0.55f, 0.6f);

        delegate void DrawSignature();

        public VeldridController(GraphicsBackend gBack, object ui)
        {
            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(50, 50, 1280, 720, WindowState.Normal, "MOLE"),
                new GraphicsDeviceOptions(true, null, true, 
                    ResourceBindingModel.Improved, true, true), 
                gBack, 
                out Sdl2Window window, 
                out GraphicsDevice gd);
            window.Resized += () =>
            {
                gd.MainSwapchain.Resize((uint)window.Width, (uint)window.Height);
                _controller.WindowResized(window.Width, window.Height);
            };
            var cl = gd.ResourceFactory.CreateCommandList();
            _controller = new ImGuiController(gd, window, gd.MainSwapchain.Framebuffer.OutputDescription, window.Width, window.Height);
            Random random = new();

            var draw = (DrawSignature)ui.GetType().GetMethod("Draw")?.CreateDelegate(typeof(DrawSignature));

            // Main application loop
            while (window.Exists)
            {
                InputSnapshot snapshot = window.PumpEvents();
                if (!window.Exists) { break; }
                _controller.Update(1f / 60f, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.
                ImGuiViewportPtr mainViewportPtr = ImGui.GetMainViewport();
                var mainViewPortDockSpaceId = ImGui.DockSpaceOverViewport(mainViewportPtr);
                draw();
                cl.Begin();
                cl.SetFramebuffer(gd.MainSwapchain.Framebuffer);
                cl.ClearColorTarget(0, new RgbaFloat(ClearColor.X, ClearColor.Y, ClearColor.Z, 1f));
                _controller.Render(gd, cl);
                cl.End();
                gd.SubmitCommands(cl);
                gd.SwapBuffers(gd.MainSwapchain);
                ImGuiController.SwapExtraWindows(gd);
            }

            // Clean up Veldrid resources
            gd.WaitForIdle();
            _controller.Dispose();
            cl.Dispose();
            gd.Dispose();
        }
    }
}
