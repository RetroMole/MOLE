using RetroMole.Core.Utility;
using Serilog;

namespace RetroMole.Core.Interfaces
{
    public abstract class WindowBase
    {
        protected bool ShouldDraw = false;
        public bool IsOpen { get => ShouldDraw; }
        public event Action<WindowBase> OnClose;
        public virtual void TriggerClose() => OnClose?.Invoke(this);
        public abstract void Draw(Project.UiData data, Dictionary<string, WindowBase> windows);
        public void Open() { ShouldDraw = true; Log.Information("Opened Window {0}", GetType().Name); }
    }
}