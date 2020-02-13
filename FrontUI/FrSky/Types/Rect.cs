using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontUI.FrSky.Types
{
    public class Rect : OsdType
    {
        public Rect(int x, int y, int w, int h)
        {
            Point = new Point(x, y);
            Size = new Point(w, h);
        }

        public Point Point;
        public Point Size;

        public override int Lenght => Point.Lenght + Size.Lenght;

        public override IEnumerable<byte> Serialize()
        {
            return Enumerable.Union(Point.Serialize(), Size.Serialize());
        }
    }
}
