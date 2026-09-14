namespace RetroMole.Core.Detection;

using System.Reflection;

public static class SystemDefinitionRegistry
{
    private static readonly List<ISystemDefinition> Definitions = new();

    public static void Register(ISystemDefinition definition)
    {
        Definitions.Add(definition);
    }

    public static ISystemDefinition? Detect(ReadOnlySpan<byte> rawBytes)
    {
        foreach (var definition in Definitions)
        {
            if (definition.Detect(rawBytes))
            {
                return definition;
            }
        }

        return null;
    }

    public static IReadOnlyList<ISystemDefinition> GetAll()
    {
        return Definitions;
    }
}
