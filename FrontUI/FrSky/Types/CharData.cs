using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontUI.FrSky.Types
{
    public class CharData : OsdType
    {
        public byte[] Data { get; } = new byte[54];
        public byte[] Meta { get; } = new byte[10];

        public CharData()
        {
        }

        public CharData(byte[] data, byte[] meta)
        {
            if (data.Length != 54)
                throw new ArgumentException(nameof(data));

            if (meta.Length != 10)
                throw new ArgumentException(nameof(meta));

            Data = data;
            Meta = meta;
        }

        public override int Lenght => Data.Length + Meta.Length;

        public override IEnumerable<byte> Serialize()
        {
            return Data.Concat(Meta);
        }
    }
}
