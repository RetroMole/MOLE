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
    public int PRGNVRAMSize;
    public int CHRRAMSize;
    public int CHRNVRAMSize;
    public byte F7ConsoleType;
    public byte VSPPUType;
    public byte VSHWType;
    public byte EXConsoleType;
    public byte MiscROMs;
    public byte ExpansionDevice;
}
