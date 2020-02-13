using FrontUI.FrSky;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontUI
{
    public class Controller
    {
        private PixelOSD osd;
        private PortWrapper port;
        private readonly IFront front;

        public Controller(IFront front)
        {
            this.front = front;

            front.PortSelected += port => CreateOsd(port);
            front.Draw += Front_Draw;
        }

        private void CreateOsd(string portName)
        {
            try
            {
                osd?.Close();
                port = new PortWrapper(ComHelper.CreatePort(portName));

                port.Writen += Port_Writen;
                port.Readed += Port_Readed;
                port.PortFlush += Port_PortFlush;

                osd = new PixelOSD(port);
                front.Status = "PixelOSD created";                
            }
            catch (Exception ex)
            {
                front.Status = ex.Message;
            }
        }

        private void Port_PortFlush()
        {
            front.OutcomingData(Environment.NewLine);
        }

        private void Port_Readed(byte[] buffer, int offset, int count)
        {
            var str = ToHexString(buffer, offset, count);
            front.IncomingData(str);
        }

        private void Port_Writen(byte[] buffer, int offset, int count)
        {
            var str = ToHexString(buffer, offset, count);
            front.OutcomingData(str);
        }

        private void Front_Draw()
        {
            if (null != osd)
            {
                osd.TransactionBegin();
                osd.MoveCursor(10, 10);
                osd.DrawLine(10, 20);
            }
            else
            {
                front.Status = "No OSD";
            }
        }

        private string ToHexString(byte[] buffer, int offset, int count)
        {
            if (count == 0)
                return string.Empty;

            var sb = new StringBuilder();

            for(int i=0; i<count; ++i)
            {
                sb.Append((buffer[i + offset]).ToString());
                sb.Append(" ");
            }

            double elapsedMs = (double)count * 12d / 115200d * 1000 + 1;
            sb.Append($" ({count} b / {elapsedMs.ToString("0")} ms)");

            return sb.ToString();
        }
    }
}
