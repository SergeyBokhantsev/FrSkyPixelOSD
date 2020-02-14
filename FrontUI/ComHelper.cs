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
            if (portName.Equals("fake", StringComparison.OrdinalIgnoreCase))
                return new FakePort();

            return new COMPort(portName);
        }

        public static IPort CreateFakePort(string portName)
        {
            return new FakePort();
        }
    }
}
