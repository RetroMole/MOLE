using System;
using System.Collections.Generic;
using Mole.Shared.Util;

namespace Mole.Gui
{
    public abstract class Window
    {
        public bool ShouldDraw = false;
        public event Action<Window> Close;
        protected virtual void OnClose(Window w) => Close?.Invoke(w);
        public abstract void Draw(Project.UiData data, Dictionary<string, Window> windows);
    }
}