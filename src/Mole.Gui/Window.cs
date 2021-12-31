using System;
using System.Collections.Generic;
using Mole.Shared.Util;

namespace Mole.Gui
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