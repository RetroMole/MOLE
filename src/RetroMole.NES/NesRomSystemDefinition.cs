using RetroMole.Core.Detection;
using RetroMole.Core.Mappers;
using RetroMole.NES.Mappers;

namespace RetroMole.NES;

public class NesRomSystemDefinition : ISystemDefinition
{
    public string SystemName => "NES";

    public bool Detect(ReadOnlySpan<byte> rawBytes)
    {
        if (rawBytes.Length < 16)
            return false;
        return rawBytes[0] == (byte)'N'
            && rawBytes[1] == (byte)'E'
            && rawBytes[2] == (byte)'S'
            && rawBytes[3] == 0x1A;
    }

    public HeaderInfo ParseHeader(byte[] rawBytes)
    {
        var h = rawBytes.Take(16).ToArray();
        var info = new HeaderInfo
        {
            System = SystemName,
            RawHeader = h,
            DeclaredMapper = ((h[8] & 0b0000_1111) << 8) | (h[7] & 0b1111_0000) | ((h[6] & 0b1111_0000) >> 4)
        };

        if ((h[7] & 0x0C) == 0x08)
        {
            info.PrgRomSize = ((h[9] & 0b0000_1111) << 8) | h[4];
            info.ChrRomSize = ((h[9] & 0b1111_0000) << 4) | h[5];
            info.PrgRamSize = 64 << (h[10] & 0b0000_1111);
            info.ChrRamSize = 64 << (h[11] & 0b0000_1111);
            info.GameCode = $"NES2.{h[12]:X2}";
        }
        else
        {
            info.PrgRomSize = h[4] * 16384;
            info.ChrRomSize = h[5] * 8192;
            info.PrgRamSize = h[8] * 8192;
        }

        info.InternalTitle = System.Text.Encoding.ASCII.GetString(h, 4, 16).TrimEnd('\0');
        info.Extras["F6_Trainer"] = (h[6] & 0b0000_0100) != 0;

        return info;
    }

    public IReadOnlyList<MapperSpec> SupportedMappers => new List<MapperSpec>
    {
        new MapperSpec(0, "NROM", typeof(NRomMapper)),
        new MapperSpec(1, "MMC1", typeof(MMC1Mapper)),
        new MapperSpec(2, "UxROM", typeof(NRomMapper)),
        new MapperSpec(3, "CNROM", typeof(NRomMapper)),
        new MapperSpec(4, "MMC3", typeof(MMC3Mapper))
    };

    public Type RomType => typeof(NesRom);
}
