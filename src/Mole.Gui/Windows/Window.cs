using System.Collections.Generic;

namespace Mole.Gui.Windows
{
    /// <summary>
    /// Window main class
    /// </summary>
    public abstract class Window
    {
        public bool ShouldDraw = false;
        
        public virtual void Draw(Ui.UiData data, List<Window> windows) { }
    }
}