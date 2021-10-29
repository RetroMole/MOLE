using System;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using System.Runtime.InteropServices;

using static ImGuiNET.ImGuiNative;

namespace Mole.Veldrid
{
    public static class VeldridController
    {
        private static Sdl2Window _window;
        private static GraphicsDevice _gd;
        private static CommandList _cl;
        private static ImGuiController _controller;

        private static readonly Vector3 ClearColor = new Vector3(0.45f, 0.55f, 0.6f);
        public static void SetThing(out float i, float val) { i = val; }
        
        delegate void Draw();
        private static Draw _draw;

        public static void Main(GraphicsBackend backend, object ui)
        {
            _draw = (Draw)ui.GetType().GetMethod("Draw")?.CreateDelegate(typeof(Draw));

            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(50, 50, 1280, 720, WindowState.Normal, $"Mole [Veldrid:{backend}]"),
                new GraphicsDeviceOptions(true, null, true, 
                    ResourceBindingModel.Improved, true, true),
                backend,
                out _window,
                out _gd);
            _window.Resized += () =>
            {
                _gd.MainSwapchain.Resize((uint)_window.Width, (uint)_window.Height);
                _controller.WindowResized(_window.Width, _window.Height);
            };
            _cl = _gd.ResourceFactory.CreateCommandList();
            _controller = new ImGuiController(_gd, _gd.MainSwapchain.Framebuffer.OutputDescription, _window.Width, _window.Height);

            // Main application loop
            while (_window.Exists)
            {
                InputSnapshot snapshot = _window.PumpEvents();
                if (!_window.Exists) { break; }
                _controller.Update(1f / 60f, snapshot);
                _draw();
                _cl.Begin();
                _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
                _cl.ClearColorTarget(0, new RgbaFloat(ClearColor.X, ClearColor.Y, ClearColor.Z, 1f));
                _controller.Render(_gd, _cl);
                _cl.End();
                _gd.SubmitCommands(_cl);
                _gd.SwapBuffers(_gd.MainSwapchain);
            }

            // Clean up Veldrid resources
            _gd.WaitForIdle();
            _controller.Dispose();
            _cl.Dispose();
            _gd.Dispose();
        }
    }
}
