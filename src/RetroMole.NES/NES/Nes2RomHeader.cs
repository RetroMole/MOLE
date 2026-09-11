namespace RetroMole.NES;

public struct Nes2RomHeader : INesRomHeader
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
    public byte SubMapper;
    public byte RegionTiming;
    public int PRG_NVRAMSize;
    public int CHR_RAMSize;
    public int CHR_NVRAMSize;
    public byte F7_ConsoleType;
    public byte VS_PPUType;
    public byte VS_HWType;
    public byte EX_ConsoleType;
    public byte MiscROMs;
    public byte ExpansionDevice;
}
