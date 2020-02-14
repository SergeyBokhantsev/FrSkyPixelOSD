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

        public static int Deserialize(byte[] ptr, int offset, int count, out int value)
        {
            int s = 0;
            value = 0;
            for (int ii = 0; ii < count - offset; ii++)
            {
                byte b = ptr[ii + offset];
                if (b < 0x80)
                {
                    if (ii > 5 || (ii == 5 && b > 1))
                    {
                        // uint32_t overflow
                        return -2;
                    }
                    value |= ((int)b) << s;
                    return ii + 1;
                }
                value |= ((int)(b & 0x7f)) << s;
                s += 7;
            }
            // no value could be decoded and we have no data left
            return -1;
        }
    }
}
