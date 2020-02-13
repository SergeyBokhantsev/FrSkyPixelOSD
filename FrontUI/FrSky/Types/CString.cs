using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontUI.FrSky.Types
{
    public class CString : OsdType
    {
        public string Value { get; set; }

        public CString(string value)
        {
            Value = value;
        }

        public override int Lenght => Serialize().Count();

        public override IEnumerable<byte> Serialize()
        {
            var bytes = new List<byte>(Encoding.ASCII.GetBytes(Value));
            bytes.Add(0);
            return bytes;
        }
    }
}
