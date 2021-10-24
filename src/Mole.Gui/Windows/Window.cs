using System.Collections.Generic;
using Mole.Shared.Util;

namespace Mole.Gui.Windows
{
    /// <summary>
    /// Window main class
    /// </summary>
    public abstract class Window
    {
        public bool ShouldDraw = false;
        
        public virtual void Draw(Project.UiData data, List<Window> windows) { }
    }
}