using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LA_Back
{
    public class GFXHandler : ROMHandler
    {
        const uint GFXoffset = 0x8000;

		public GFXHandler(string Name) : base(Name)
        {
            
        }
    }
}
