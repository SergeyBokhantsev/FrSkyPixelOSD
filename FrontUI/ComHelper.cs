using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontUI
{
    public class ComHelper
    {
        public static string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }

        public static IPort CreatePort(string portName)
        {
            return new COMPort(portName);
        }
    }
}
