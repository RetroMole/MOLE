namespace RetroMole.Core.Detection;

using System.Reflection;

public interface ISystemDefinition
{
    string SystemName { get; }

    bool Detect(ReadOnlySpan<byte> rawBytes);

    HeaderInfo ParseHeader(byte[] rawBytes);

    IReadOnlyList<MapperSpec> SupportedMappers { get; }

    Type RomType { get; }
}
