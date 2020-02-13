using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontUI.FrSky.Types
{
    public class Uvariant : OsdType
    {
        public int Value { get; set; }

        public Uvariant(int value)
        {
            Value = value;

            if (value < 0)
                throw new InvalidOperationException();
        }

        public override int Lenght => Serialize().Count();

        public override IEnumerable<byte> Serialize()
        {
            int val = Value;
            var ptr = new List<byte>();

            while (val > 0x80)
            {
                ptr.Add((byte)((val & 0xFF) | 0x80));
                val >>= 7;
            }

            ptr.Add((byte)(val & 0xFF));

            return ptr;
        }
    }
}
