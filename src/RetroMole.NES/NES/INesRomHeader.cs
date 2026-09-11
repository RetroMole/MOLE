namespace RetroMole.NES;

public interface INesRomHeader
{
    public byte[] ID { get; set; }
    public int Mapper { get; set; }
    public int PRG_ROMSize { get; set; }
    public int CHR_ROMSize { get; set; }
    public int PRG_RAMSize { get; set; }
    public bool F6_Mirroring { get; set; }
    public bool F6_Battery { get; set; }
    public bool F6_Trainer { get; set; }
    public bool F6_4ScreenMode { get; set; }
}
