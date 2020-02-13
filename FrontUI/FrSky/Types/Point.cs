using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontUI.FrSky.Types
{
    public class Point : OsdType
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override int Lenght => 3;

        public override IEnumerable<byte> Serialize()
        {
            int xLim = X & 0x07FF;
            if (X < 0) xLim = xLim | 0x0800;

            int yLim = Y & 0x07FF;
            if (Y < 0) yLim = yLim | 0x0800;

            int i = xLim << 12;
            i = i | yLim;

            byte a = (byte)((i & 0xFF0000) >> 16);
            byte b = (byte)((i & 0x00FF00) >> 8);
            byte c = (byte)(i & 0x0000FF);

            return new[] { a, b, c };
        }
    }
}
