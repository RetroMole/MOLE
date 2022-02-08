namespace RetroMole.Core.Interfaces
{
    public interface IRom : IGameModuleComponent, IEnumerable<byte>, IEnumerator<byte>
    { }
}
