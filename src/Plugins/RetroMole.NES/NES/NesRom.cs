namespace RetroMole.NES;

using RetroMole.Core;
using RetroMole.Core.Detection;
using RetroMole.Core.Mappers;
using Serilog;

public class NesRom : ROM
{
    public INesRomHeader Header;

    public IAddressMapper Mapper { get; }

    public NesRom(byte[] data, IAddressMapper mapper)
        : base(data)
    {
        Mapper = mapper;
        var h = data.Take(16).ToArray();
        if (!h.Take(4).SequenceEqual(new byte[] { (byte)'N', (byte)'E', (byte)'S', 0x1A }))
        {
            Log.Error("Not a valid iNES/NES2.0 file");
            return;
        }

        if ((h[7] & 0x0C) == 0x08)
        {
            Log.Information("NES2.0 header found!");
            Header = new Nes2RomHeader()
            {
                ID = h.Take(4).ToArray(),
                PRG_ROMSize = ((h[9] & 0b0000_1111) << 8) | h[4],
                PRG_RAMSize = 64 << (h[10] & 0b0000_1111),
                PRGNVRAMSize = 60 << (h[10] & 0b1111_0000),

                CHR_ROMSize = ((h[9] & 0b1111_0000) << 4) | h[5],
                CHRRAMSize = 64 << (h[11] & 0b0000_1111),
                CHRNVRAMSize = 64 << (h[11] & 0b0000_1111),

                Mapper = ((h[8] & 0b0000_1111) << 8) | (h[7] & 0b1111_0000) | ((h[6] & 0b1111_0000) >> 4),
                SubMapper = (byte)((h[8] & 0b1111_0000) >> 4),

                F6_Mirroring = (h[6] & 1) == 1,
                F6_Battery = (h[6] & 2) == 2,
                F6_Trainer = (h[6] & 4) == 4,
                F6_4ScreenMode = (h[6] & 8) == 8,

                F7ConsoleType = (byte)(h[7] & 0b0000_0011),

                RegionTiming = h[12],

                VSPPUType = (byte)((h[7] & 3) == 1 ? (h[13] & 0b0000_1111) : 0),
                VSHWType = (byte)((h[7] & 3) == 1 ? ((h[13] & 0b1111_0000) >> 4) : 0),

                EXConsoleType = (byte)((h[7] & 3) == 3 ? (h[13] & 0b0000_1111) : 0),

                MiscROMs = h[14],
                ExpansionDevice = h[15],
            };
        }
        else
        {
            Log.Information("iNES header found!");
            Header = new Nes1RomHeader()
            {
                ID = h.Take(4).ToArray(),
                PRG_ROMSize = h[4] * 16384,
                PRG_RAMSize = h[8] * 8192,
                CHR_ROMSize = h[5] * 8192,

                Mapper = (h[7] & 0b1111_0000) | ((h[6] & 0b1111_0000) >> 4),

                F6_Mirroring = (h[6] & 1) == 1,
                F6_Battery = (h[6] & 2) == 2,
                F6_Trainer = (h[6] & 4) == 4,
                F6_4ScreenMode = (h[6] & 8) == 8,

                F7VSUnisystem = (h[7] & 1) == 1,
                F7PlayChoice10 = (h[7] & 2) == 2,

                F9TVSystem = h[9],

                F10TVSystem = (byte)(h[10] & 3),
                F10PRGRAM = (h[10] & 16) == 0,
                F10BusConflicts = (h[10] & 32) == 32,
            };
        }

        int programOffset = 16 + (Header.F6_Trainer ? 256 : 0);
        Chunks.Add(new Chunk(Slice(programOffset, Header.PRG_ROMSize)));
        Chunks[0].Type = ChunkType.Program;
        Chunks.Add(new Chunk(Slice(programOffset + Header.PRG_ROMSize, Header.CHR_ROMSize)));
        Chunks[1].Type = ChunkType.Graphics;
    }
}
