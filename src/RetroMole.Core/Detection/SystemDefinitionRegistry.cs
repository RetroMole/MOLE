using System.Reflection;

namespace RetroMole.Core.Detection;

public static class SystemDefinitionRegistry
{
    private static readonly List<ISystemDefinition> _definitions = new();

    public static void Register(ISystemDefinition definition)
    {
        _definitions.Add(definition);
    }

    public static ISystemDefinition? Detect(ReadOnlySpan<byte> rawBytes)
    {
        foreach (var definition in _definitions)
        {
            if (definition.Detect(rawBytes))
                return definition;
        }
        return null;
    }

    public static IReadOnlyList<ISystemDefinition> GetAll()
    {
        return _definitions;
    }
}
