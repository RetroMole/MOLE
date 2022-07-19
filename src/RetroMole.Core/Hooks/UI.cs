namespace RetroMole.Core.Hooks;

public static class UI
{
    public static event Action OnMainMenuBar = () => { };
    public static void TriggerMainMenuBar() => OnMainMenuBar?.Invoke();
}