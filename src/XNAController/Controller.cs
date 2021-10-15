using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Num = System.Numerics;

#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable IDE0044 // Add readonly modifier
namespace XNAController
{
    /// <summary>
    /// Simple FNA + ImGui example
    /// </summary>
    public class Controller : Game
    {
        private GraphicsDeviceManager _graphics;
        private ImGuiRenderer _imGuiRenderer;

        //private Texture2D _xnaTexture;
        //private IntPtr _imGuiTexture;

        private IntPtr IconSurface;

        private object UI;
        delegate void DrawSignature();
        private DrawSignature draw;

        public Controller(object UI)
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1024,
                PreferredBackBufferHeight = 768,
                PreferMultiSampling = true
            };
            this.UI = UI;
            draw = (DrawSignature)UI.GetType().GetMethod("Draw").CreateDelegate(typeof(DrawSignature));

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _imGuiRenderer = new ImGuiRenderer(this);
            _imGuiRenderer.RebuildFontAtlas();

            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            Window.AllowUserResizing = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            IconSurface = SDL2.SDL.SDL_LoadBMP("Icon.bmp");
            // Texture loading example

            // First, load the texture as a Texture2D (can also be done using the XNA/FNA content pipeline)
            //_xnaTexture = CreateTexture(GraphicsDevice, 300, 150, pixel =>
            //{
            //    var red = (pixel % 300) / 2;
            //    return new Color(red, 1, 1);
            //});

            // Then, bind it to an ImGui-friendly pointer, that we can use during regular ImGui.** calls (see below)
            //_imGuiTexture = _imGuiRenderer.BindTexture(_xnaTexture);

            base.LoadContent();
        }

        protected override void Draw(GameTime gameTime)
        {
            SDL2.SDL.SDL_SetWindowIcon(Window.Handle,IconSurface);
            GraphicsDevice.Clear(new Color(clear_color.X, clear_color.Y, clear_color.Z));

            // Call BeforeLayout first to set things up
            _imGuiRenderer.BeforeLayout(gameTime);

            // Draw our UI
            ImGui.DockSpaceOverViewport(ImGui.GetMainViewport());
            draw();

            // Call AfterLayout now to finish up and draw all the things
            _imGuiRenderer.AfterLayout();

            base.Draw(gameTime);
        }

        private Num.Vector3 clear_color = new(114f / 255f, 144f / 255f, 154f / 255f);
    }
}
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore IDE0044 // Add readonly modifier
