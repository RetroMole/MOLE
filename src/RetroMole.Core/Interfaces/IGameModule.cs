namespace RetroMole.Core.Interfaces
{
    public interface IGameModule
    {
        public string Name { get; }
        public string Description { get; }
        public Dictionary<string, IGameModuleComponent> Components { get; }
        public Dictionary<string, WindowBase> Windows { get; }
        public void HookEvents();
    }
}
