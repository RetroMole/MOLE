namespace RetroMole.Core.Detection;

public class HeaderInfo
{
    public string System { get; set; } = string.Empty;
    public int DeclaredMapper { get; set; }
    public int PrgRomSize { get; set; }
    public int ChrRomSize { get; set; }
    public int PrgRamSize { get; set; }
    public int ChrRamSize { get; set; }
    public byte[] RawHeader { get; set; } = Array.Empty<byte>();
    public string? InternalTitle { get; set; }
    public string? GameCode { get; set; }
    public Dictionary<string, object> Extras { get; set; } = new();
}
