using System;
using System.Collections.Generic;
using RetroMole.Core.Utility;

namespace RetroMole.Core.Interfaces
{
    public abstract class WindowBase
    {
        protected bool ShouldDraw = false;
        public event Action<WindowBase> Close;
        protected virtual void OnClose(WindowBase w) => Close?.Invoke(w);
        public abstract void Draw(Project.UiData data, Dictionary<string, WindowBase> windows);
        public void Open() { ShouldDraw = true; }
    }
}