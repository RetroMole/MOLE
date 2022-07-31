using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using RetroMole.Core.Interfaces;

namespace RetroMole.Core.Interfaces;

public class Texture
{
    public int Width;
    public int Height;
    public IntPtr ID;

    //------------------------------------------------------------------------------------------------------------------------
    private Rgba32[] _Pixels;
    public Rgba32 this[int x, int y]
    {
        get => _Pixels[x + y * Width];
        set {
            var old = (Texture)MemberwiseClone();
            _Pixels[x + y * Width] = value;
            OnChanged?.Invoke(new OnTextureChangedEventArgs(old, this));
        }
    }
    public Rgba32[] Pixels
    {
        get => _Pixels;
        set {
            var old = (Texture)MemberwiseClone();
            _Pixels = value;
            OnChanged?.Invoke(new OnTextureChangedEventArgs(old, this));
        }
    }

    //----------------------------------------------------------------------------------------------------------------------
    public event Action<OnTextureChangedEventArgs>? OnChanged;
    public class OnTextureChangedEventArgs : EventArgs
    {
        public Texture oldT;
        public Texture newT;
        public OnTextureChangedEventArgs(Texture oldT, Texture newT) { this.oldT = oldT; this.newT = newT; }
    }
    
    //----------------------------------------------------------------------------------------------------------------------
    public static Texture Bind(string FilePath, ImGuiController BindController)
    {
        var texture = new Texture(FilePath);
        texture.ID = BindController.BindTexture(texture);

        texture.OnChanged += (e) => BindController.UpdateTexture(e.newT);

        return texture;
    }
    private Texture(string FilePath)
    {
        Image<Rgba32> img = Image.Load<Rgba32>(FilePath);

        Width = img.Width;
        Height = img.Height;

        _Pixels = new Rgba32[Width * Height];
        img.CopyPixelDataTo(Pixels);
    }
}
