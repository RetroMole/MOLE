using Veldrid;
using Veldrid.Sdl2;
namespace RetroMole.Render;

public partial class Veldrid : Core.Interfaces.Package
{
    public static unsafe class SDL2Extensions
    {
        public delegate IntPtr D_SDL_RWFromFile(string file, string mode);
        public delegate IntPtr D_SDL_LoadBMP_RW(IntPtr src, int freesrc);
        public delegate void D_SDL_FreeSurface(IntPtr surface);
        public delegate void D_SDL_SetWindowIcon(IntPtr window, IntPtr icon);
        public delegate void D_SDL_RaiseWindow(IntPtr sdl2Window);
        public delegate int D_SDL_GetNumVideoDisplays();
        public unsafe delegate int D_SDL_GetDisplayUsableBounds(int displayIndex, Rectangle* rect);
        public unsafe delegate uint D_SDL_GetGlobalMouseState(int* x, int* y);
        public static D_SDL_RWFromFile SDL_RWFromFile = Sdl2Native.LoadFunction<D_SDL_RWFromFile>("SDL_RWFromFile");
        public static D_SDL_LoadBMP_RW SDL_LoadBMP_RW = Sdl2Native.LoadFunction<D_SDL_LoadBMP_RW>("SDL_LoadBMP_RW");
        public static D_SDL_FreeSurface SDL_FreeSurface = Sdl2Native.LoadFunction<D_SDL_FreeSurface>("SDL_FreeSurface");
        public static D_SDL_SetWindowIcon SDL_SetWindowIcon = Sdl2Native.LoadFunction<D_SDL_SetWindowIcon>("SDL_SetWindowIcon");
        public static D_SDL_GetNumVideoDisplays SDL_GetNumVideoDisplays = Sdl2Native.LoadFunction<D_SDL_GetNumVideoDisplays>("SDL_GetNumVideoDisplays");
        public static D_SDL_GetDisplayUsableBounds SDL_GetDisplayUsableBounds = Sdl2Native.LoadFunction<D_SDL_GetDisplayUsableBounds>("SDL_GetDisplayUsableBounds");
        public static D_SDL_RaiseWindow SDL_RaiseWindow = Sdl2Native.LoadFunction<D_SDL_RaiseWindow>("SDL_RaiseWindow");
        public static D_SDL_GetGlobalMouseState SDL_GetGlobalMouseState = Sdl2Native.LoadFunction<D_SDL_GetGlobalMouseState>("SDL_GetGlobalMouseState");
    }
}
