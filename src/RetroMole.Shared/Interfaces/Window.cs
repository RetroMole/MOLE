using System;
using System.Collections.Generic;
using RetroMole.Core.Utility;
using Serilog;

namespace RetroMole.Core.Interfaces
{
    public abstract class WindowBase
    {
        protected bool ShouldDraw = false;
        public bool IsOpen { get => ShouldDraw; }
        public event Action<WindowBase> Close;
        public virtual void OnClose(WindowBase w) => Close?.Invoke(w);
        public abstract void Draw(Project.UiData data, Dictionary<string, WindowBase> windows);
        public void Open() { ShouldDraw = true; Log.Information("Opened Window {0}", GetType().Name); }
    }
}