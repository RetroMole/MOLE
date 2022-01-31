using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroMole.Core.Interfaces
{
    public interface IGameModule
    {
        public string Name { get; }
        public string Description { get; }
        public Dictionary<string, IGameModuleComponent> Components { get; }
        public Dictionary<string, WindowBase> Windows { get; }
    }
}
