using System.Collections.Generic;
using Mole.Shared.Util;

namespace Mole.Gui
{
    public abstract class Window
    {
        public bool ShouldDraw = false;
        public abstract void Draw(Project.UiData data, List<Window> windows);
    }
}