using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using RetroMole.Core.Interfaces;

namespace RetroMole.Core;

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
            _Pixels[x + y * Width] = value;
            OnChanged?.Invoke(new OnTextureChangedEventArgs(this));
        }
    }
    public Rgba32[] Pixels
    {
        get => _Pixels;
        set {
            _Pixels = value;
            OnChanged?.Invoke(new OnTextureChangedEventArgs(this));
        }
    }

    //----------------------------------------------------------------------------------------------------------------------
    public event Action<OnTextureChangedEventArgs>? OnChanged;
    public class OnTextureChangedEventArgs : EventArgs
    {
        public Texture Texture { get; }
        public OnTextureChangedEventArgs(Texture texture) { Texture = texture; }
    }
    
    //----------------------------------------------------------------------------------------------------------------------
    public static Texture Bind(string FilePath, ImGuiController BindController)
    {
        var texture = new Texture(FilePath);
        texture.ID = BindController.BindTexture(texture);

        texture.OnChanged += (e) => BindController.UpdateTexture(e.Texture);

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
