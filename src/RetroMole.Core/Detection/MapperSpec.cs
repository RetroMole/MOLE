namespace RetroMole.Core.Detection;

public class MapperSpec
{
    public int MapperNumber { get; }

    public string Name { get; }

    public Type MapperType { get; }

    public int Priority { get; }

    public MapperSpec(int mapperNumber, string name, Type mapperType, int priority = 0)
    {
        MapperNumber = mapperNumber;
        Name = name;
        MapperType = mapperType;
        Priority = priority;
    }
}
