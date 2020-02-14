using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontUI
{
    public interface IPort
    {
        bool Available { get; }
        int Read(byte[] buffer, int offset, int count);
        void Write(byte[] buffer, int offset, int count);
        void Flush();
        void Close();
    }

    public class COMPort : IPort
    {
        private readonly SerialPort port;

        public COMPort(string name)
        {
            port = new SerialPort(name, 115200, Parity.None, 8, StopBits.One);
            port.Open();
        }

        public bool Available => port.BytesToRead > 0;

        public void Close()
        {
            port.Close();
        }

        public void Flush()
        {
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return port.Read(buffer, offset, count);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            port.Write(buffer, offset, count);
        }
    }

    public class PortWrapper : IPort
    {
        public event Action<byte[], int, int> Readed;
        public event Action<byte[], int, int> Writen;
        public event Action PortFlush;
        public event Action PortClosed;

        private readonly IPort port;

        public PortWrapper(IPort port)
        {
            this.port = port;
        }

        public bool Available => port.Available;

        public void Close()
        {
            port.Close();
            PortClosed?.Invoke();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            var result = port.Read(buffer, offset, count);
            Readed?.Invoke(buffer, offset, count);
            return result;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            port.Write(buffer, offset, count);
            Writen?.Invoke(buffer, offset, count);
        }

        public void Flush()
        {
            port.Flush();
            PortFlush?.Invoke();
        }
    }
}
