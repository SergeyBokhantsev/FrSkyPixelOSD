using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontUI
{
    public interface IFront
    {
        event Action<string> PortSelected;

        event Action Draw;

        string Status { set; }

        void IncomingData(string str);
        void OutcomingData(string str);
    }
}
