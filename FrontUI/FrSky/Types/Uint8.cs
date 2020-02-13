using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontUI.FrSky.Types
{
    public class Uint8 : OsdType
    {
        public byte Value { get; set; }

        public Uint8(byte value)
        {
            Value = value;
        }

        public override int Lenght => 1;

        public override IEnumerable<byte> Serialize()
        {
            return Enumerable.Repeat(Value, 1);
        }
    }
}
