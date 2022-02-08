using RetroMole.Core.Interfaces;
using Serilog;

namespace RetroMole.Core.Components.NES
{
    public class Rom : IGameModuleComponent
    {
        // Fields
        public string path = "";

        // Internal representation
        private byte[] _rom;
        private byte[] _header;

        // ROMs
        public PRG_ROM PRG;
        public CHR_ROM CHR;

        // Header
        public object? Header;
        public struct INESHeader
        {
            public byte[] ID;

            public int PRG_ROMSize;
            public int CHR_ROMsize;
            public int PRG_RAMSize;

            public int Mapper;

            public bool F6_Mirroring;
            public bool F6_Battery;
            public bool F6_Trainer;
            public bool F6_4ScreenMode;

            public bool F7_VSUnisystem;
            public bool F7_PlayChoice10;

            public byte F9_TVSystem;
            public byte F10_TVSystem;
            public bool F10_PRGRAM;
            public bool F10_BusConflicts;
        }

        public struct NES20Header
        {
            public byte[] ID;

            public int PRG_ROMSize;
            public int CHR_ROMSize;

            public int PRG_RAMSize;
            public int PRG_NVRAMSize;

            public int CHR_RAMSize;
            public int CHR_NVRAMSize;

            public int Mapper;
            public byte SubMapper;
            public byte RegionTiming;

            public bool F6_Mirroring;
            public bool F6_Battery;
            public bool F6_Trainer;
            public bool F6_4ScreenMode;
            public byte F7_ConsoleType;

            public byte VS_PPUType;
            public byte VS_HWType;

            public byte EX_ConsoleType;

            public byte MiscROMs;
            public byte ExpansionDevice;
        }

        // Empty ctor
        public Rom() { }

        // File ctor
        public Rom(string FilePath)
        {
            path = FilePath;
            _rom = File.ReadAllBytes(path);
            var h = _rom.Take(16).ToArray();
            _header = h;
            if (!(h[0] == 'N' && h[1] == 'E' && h[2] == 'S' && h[3] == 0x1A))
            {
                Log.Error("Not a valid iNES/NES2.0 file");
                return;
            }

            if ((h[7] & 0x0C) == 0x08)
            {
                Log.Information("NES2.0 header found!");
                Header = new NES20Header()
                {
                    ID = h.Take(4).ToArray(),
                    PRG_ROMSize = ((h[9] & 0b0000_1111) << 8) | h[4],
                    PRG_RAMSize = 64 << (h[10] & 0b0000_1111),
                    PRG_NVRAMSize = 60 << (h[10] & 0b1111_0000),

                    CHR_ROMSize = ((h[9] & 0b1111_0000) << 4) | h[5],
                    CHR_RAMSize = 64 << (h[11] & 0b0000_1111),
                    CHR_NVRAMSize = 64 << (h[11] & 0b0000_1111),

                    Mapper = ((h[8] & 0b0000_1111) << 8) | (h[7] & 0b1111_0000) | ((h[6] & 0b1111_0000) >> 4),
                    SubMapper = (byte)((h[8] & 0b1111_0000) >> 4),

                    F6_Mirroring = (h[6] & 1) == 1,         // 0 false,  1 true
                    F6_Battery = (h[6] & 2) == 2,           // 0 false,  2 true
                    F6_Trainer = (h[6] & 4) == 4,           // 0 false,  4 true
                    F6_4ScreenMode = (h[6] & 8) == 8,       // 0 false,  8 true

                    F7_ConsoleType = (byte)(h[7] & 0b0000_0011),

                    RegionTiming = h[12],

                    VS_PPUType = (byte)((h[7] & 3) == 1 ? (h[13] & 0b0000_1111) : 0),
                    VS_HWType = (byte)((h[7] & 3) == 1 ? ((h[13] & 0b1111_0000) >> 4) : 0),

                    EX_ConsoleType = (byte)((h[7] & 3) == 3 ? (h[13] & 0b0000_1111) : 0),

                    MiscROMs = h[14],
                    ExpansionDevice = h[15]
                };
            }
            else
            {
                Log.Information("iNES header found!");
                Header = new INESHeader()
                {
                    ID = h.Take(4).ToArray(),
                    PRG_ROMSize = h[4] * 16384,
                    PRG_RAMSize = h[8] * 8192,
                    CHR_ROMsize = h[5] * 8192,

                    Mapper = (h[7] & 0b1111_0000) | ((h[6] & 0b1111_0000) >> 4),

                    F6_Mirroring = (h[6] & 1) == 1,         // 0 false,  1 true
                    F6_Battery = (h[6] & 2) == 2,           // 0 false,  2 true
                    F6_Trainer = (h[6] & 4) == 4,           // 0 false,  4 true
                    F6_4ScreenMode = (h[6] & 8) == 8,       // 0 false,  8 true

                    F7_VSUnisystem = (h[7] & 1) == 1,       // 0 false,  1 true
                    F7_PlayChoice10 = (h[7] & 2) == 2,      // 0 false,  2 true

                    F9_TVSystem = h[9],

                    F10_TVSystem = (byte)(h[10] & 3),
                    F10_PRGRAM = (h[10] & 16) == 0,         // 0 false, 16 true
                    F10_BusConflicts = (h[10] & 32) == 32   // 0 false, 32 true
                };
            }

            PRG = new(
                _rom.Skip(16 + ( // Skip Header
                    (bool)Header.GetType().GetField("F6_Trainer").GetValue(Header) ? 256 : 0) // Skip Trainer
                )
                .Take((int)Header.GetType().GetField("PRG_ROMSize").GetValue(Header)) // Get Size * bytes from _rom
                .ToArray()
            );

            CHR = new(_rom.Skip(16 + ( // Skip Header
                    (bool)Header.GetType().GetField("F6_Trainer").GetValue(Header) ? 256 : 0) + // Skip Trainer
                    PRG.Count() // Skip PRG_ROM
                )
                .Take((int)Header.GetType().GetField("PRG_ROMSize").GetValue(Header)) // Get Size * bytes from _rom
                .ToArray()
            );
            if (OnRomSave is null)
                OnRomSave += Save;
        }
        public event Action OnRomSave;
        public void TriggerRomSave() => OnRomSave?.Invoke();
        private void Save() => Save(path);
        private void Save(string path) => File.WriteAllBytes(path, _header.Concat(PRG).Concat(CHR).ToArray());
    }
}
