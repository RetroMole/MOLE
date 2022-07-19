using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RetroMole.Core.Interfaces;

public class Texture
{
    public int Width;
    public int Height;
    public byte[] Data;
    public IntPtr ID;
    private Texture(string FilePath)
    {
        Image<Bgra32> img = Image.Load<Bgra32>(FilePath);
        this.Width = img.Width;
        this.Height = img.Height;

        this.Data = new byte[this.Width * this.Height * 4];
        img.CopyPixelDataTo(this.Data);
    }
    public static Texture Bind(string FilePath, ImGuiController BindController)
    {
        var texture = new Texture(FilePath);
        texture.ID = BindController.GetOrCreateImgGuiBinding(texture);
        return texture;
    }
}