using RetroMole.Core.Detection;
using RetroMole.Core.Mappers;

namespace RetroMole.Core;

public static class RomFactory
{
    public static ROM Load(string path)
    {
        var data = File.ReadAllBytes(Path.GetFullPath(path));
        return Load(data);
    }

    public static ROM Load(byte[] data)
    {
        var definition = SystemDefinitionRegistry.Detect(data);
        if (definition == null)
            throw new NotSupportedException("No system definition detected for this ROM.");

        var header = definition.ParseHeader(data);

        var mapper = ResolveMapper(definition, header.DeclaredMapper)
                     ?? throw new NotSupportedException($"Mapper {header.DeclaredMapper} is not supported for {definition.SystemName}.");

        var rom = (ROM)Activator.CreateInstance(definition.RomType, data, mapper)
                  ?? throw new InvalidOperationException($"Failed to construct ROM type {definition.RomType.FullName}.");

        return rom;
    }

    private static IAddressMapper? ResolveMapper(ISystemDefinition definition, int mapperNumber)
    {
        foreach (var spec in definition.SupportedMappers)
        {
            if (spec.MapperNumber == mapperNumber)
                return (IAddressMapper?)Activator.CreateInstance(spec.MapperType);
        }
        return null;
    }
}
