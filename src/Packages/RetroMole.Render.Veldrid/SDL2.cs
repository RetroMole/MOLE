using Veldrid.Sdl2;
namespace RetroMole.Render;

public partial class Veldrid : Core.Interfaces.Package
{
    public static class SDL2Extensions
    {
        public delegate IntPtr del_SDL_RWFromFile(string file, string mode);
        public delegate IntPtr del_SDL_LoadBMP_RW(IntPtr src, int freesrc);
        public delegate void del_SDL_FreeSurface(IntPtr surface);
        public delegate void del_SDL_SetWindowIcon(IntPtr window, IntPtr icon);
        public static del_SDL_RWFromFile SDL_RWFromFile = Sdl2Native.LoadFunction<del_SDL_RWFromFile>("SDL_RWFromFile");
        public static del_SDL_LoadBMP_RW SDL_LoadBMP_RW = Sdl2Native.LoadFunction<del_SDL_LoadBMP_RW>("SDL_LoadBMP_RW");
        public static del_SDL_FreeSurface SDL_FreeSurface = Sdl2Native.LoadFunction<del_SDL_FreeSurface>("SDL_FreeSurface");
        public static del_SDL_SetWindowIcon SDL_SetWindowIcon = Sdl2Native.LoadFunction<del_SDL_SetWindowIcon>("SDL_SetWindowIcon");
    }
}
