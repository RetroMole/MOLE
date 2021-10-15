using ImGuiNET;
using System;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace VeldridController
{
    public class Controller
    {
        private static Sdl2Window _window;
        private static GraphicsDevice _gd;
        private static CommandList _cl;
        private static ImGuiController _controller;

        private static Vector3 _clearColor = new(0.45f, 0.55f, 0.6f);

        delegate void DrawSignature();


        public Controller(GraphicsBackend g_back, object UI)
        {
            // Create window, GraphicsDevice, and all resources necessary for the demo.
            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(50, 50, 1280, 720, WindowState.Normal, "MOLE"),
                new GraphicsDeviceOptions(true, null, true, ResourceBindingModel.Improved, true, true),
                g_back,
                out _window,
                out _gd);
            _window.Resized += () =>
            {
                _gd.MainSwapchain.Resize((uint)_window.Width, (uint)_window.Height);
                _controller.WindowResized(_window.Width, _window.Height);
            };
            _cl = _gd.ResourceFactory.CreateCommandList();
            _controller = new ImGuiController(_gd, _window, _gd.MainSwapchain.Framebuffer.OutputDescription, _window.Width, _window.Height);
            Random random = new();

            var draw = (DrawSignature)UI.GetType().GetMethod("Draw").CreateDelegate(typeof(DrawSignature));

            IntPtr IconSurface = (IntPtr)Sdl2Native.LoadFunction<Action>("SDL_LoadBMP").DynamicInvoke("Icon.bmp");
            var SetWindowIcon = Sdl2Native.LoadFunction<Action>("SDL_SetWindowIcon");

            // Main application loop
            while (_window.Exists)
            {
                SetWindowIcon.DynamicInvoke(_window.Handle, IconSurface);
                InputSnapshot snapshot = _window.PumpEvents();
                if (!_window.Exists) { break; }
                _controller.Update(1f / 60f, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

                ImGuiViewportPtr mainViewportPtr = ImGui.GetMainViewport();
                var mainViewPortDockSpaceID = ImGui.DockSpaceOverViewport(mainViewportPtr);

                draw();

                _cl.Begin();
                _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
                _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));
                _controller.Render(_gd, _cl);
                _cl.End();
                _gd.SubmitCommands(_cl);
                _gd.SwapBuffers(_gd.MainSwapchain);
                ImGuiController.SwapExtraWindows(_gd);
            }

            // Clean up Veldrid resources
            _gd.WaitForIdle();
            _controller.Dispose();
            _cl.Dispose();
            _gd.Dispose();
        }
    }
}
