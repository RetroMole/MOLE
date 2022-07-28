#pragma warning disable CS8600, CS8618, CS8602

using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using ImGuiNET;

namespace RetroMole.Render;
public partial class Veldrid : Core.Interfaces.Package
{
    public class Controller : Core.Interfaces.ImGuiController, IDisposable
    {
        private GraphicsDevice _gd;
        private Sdl2Window _window;
        private IntPtr _icon;
        private CommandList _cl;
        private bool _frameBegun;
        private static Vector3 _clearColor = new Vector3(0.45f, 0.55f, 0.6f);

        // Veldrid objects
        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _indexBuffer;
        private DeviceBuffer _projMatrixBuffer;
        private Shader _vertexShader;
        private Shader _fragmentShader;
        private ResourceLayout _mainRL;
        private ResourceLayout _ftRL;
        private Pipeline _pipeline;
        private ResourceSet _mainRS;
        private ResourceSet _ftRS;
        private Dictionary<IntPtr, ResourceSet> _textureRS = new();

        private bool _controlDown;
        private bool _shiftDown;
        private bool _altDown;
        private bool _winKeyDown;

        private int _windowWidth;
        private int _windowHeight;
        private Vector2 _scaleFactor = Vector2.One;


        // Window stuff
        private readonly ImGuiWindow _mainViewportWindow;
        private readonly Platform_CreateWindow _createWindow;
        private readonly Platform_DestroyWindow _destroyWindow;
        private readonly Platform_GetWindowPos _getWindowPos;
        private readonly Platform_ShowWindow _showWindow;
        private readonly Platform_SetWindowPos _setWindowPos;
        private readonly Platform_SetWindowSize _setWindowSize;
        private readonly Platform_GetWindowSize _getWindowSize;
        private readonly Platform_SetWindowFocus _setWindowFocus;
        private readonly Platform_GetWindowFocus _getWindowFocus;
        private readonly Platform_GetWindowMinimized _getWindowMinimized;
        private readonly Platform_SetWindowTitle _setWindowTitle;
        private int _lastAssignedID = 100;
        public ImGuiIOPtr _IO;
        public ImGuiPlatformIOPtr _platformIO;

//--------------------------General-----------------------------
        public unsafe Controller(int width, int height, GraphicsBackend vk_backend = GraphicsBackend.OpenGL)
        {
            // Create window, GraphicsDevice, and all resources necessary to render
            _window = VeldridStartup.CreateWindow(new WindowCreateInfo(50, 50, 1280, 720, WindowState.Normal, "RetroMole"));
            _gd = VeldridStartup.CreateGraphicsDevice(_window, new GraphicsDeviceOptions(true, null, true, ResourceBindingModel.Improved, true, true), vk_backend);
            _cl = _gd.ResourceFactory.CreateCommandList();

            _window.Resized += () =>
            {
                _gd.MainSwapchain.Resize((uint)_window.Width, (uint)_window.Height);
                WindowResized(_window.Width, _window.Height);
            };
            _windowWidth = width;
            _windowHeight = height;

            // Load and set icon
            var icon_src = SDL2Extensions.SDL_RWFromFile.Invoke(Path.Combine(Core.GLOBALS.ExecPath, "Icon.bmp"), "rb");
            _icon = SDL2Extensions.SDL_LoadBMP_RW.Invoke(icon_src, 1);
            SDL2Extensions.SDL_SetWindowIcon.Invoke(_window.SdlWindowHandle, _icon);

            // Set up ImGui
            IntPtr context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);
            _IO = ImGui.GetIO();

            _IO.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            if (vk_backend == GraphicsBackend.Vulkan)
                _IO.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

            _platformIO = ImGui.GetPlatformIO();
            ImGuiViewportPtr mainViewport = _platformIO.Viewports[0];
            mainViewport.PlatformHandle = _window.Handle;
            _mainViewportWindow = new ImGuiWindow(_gd, mainViewport, _window);

            _createWindow = CreateWindow;
            _destroyWindow = DestroyWindow;
            _getWindowPos = GetWindowPos;
            _showWindow = ShowWindow;
            _setWindowPos = SetWindowPos;
            _setWindowSize = SetWindowSize;
            _getWindowSize = GetWindowSize;
            _setWindowFocus = SetWindowFocus;
            _getWindowFocus = GetWindowFocus;
            _getWindowMinimized = GetWindowMinimized;
            _setWindowTitle = SetWindowTitle;

            _platformIO.Platform_CreateWindow = Marshal.GetFunctionPointerForDelegate(_createWindow);
            _platformIO.Platform_DestroyWindow = Marshal.GetFunctionPointerForDelegate(_destroyWindow);
            _platformIO.Platform_ShowWindow = Marshal.GetFunctionPointerForDelegate(_showWindow);
            _platformIO.Platform_SetWindowPos = Marshal.GetFunctionPointerForDelegate(_setWindowPos);
            _platformIO.Platform_SetWindowSize = Marshal.GetFunctionPointerForDelegate(_setWindowSize);
            _platformIO.Platform_SetWindowFocus = Marshal.GetFunctionPointerForDelegate(_setWindowFocus);
            _platformIO.Platform_GetWindowFocus = Marshal.GetFunctionPointerForDelegate(_getWindowFocus);
            _platformIO.Platform_GetWindowMinimized = Marshal.GetFunctionPointerForDelegate(_getWindowMinimized);
            _platformIO.Platform_SetWindowTitle = Marshal.GetFunctionPointerForDelegate(_setWindowTitle);

            ImGuiNative.ImGuiPlatformIO_Set_Platform_GetWindowPos(_platformIO.NativePtr, Marshal.GetFunctionPointerForDelegate(_getWindowPos));
            ImGuiNative.ImGuiPlatformIO_Set_Platform_GetWindowSize(_platformIO.NativePtr, Marshal.GetFunctionPointerForDelegate(_getWindowSize));

            unsafe
            {
                _IO.NativePtr->BackendPlatformName = (byte*)new FixedAsciiString("Veldrid.SDL2 Backend").DataPtr;
            }
            _IO.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;
            _IO.BackendFlags |= ImGuiBackendFlags.HasSetMousePos;
            _IO.BackendFlags |= ImGuiBackendFlags.PlatformHasViewports;
            _IO.BackendFlags |= ImGuiBackendFlags.RendererHasViewports;
            _IO.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

            _IO.Fonts.AddFontDefault();

            CreateDeviceResources();
            SetKeyMappings();

            SetPerFrameImGuiData(1f / 60f);
            UpdateMonitors();

            ImGui.NewFrame();
            _frameBegun = true;
        }

        public override void Main(Action DrawUI)
        {
            // Main application loop
            while (_window.Exists)
            {
                InputSnapshot snapshot = _window.PumpEvents();
                if (!_window.Exists) { break; }
                Update(1f / 60f, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

                DrawUI.Invoke();

                _cl.Begin();
                _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
                _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));
                Render();
                _cl.End();
                _gd.SubmitCommands(_cl);
                _gd.SwapBuffers(_gd.MainSwapchain);
                SwapExtraWindows(_gd);
            }
            // Clean up Veldrid resources
            _gd.WaitForIdle();
            Dispose();
            _cl.Dispose();
            _gd.Dispose();
            SDL2Extensions.SDL_FreeSurface.Invoke(_icon);
        }

        public void Update(float deltaSeconds, InputSnapshot snapshot)
        {
            if (_frameBegun)
            {
                ImGui.Render();
                ImGui.UpdatePlatformWindows();
            }

            SetPerFrameImGuiData(deltaSeconds);
            UpdateInput(snapshot);
            UpdateMonitors();

            _frameBegun = true;
            ImGui.NewFrame();
        }

        public void Render()
        {
            if (_frameBegun)
            {
                _frameBegun = false;
                ImGui.Render();
                RenderImDrawData(ImGui.GetDrawData());

                // Update and Render additional Platform Windows
                if ((ImGui.GetIO().ConfigFlags & ImGuiConfigFlags.ViewportsEnable) != 0)
                {
                    ImGui.UpdatePlatformWindows();
                    for (int i = 1; i < _platformIO.Viewports.Size; i++)
                    {
                        ImGuiViewportPtr vp = _platformIO.Viewports[i];
                        ImGuiWindow window = (ImGuiWindow)GCHandle.FromIntPtr(vp.PlatformUserData).Target;
                        _cl.SetFramebuffer(window.Swapchain.Framebuffer);
                        RenderImDrawData(vp.DrawData);
                    }
                }
            }
        }

        private void SetPerFrameImGuiData(float deltaSeconds)
        {
            _IO.DisplaySize = new Vector2(
                _windowWidth / _scaleFactor.X,
                _windowHeight / _scaleFactor.Y);
            _IO.DisplayFramebufferScale = _scaleFactor;
            _IO.DeltaTime = deltaSeconds; // DeltaTime is in seconds.

            _platformIO.Viewports[0].Pos = new Vector2(_window.X, _window.Y);
            _platformIO.Viewports[0].Size = new Vector2(_window.Width, _window.Height);
        }

        public override void RenderImDrawData(params object[] args)
        {
            ImDrawDataPtr draw_data = (ImDrawDataPtr)args[0];
            uint vertexOffsetInVertices = 0;
            uint indexOffsetInElements = 0;

            if (draw_data.CmdListsCount == 0)
                return;

            uint totalVBSize = (uint)(draw_data.TotalVtxCount * Unsafe.SizeOf<ImDrawVert>());
            if (totalVBSize > _vertexBuffer.SizeInBytes)
            {
                _gd.DisposeWhenIdle(_vertexBuffer);
                _vertexBuffer = _gd.ResourceFactory.CreateBuffer(new BufferDescription((uint)(totalVBSize * 1.5f), BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            }

            uint totalIBSize = (uint)(draw_data.TotalIdxCount * sizeof(ushort));
            if (totalIBSize > _indexBuffer.SizeInBytes)
            {
                _gd.DisposeWhenIdle(_indexBuffer);
                _indexBuffer = _gd.ResourceFactory.CreateBuffer(new BufferDescription((uint)(totalIBSize * 1.5f), BufferUsage.IndexBuffer | BufferUsage.Dynamic));
            }

            Vector2 pos = draw_data.DisplayPos;
            for (int i = 0; i < draw_data.CmdListsCount; i++)
            {
                ImDrawListPtr cmd_list = draw_data.CmdListsRange[i];

                _cl.UpdateBuffer(
                    _vertexBuffer,
                    vertexOffsetInVertices * (uint)Unsafe.SizeOf<ImDrawVert>(),
                    cmd_list.VtxBuffer.Data,
                    (uint)(cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>()));

                _cl.UpdateBuffer(
                    _indexBuffer,
                    indexOffsetInElements * sizeof(ushort),
                    cmd_list.IdxBuffer.Data,
                    (uint)(cmd_list.IdxBuffer.Size * sizeof(ushort)));

                vertexOffsetInVertices += (uint)cmd_list.VtxBuffer.Size;
                indexOffsetInElements += (uint)cmd_list.IdxBuffer.Size;
            }

            // Setup orthographic projection matrix into our constant buffer
            Matrix4x4 mvp = Matrix4x4.CreateOrthographicOffCenter(
                pos.X,
                pos.X + draw_data.DisplaySize.X,
                pos.Y + draw_data.DisplaySize.Y,
                pos.Y,
                -1.0f,
                1.0f);

            _cl.UpdateBuffer(_projMatrixBuffer, 0, ref mvp);

            _cl.SetVertexBuffer(0, _vertexBuffer);
            _cl.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            _cl.SetPipeline(_pipeline);
            _cl.SetGraphicsResourceSet(0, _mainRS);

            draw_data.ScaleClipRects(_IO.DisplayFramebufferScale);

            // Render command lists
            int vtx_offset = 0;
            int idx_offset = 0;
            for (int n = 0; n < draw_data.CmdListsCount; n++)
            {
                ImDrawListPtr cmd_list = draw_data.CmdListsRange[n];
                for (int cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++)
                {
                    ImDrawCmdPtr pcmd = cmd_list.CmdBuffer[cmd_i];
                    if (pcmd.UserCallback != IntPtr.Zero)
                        throw new NotImplementedException();
                    else
                    {
                        // if (pcmd.TextureId != IntPtr.Zero)
                        if (pcmd.TextureId == FontTexture.ID)
                            _cl.SetGraphicsResourceSet(1, _ftRS);
                        else
                            _cl.SetGraphicsResourceSet(1, _textureRS[pcmd.TextureId]);

                        _cl.SetScissorRect(
                            0,
                            (uint)(pcmd.ClipRect.X - pos.X),
                            (uint)(pcmd.ClipRect.Y - pos.Y),
                            (uint)(pcmd.ClipRect.Z - pcmd.ClipRect.X),
                            (uint)(pcmd.ClipRect.W - pcmd.ClipRect.Y));

                        _cl.DrawIndexed(pcmd.ElemCount, 1, pcmd.IdxOffset + (uint)idx_offset, (int)pcmd.VtxOffset + vtx_offset, 0);
                    }
                }
                vtx_offset += cmd_list.VtxBuffer.Size;
                idx_offset += cmd_list.IdxBuffer.Size;
            }
        }

        private byte[] LoadEmbeddedShaderCode(string name, ShaderStages stage)
        {
            switch (_gd.ResourceFactory.BackendType)
            {
                case GraphicsBackend.Direct3D11:
                {
                    string resourceName = name + ".hlsl.bytes";
                    return GetEmbeddedResourceBytes(resourceName);
                }
                case GraphicsBackend.OpenGL:
                {
                    string resourceName = name + ".glsl";
                    return GetEmbeddedResourceBytes(resourceName);
                }
                case GraphicsBackend.Vulkan:
                {
                    string resourceName = name + ".spv";
                    return GetEmbeddedResourceBytes(resourceName);
                }
                case GraphicsBackend.Metal:
                {
                    string resourceName = name + ".metallib";
                    return GetEmbeddedResourceBytes(resourceName);
                }
                default:
                    throw new NotImplementedException();
            }
        }

        private byte[] GetEmbeddedResourceBytes(string resourceName)
        {
            Assembly assembly = typeof(Controller).Assembly;
            using (Stream s = assembly.GetManifestResourceStream(resourceName))
            {
                byte[] ret = new byte[s.Length];
                s.Read(ret, 0, (int)s.Length);
                return ret;
            }
        }

        public void Dispose()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
            _projMatrixBuffer.Dispose();

            _vertexShader.Dispose();
            _fragmentShader.Dispose();

            _pipeline.Dispose();

            _mainRS.Dispose();
            _ftRS.Dispose();

            _mainRL.Dispose();
            _ftRL.Dispose();

            ((TextureView)FontTexture.Texture).Target.Dispose();
            ((TextureView)FontTexture.Texture).Dispose();

            Array.ForEach<IntPtr>(Textures.Keys.ToArray(), ID => FreeTexture(ID));
        }

        //--------------------------Windows-----------------------------
        private void CreateWindow(ImGuiViewportPtr vp) { ImGuiWindow window = new ImGuiWindow(_gd, vp); }

        private void DestroyWindow(ImGuiViewportPtr vp)
        {
            if (vp.PlatformUserData != IntPtr.Zero)
            {
                ImGuiWindow window = (ImGuiWindow)GCHandle.FromIntPtr(vp.PlatformUserData).Target;
                window.Dispose();

                vp.PlatformUserData = IntPtr.Zero;
            }
        }

        private void ShowWindow(ImGuiViewportPtr vp)
        {
            ImGuiWindow window = (ImGuiWindow)GCHandle.FromIntPtr(vp.PlatformUserData).Target;
            Sdl2Native.SDL_ShowWindow(window.Window.SdlWindowHandle);
        }

        private unsafe void GetWindowPos(ImGuiViewportPtr vp, Vector2* outPos)
        {
            ImGuiWindow window = (ImGuiWindow)GCHandle.FromIntPtr(vp.PlatformUserData).Target;
            *outPos = new Vector2(window.Window.Bounds.X, window.Window.Bounds.Y);
        }

        private void SetWindowPos(ImGuiViewportPtr vp, Vector2 pos)
        {
            ImGuiWindow window = (ImGuiWindow)GCHandle.FromIntPtr(vp.PlatformUserData).Target;
            window.Window.X = (int)pos.X;
            window.Window.Y = (int)pos.Y;
        }

        private void SetWindowSize(ImGuiViewportPtr vp, Vector2 size)
        {
            ImGuiWindow window = (ImGuiWindow)GCHandle.FromIntPtr(vp.PlatformUserData).Target;
            Sdl2Native.SDL_SetWindowSize(window.Window.SdlWindowHandle, (int)size.X, (int)size.Y);
        }

        private unsafe void GetWindowSize(ImGuiViewportPtr vp, Vector2* outSize)
        {
            ImGuiWindow window = (ImGuiWindow)GCHandle.FromIntPtr(vp.PlatformUserData).Target;
            Rectangle bounds = window.Window.Bounds;
            *outSize = new Vector2(bounds.Width, bounds.Height);
        }

        private void SetWindowFocus(ImGuiViewportPtr vp)
        {
            ImGuiWindow window = (ImGuiWindow)GCHandle.FromIntPtr(vp.PlatformUserData).Target;
            SDL2Extensions.SDL_RaiseWindow.Invoke(window.Window.SdlWindowHandle);
        }

        private byte GetWindowFocus(ImGuiViewportPtr vp)
        {
            ImGuiWindow window = (ImGuiWindow)GCHandle.FromIntPtr(vp.PlatformUserData).Target;
            SDL_WindowFlags flags = Sdl2Native.SDL_GetWindowFlags(window.Window.SdlWindowHandle);
            return (flags & SDL_WindowFlags.InputFocus) != 0 ? (byte)1 : (byte)0;
        }

        private byte GetWindowMinimized(ImGuiViewportPtr vp)
        {
            ImGuiWindow window = (ImGuiWindow)GCHandle.FromIntPtr(vp.PlatformUserData).Target;
            SDL_WindowFlags flags = Sdl2Native.SDL_GetWindowFlags(window.Window.SdlWindowHandle);
            return (flags & SDL_WindowFlags.Minimized) != 0 ? (byte)1 : (byte)0;
        }

        private unsafe void SetWindowTitle(ImGuiViewportPtr vp, IntPtr title)
        {
            ImGuiWindow window = (ImGuiWindow)GCHandle.FromIntPtr(vp.PlatformUserData).Target;
            byte* titlePtr = (byte*)title;
            int count = 0;
            while (titlePtr[count] != 0)
            {
                titlePtr += 1;
            }
            window.Window.Title = System.Text.Encoding.ASCII.GetString(titlePtr, count);
        }

        public void WindowResized(int width, int height)
        {
            _windowWidth = width;
            _windowHeight = height;
        }

        public void SwapExtraWindows(GraphicsDevice gd)
        {
            ImGuiPlatformIOPtr platformIO = ImGui.GetPlatformIO();
            for (int i = 1; i < platformIO.Viewports.Size; i++)
            {
                ImGuiViewportPtr vp = platformIO.Viewports[i];
                ImGuiWindow window = (ImGuiWindow)GCHandle.FromIntPtr(vp.PlatformUserData).Target;
                SDL2Extensions.SDL_SetWindowIcon(window.Window.SdlWindowHandle, _icon);
                gd.SwapBuffers(window.Swapchain);
            }
        }

        private unsafe void UpdateMonitors()
        {  
            Marshal.FreeHGlobal(_platformIO.NativePtr->Monitors.Data);
            int numMonitors = SDL2Extensions.SDL_GetNumVideoDisplays();
            IntPtr data = Marshal.AllocHGlobal(Unsafe.SizeOf<ImGuiPlatformMonitor>() * numMonitors);
            _platformIO.NativePtr->Monitors = new ImVector(numMonitors, numMonitors, data);
            for (int i = 0; i < numMonitors; i++)
            {
                Rectangle r;
                SDL2Extensions.SDL_GetDisplayUsableBounds(i, &r);
                ImGuiPlatformMonitorPtr monitor = _platformIO.Monitors[i];
                monitor.DpiScale = 1f;
                monitor.MainPos = new Vector2(r.X, r.Y);
                monitor.MainSize = new Vector2(r.Width, r.Height);
                monitor.WorkPos = new Vector2(r.X, r.Y);
                monitor.WorkSize = new Vector2(r.Width, r.Height);
            }
        }

        //-------------------------Bindings-----------------------------
        public void CreateDeviceResources()
        {
            OutputDescription outputDescription = _gd.MainSwapchain.Framebuffer.OutputDescription;
            ResourceFactory factory = _gd.ResourceFactory;
            
            _vertexBuffer = factory.CreateBuffer(new BufferDescription(10000, BufferUsage.VertexBuffer | BufferUsage.Dynamic));
            _vertexBuffer.Name = "ImGui.NET Vertex Buffer";
            _indexBuffer = factory.CreateBuffer(new BufferDescription(2000, BufferUsage.IndexBuffer | BufferUsage.Dynamic));
            _indexBuffer.Name = "ImGui.NET Index Buffer";
            RecreateFontDeviceTexture();

            _projMatrixBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            _projMatrixBuffer.Name = "ImGui.NET Projection Buffer";

            byte[] vertexShaderBytes = LoadEmbeddedShaderCode("imgui-vertex", ShaderStages.Vertex);
            byte[] fragmentShaderBytes = LoadEmbeddedShaderCode("imgui-frag", ShaderStages.Fragment);
            _vertexShader = factory.CreateShader(new ShaderDescription(ShaderStages.Vertex, vertexShaderBytes, _gd.BackendType == GraphicsBackend.Metal ? "VS" : "main"));
            _fragmentShader = factory.CreateShader(new ShaderDescription(ShaderStages.Fragment, fragmentShaderBytes, _gd.BackendType == GraphicsBackend.Metal ? "FS" : "main"));

            VertexLayoutDescription[] vertexLayouts = new VertexLayoutDescription[]
            {
                new VertexLayoutDescription(
                    new VertexElementDescription("in_position", VertexElementSemantic.Position, VertexElementFormat.Float2),
                    new VertexElementDescription("in_texCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                    new VertexElementDescription("in_color", VertexElementSemantic.Color, VertexElementFormat.Byte4_Norm))
            };

            _mainRL = factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("ProjectionMatrixBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("MainSampler", ResourceKind.Sampler, ShaderStages.Fragment)));
            _ftRL = factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("MainTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment)));

            GraphicsPipelineDescription pd = new GraphicsPipelineDescription(
                BlendStateDescription.SingleAlphaBlend,
                new DepthStencilStateDescription(false, false, ComparisonKind.Always),
                new RasterizerStateDescription(FaceCullMode.None, PolygonFillMode.Solid, FrontFace.Clockwise, false, true),
                PrimitiveTopology.TriangleList,
                new ShaderSetDescription(vertexLayouts, new[] { _vertexShader, _fragmentShader }),
                new[] { _mainRL, _ftRL },
                outputDescription,
                ResourceBindingModel.Default);
            _pipeline = factory.CreateGraphicsPipeline(ref pd);

            _mainRS = factory.CreateResourceSet(new(
                _mainRL,
                _projMatrixBuffer,
                _gd.PointSampler
            ));
            _ftRS = factory.CreateResourceSet(new(
                _ftRL,
                (TextureView)FontTexture.Texture
            ));
        }

        public override IntPtr BindTexture(Core.Interfaces.Texture Texture)
        {
            Texture t = _gd.ResourceFactory.CreateTexture(new TextureDescription(
                (uint)Texture.Width,
                (uint)Texture.Height,
                1,1,1,
                PixelFormat.B8_G8_R8_A8_UNorm,
                TextureUsage.Sampled,
                TextureType.Texture2D
            ));    
            
            _gd.UpdateTexture(t, Texture.Pixels, 0, 0, 0, (uint)Texture.Width, (uint)Texture.Height, 1, 0, 0);

            var tv = _gd.ResourceFactory.CreateTextureView(t);
            var rs = _gd.ResourceFactory.CreateResourceSet(new(
                _ftRL,
                tv
            ));

            var ID = GetNextImGuiBindingID();

            Textures.Add(ID, tv);
            _textureRS.Add(ID, rs);

            return ID;
        }
        
        public override IntPtr UpdateTexture(Core.Interfaces.Texture texture)
        {
            Texture t = ((TextureView)Textures[texture.ID]).Target;

            _gd.UpdateTexture(t, texture.Pixels, 0, 0, 0, (uint)texture.Width, (uint)texture.Height, 1, 0, 0);

            var tv = _gd.ResourceFactory.CreateTextureView(t);
            var rs = _gd.ResourceFactory.CreateResourceSet(new(
                _ftRL,
                tv
            ));

            Textures[texture.ID] = tv;
            _textureRS[texture.ID] = rs;

            return texture.ID;
        }
        public override void FreeTexture(IntPtr ID)
        {
            TextureView tv = (TextureView)Textures[ID];
            ResourceSet rs = _textureRS[ID];

            Textures.Remove(ID);
            _textureRS.Remove(ID);

            tv.Target.Dispose();  
            tv.Dispose();
            rs.Dispose();        
        }
        private IntPtr GetNextImGuiBindingID() => (IntPtr)(++_lastAssignedID);

        public unsafe void RecreateFontDeviceTexture()
        {
            byte* pixels;
            int width, height, bytesPerPixel;
            _IO.Fonts.GetTexDataAsRGBA32(out pixels, out width, out height, out bytesPerPixel);
            _IO.Fonts.SetTexID(FontTexture.ID);

            var ft = _gd.ResourceFactory.CreateTexture(
                TextureDescription.Texture2D(
                    (uint)width,
                    (uint)height,
                    1,1,
                    PixelFormat.B8_G8_R8_A8_UNorm,
                    TextureUsage.Sampled
                )
            );
            ft.Name = "ImGui.NET Font Texture";

            _gd.UpdateTexture(
                ft,
                (IntPtr)pixels,
                (uint)(bytesPerPixel * width * height),
                0,0,0,
                (uint)width,
                (uint)height,
                1,0,0
            );

            FontTexture.Texture = _gd.ResourceFactory.CreateTextureView(ft);

            _IO.Fonts.ClearTexData();
        }

//--------------------------Input-------------------------------
        public override void UpdateInput(params object[] args) => UpdateInput((InputSnapshot)args[0]);
        public void UpdateInput(InputSnapshot snapshot)
        {
            ImGuiIOPtr io = ImGui.GetIO();

            Vector2 mousePosition = snapshot.MousePosition;

            // Determine if any of the mouse buttons were pressed during this snapshot period, even if they are no longer held.
            bool leftPressed = false;
            bool middlePressed = false;
            bool rightPressed = false;
            foreach (MouseEvent me in snapshot.MouseEvents)
            {
                if (me.Down)
                {
                    switch (me.MouseButton)
                    {
                        case MouseButton.Left:
                            leftPressed = true;
                            break;
                        case MouseButton.Middle:
                            middlePressed = true;
                            break;
                        case MouseButton.Right:
                            rightPressed = true;
                            break;
                    }
                }
            }

            io.MouseDown[0] = leftPressed || snapshot.IsMouseDown(MouseButton.Left);
            io.MouseDown[1] = middlePressed || snapshot.IsMouseDown(MouseButton.Right);
            io.MouseDown[2] = rightPressed || snapshot.IsMouseDown(MouseButton.Middle);

            int x, y;
            unsafe
            {
                uint buttons = SDL2Extensions.SDL_GetGlobalMouseState(&x, &y);
                io.MouseDown[0] = (buttons & 0b0001) != 0;
                io.MouseDown[1] = (buttons & 0b0010) != 0;
                io.MouseDown[2] = (buttons & 0b0100) != 0;
            }

            io.MousePos = new Vector2(x, y);
            io.MouseWheel = snapshot.WheelDelta;

            IReadOnlyList<char> keyCharPresses = snapshot.KeyCharPresses;
            for (int i = 0; i < keyCharPresses.Count; i++)
            {
                char c = keyCharPresses[i];
                io.AddInputCharacter(c);
            }

            IReadOnlyList<KeyEvent> keyEvents = snapshot.KeyEvents;
            for (int i = 0; i < keyEvents.Count; i++)
            {
                KeyEvent keyEvent = keyEvents[i];
                io.KeysDown[(int)keyEvent.Key] = keyEvent.Down;
                if (keyEvent.Key == Key.ControlLeft)
                {
                    _controlDown = keyEvent.Down;
                }
                if (keyEvent.Key == Key.ShiftLeft)
                {
                    _shiftDown = keyEvent.Down;
                }
                if (keyEvent.Key == Key.AltLeft)
                {
                    _altDown = keyEvent.Down;
                }
                if (keyEvent.Key == Key.WinLeft)
                {
                    _winKeyDown = keyEvent.Down;
                }
            }

            io.KeyCtrl = _controlDown;
            io.KeyAlt = _altDown;
            io.KeyShift = _shiftDown;
            io.KeySuper = _winKeyDown;

            ImVector<ImGuiViewportPtr> viewports = ImGui.GetPlatformIO().Viewports;
            for (int i = 1; i < viewports.Size; i++)
            {
                ImGuiViewportPtr v = viewports[i];
                ImGuiWindow window = ((ImGuiWindow)GCHandle.FromIntPtr(v.PlatformUserData).Target);
                window.Update();
            }
        }

        private static void SetKeyMappings()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.KeyMap[(int)ImGuiKey.Tab] = (int)Key.Tab;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Key.Left;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Key.Right;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Key.Up;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Key.Down;
            io.KeyMap[(int)ImGuiKey.PageUp] = (int)Key.PageUp;
            io.KeyMap[(int)ImGuiKey.PageDown] = (int)Key.PageDown;
            io.KeyMap[(int)ImGuiKey.Home] = (int)Key.Home;
            io.KeyMap[(int)ImGuiKey.End] = (int)Key.End;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)Key.Delete;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)Key.BackSpace;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)Key.Enter;
            io.KeyMap[(int)ImGuiKey.Escape] = (int)Key.Escape;
            io.KeyMap[(int)ImGuiKey.Space] = (int)Key.Space;
            io.KeyMap[(int)ImGuiKey.A] = (int)Key.A;
            io.KeyMap[(int)ImGuiKey.C] = (int)Key.C;
            io.KeyMap[(int)ImGuiKey.V] = (int)Key.V;
            io.KeyMap[(int)ImGuiKey.X] = (int)Key.X;
            io.KeyMap[(int)ImGuiKey.Y] = (int)Key.Y;
            io.KeyMap[(int)ImGuiKey.Z] = (int)Key.Z;
            io.KeyMap[(int)ImGuiKey.Space] = (int)Key.Space;
        }
    }
}

#pragma warning restore CS8600, CS8618, CS8602
