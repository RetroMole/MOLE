namespace RetroMole.Events
{
    public static class Ui
    {
        public static event Action OnMenuBar;
        public static void TriggerMenuBar() => OnMenuBar?.Invoke();
    }
}
