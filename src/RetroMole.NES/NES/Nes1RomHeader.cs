namespace RetroMole.NES;

public struct Nes1RomHeader : INesRomHeader
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
    public bool F7_VSUnisystem;
    public bool F7_PlayChoice10;
    public byte F9_TVSystem;
    public byte F10_TVSystem;
    public bool F10_PRGRAM;
    public bool F10_BusConflicts;
}
