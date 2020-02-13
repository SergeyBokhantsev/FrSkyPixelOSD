using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontUI.FrSky.Types
{
    public class Uint16 : OsdType
    {
        public short Value { get; set; }

        public Uint16(short value)
        {
            Value = value;
        }

        public override int Lenght => 2;

        public override IEnumerable<byte> Serialize()
        {
            return new byte[] { (byte)(Value & 0x00FF), (byte)(Value & 0xFF00) };
        }
    }
}
