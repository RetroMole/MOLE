using System.Reflection;

namespace RetroMole.Core.Detection;

public interface ISystemDefinition
{
    string SystemName { get; }
    bool Detect(ReadOnlySpan<byte> rawBytes);
    HeaderInfo ParseHeader(byte[] rawBytes);
    IReadOnlyList<MapperSpec> SupportedMappers { get; }
    Type RomType { get; }
}
