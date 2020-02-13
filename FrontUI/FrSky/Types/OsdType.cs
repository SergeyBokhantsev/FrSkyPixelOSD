using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontUI.FrSky.Types
{
    public abstract class OsdType
    {
        public abstract int Lenght { get; }
        public abstract IEnumerable<byte> Serialize();        
    }
}
