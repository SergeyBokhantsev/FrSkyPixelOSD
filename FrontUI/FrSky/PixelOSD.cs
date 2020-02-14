using FrontUI.FrSky.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrontUI.FrSky
{
    public class PixelOSD
    {
        public enum Color : byte
        {
            Black = 0,
            Transparent = 1,
            White = 2,
            Gray = 3
        };

        [Flags]
        public enum Outline : byte
        {
            None = 0,
            Top = 1 << 0,
            Right = 1 << 1,
            Bottom = 1 << 2,
            Left = 1 << 3,
        }

        private readonly byte[] Header = new byte[] { (byte)'$', (byte)'A' };

        private enum IncomingState { Hdr1Waiting, Hdr2Waiting, DataReading }

        private IncomingState incomingState;
        private byte[] incomingPayload = new byte[128];
        private int incomingDataLen;

        private enum CMD : byte
        {
            OSD_CMD_RESPONSE_ERROR = 0,

            OSD_CMD_INFO = 1,
            OSD_CMD_READ_FONT = 2,
            OSD_CMD_WRITE_FONT = 3,
            OSD_CMD_GET_CAMERA = 4,
            OSD_CMD_SET_CAMERA = 5,
            OSD_CMD_GET_ACTIVE_CAMERA = 6,
            OSD_CMD_GET_OSD_ENABLED = 7,
            OSD_CMD_SET_OSD_ENABLED = 8,

            OSD_CMD_TRANSACTION_BEGIN = 16,
            OSD_CMD_TRANSACTION_COMMIT = 17,
            OSD_CMD_TRANSACTION_BEGIN_PROFILED = 18,
            OSD_CMD_TRANSACTION_BEGIN_RESET_DRAWING = 19,

            OSD_CMD_DRAWING_SET_STROKE_COLOR = 22,
            OSD_CMD_DRAWING_SET_FILL_COLOR = 23,
            OSD_CMD_DRAWING_SET_STROKE_AND_FILL_COLOR = 24,
            OSD_CMD_DRAWING_SET_COLOR_INVERSION = 25,
            OSD_CMD_DRAWING_SET_PIXEL = 26,
            OSD_CMD_DRAWING_SET_PIXEL_TO_STROKE_COLOR = 27,
            OSD_CMD_DRAWING_SET_PIXEL_TO_FILL_COLOR = 28,
            OSD_CMD_DRAWING_SET_STROKE_WIDTH = 29,
            OSD_CMD_DRAWING_SET_LINE_OUTLINE_TYPE = 30,
            OSD_CMD_DRAWING_SET_LINE_OUTLINE_COLOR = 31,

            OSD_CMD_DRAWING_CLIP_TO_RECT = 40,
            OSD_CMD_DRAWING_CLEAR_SCREEN = 41,
            OSD_CMD_DRAWING_CLEAR_RECT = 42,
            OSD_CMD_DRAWING_RESET = 43,
            OSD_CMD_DRAWING_DRAW_BITMAP = 44,
            OSD_CMD_DRAWING_DRAW_BITMAP_MASK = 45,
            OSD_CMD_DRAWING_DRAW_CHAR = 46,
            OSD_CMD_DRAWING_DRAW_CHAR_MASK = 47,
            OSD_CMD_DRAWING_DRAW_STRING = 48,
            OSD_CMD_DRAWING_DRAW_STRING_MASK = 49,
            OSD_CMD_DRAWING_MOVE_TO_POINT = 50,
            OSD_CMD_DRAWING_STROKE_LINE_TO_POINT = 51,
            OSD_CMD_DRAWING_STROKE_TRIANGLE = 52,
            OSD_CMD_DRAWING_FILL_TRIANGLE = 53,
            OSD_CMD_DRAWING_FILL_STROKE_TRIANGLE = 54,
            OSD_CMD_DRAWING_STROKE_RECT = 55,
            OSD_CMD_DRAWING_FILL_RECT = 56,
            OSD_CMD_DRAWING_FILL_STROKE_RECT = 57,
            OSD_CMD_DRAWING_STROKE_ELLIPSE_IN_RECT = 58,
            OSD_CMD_DRAWING_FILL_ELLIPSE_IN_RECT = 59,
            OSD_CMD_DRAWING_FILL_STROKE_ELLIPSE_IN_RECT = 60,

            OSD_CMD_CTM_RESET = 80,
            OSD_CMD_CTM_SET = 81,
            OSD_CMD_CTM_TRANSLATE = 82,
            OSD_CMD_CTM_SCALE = 83,
            OSD_CMD_CTM_ROTATE = 84,
            OSD_CMD_CTM_ROTATE_ABOUT = 85,
            OSD_CMD_CTM_SHEAR = 86,
            OSD_CMD_CTM_SHEAR_ABOUT = 87,
            OSD_CMD_CTM_MULTIPLY = 88,

            OSD_CMD_CONTEXT_PUSH = 100,
            OSD_CMD_CONTEXT_POP = 101,

            // MAX7456 emulation commands
            OSD_CMD_DRAW_GRID_CHR = 110,
            OSD_CMD_DRAW_GRID_STR = 111,
        };

        public void Close()
        {
            lock (port)
            {
                port.Close();
                port = null;
            }
        }

        private IPort port;

        public Info Info { get; }

        public PixelOSD(IPort port)
        {
            this.port = port ?? throw new ArgumentNullException(nameof(port));
            Info = new Info();
        }

        public void RequiestInfo()
        {
            Send(CMD.OSD_CMD_INFO, new Uint8(1));
        }

        public void TransactionBegin()
        {
            Send(CMD.OSD_CMD_TRANSACTION_BEGIN);
        }
        
        public void TransactionBeginProfiled(int x, int y)
        {
            Send(CMD.OSD_CMD_TRANSACTION_BEGIN_PROFILED, new Point(x, y));
        }

        public void TransactionBeginResetDrawing()
        {
            Send(CMD.OSD_CMD_TRANSACTION_BEGIN_RESET_DRAWING);
        }

        public void TransactionCommit()
        {
            Send(CMD.OSD_CMD_TRANSACTION_COMMIT);
        }

        public void SetStrokeColor(Color color)
        {
            Send(CMD.OSD_CMD_DRAWING_SET_STROKE_COLOR, new Uint8((byte)color));
        }

        public void SetFillColor(Color color)
        {
            Send(CMD.OSD_CMD_DRAWING_SET_FILL_COLOR, new Uint8((byte)color));
        }

        public void SetStrokeAndFillColor(Color color)
        {
            Send(CMD.OSD_CMD_DRAWING_SET_STROKE_AND_FILL_COLOR, new Uint8((byte)color));
        }

        public void SetColorInversion(bool enable)
        {
            Send(CMD.OSD_CMD_DRAWING_SET_COLOR_INVERSION, new Uint8(enable ? (byte)1 : (byte)0));
        }

        public void SetStrokeWidth(byte width)
        {
            Send(CMD.OSD_CMD_DRAWING_SET_STROKE_WIDTH, new Uint8(width));
        }

        public void SetLineOutlineType(Outline outline)
        {
            Send(CMD.OSD_CMD_DRAWING_SET_LINE_OUTLINE_TYPE, new Uint8((byte)outline));
        }

        public void SetLineOutlineColor(Color color)
        {
            Send(CMD.OSD_CMD_DRAWING_SET_LINE_OUTLINE_COLOR, new Uint8((byte)color));
        }

        public void ClipToRect(int x, int y, int w, int h)
        {
            Send(CMD.OSD_CMD_DRAWING_CLIP_TO_RECT, new Rect(x, y, w, h));
        }

        public void ClearScreen()
        {
            Send(CMD.OSD_CMD_DRAWING_CLEAR_SCREEN);
        }

        public void ClearRect(int x, int y, int w, int h)
        {
            Send(CMD.OSD_CMD_DRAWING_CLEAR_RECT, new Rect(x, y, w, h));
        }

        public void MoveCursor(int x, int y)
        {
            Send(CMD.OSD_CMD_DRAWING_MOVE_TO_POINT, new Point(x, y));
        }

        public void DrawPixel(int x, int y, Color color)
        {
            Send(CMD.OSD_CMD_DRAWING_SET_PIXEL, new Point(x, y), new Uint8((byte)color));
        }

        public void DrawPixelStroke(int x, int y)
        {
            Send(CMD.OSD_CMD_DRAWING_SET_PIXEL_TO_STROKE_COLOR, new Point(x, y));
        }

        public void DrawPixelFill(int x, int y)
        {
            Send(CMD.OSD_CMD_DRAWING_SET_PIXEL_TO_FILL_COLOR, new Point(x, y));
        }

        public void DrawLine(int x, int y)
        {
            Send(CMD.OSD_CMD_DRAWING_STROKE_LINE_TO_POINT, new Point(x, y));
        }

        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, bool stroke, bool fill)
        {
            CMD cmd = stroke && fill ? CMD.OSD_CMD_DRAWING_FILL_STROKE_TRIANGLE
                      : stroke ? CMD.OSD_CMD_DRAWING_STROKE_TRIANGLE
                      : fill ? CMD.OSD_CMD_DRAWING_FILL_TRIANGLE
                      : default(CMD);

            if (cmd != default(CMD))
            {
                Send(cmd, new Point(x1, y1), new Point(x2, y2), new Point(x3, y3));
            }
        }

        public void DrawRectangle(int x, int y, int w, int h, bool stroke, bool fill)
        {
            CMD cmd = stroke && fill ? CMD.OSD_CMD_DRAWING_FILL_STROKE_RECT
                      : stroke ? CMD.OSD_CMD_DRAWING_STROKE_RECT
                      : fill ? CMD.OSD_CMD_DRAWING_FILL_RECT
                      : default(CMD);

            if (cmd != default(CMD))
            {
                Send(cmd, new Rect(x, y, w, h));
            }
        }

        public void DrawChar(int x, int y, short ch)
        {
            Send(CMD.OSD_CMD_DRAWING_DRAW_CHAR, new Point(x, y), new Uint16(ch), new BitmapOpts());
        }

        public void DrawCharMask(int x, int y, short ch, Color color)
        {
            Send(CMD.OSD_CMD_DRAWING_DRAW_CHAR_MASK, new Point(x, y), new Uint16(ch), new BitmapOpts(), new Uint8((byte)color));
        }

        public void DrawString(int x, int y, string str)
        {
            Send(CMD.OSD_CMD_DRAWING_DRAW_STRING, new Point(x, y), new BitmapOpts(), new Uvariant(str.Length), new CString(str));
        }

        public void DrawStringMask(int x, int y, string str, Color color)
        {
            Send(CMD.OSD_CMD_DRAWING_DRAW_STRING_MASK, new Point(x, y), new BitmapOpts(), new Uint8((byte)color), new Uvariant(str.Length), new CString(str));
        }

        private void Send(CMD command, params OsdType[] parameters)
        {
            lock (port)
            {
                var len = new Types.Uvariant(1 /*cmd*/ + Enumerable.Sum(parameters, p => p.Lenght) + 0 /*crc*/);

                var frame = new List<byte>(64);
                frame.AddRange(len.Serialize());
                frame.Add((byte)command);

                foreach (var p in parameters)
                    frame.AddRange(p.Serialize());

                var crc = Crc.CreateCrc8DvbS2();
                crc.Append(frame.ToArray());

                frame.AddRange(crc.ToByteArray());

                port.Write(Header, 0, Header.Length);
                //port.Write(len.Serialize().ToArray(), 0, len.Serialize().Count());
                port.Write(frame.ToArray(), 0, frame.Count);
                port.Flush();
            }
        }

        public void Read()
        {
            lock (port)
            {
                while (port.Available)
                {
                    var buffer = new byte[1];

                    if (1 != port.Read(buffer, 0, 1))
                        throw new Exception();

                    switch (incomingState)
                    {
                        case IncomingState.Hdr1Waiting:
                            if (buffer[0] == (byte)'$')
                                incomingState = IncomingState.Hdr2Waiting;
                            break;

                        case IncomingState.Hdr2Waiting:
                            if (buffer[0] == (byte)'A')
                                incomingState = IncomingState.DataReading;
                            else
                                incomingState = IncomingState.Hdr1Waiting;
                            break;

                        default:
                            incomingPayload[incomingDataLen++] = buffer[0];
                            if (ParseResponse())
                                incomingState = IncomingState.Hdr1Waiting;
                            break;
                    }
                }
            }
        }

        private bool ParseResponse()
        {
            var cmd = (CMD)incomingPayload[0];

            switch (cmd)
            {
                case CMD.OSD_CMD_INFO:
                    return ParseInfo();

                default:
                    return true;
            }
        }

        private bool ParseInfo()
        {
            if (incomingDataLen < 18)
                return false;

            if (incomingDataLen > 18)
                return true;

            var payloadNoCrc = incomingPayload.Take(17).ToList();

            if (Crc_(payloadNoCrc) == incomingPayload[17])
            {
                Info.VersionMajor = incomingPayload[3];
                Info.VersionMinor = incomingPayload[4];
                Info.VersionPatch = incomingPayload[5];
                Info.GridRows = incomingPayload[6];
                Info.GridColumns = incomingPayload[7];
                Info.PixelWidth = FromLittleEndian(incomingPayload[8], incomingPayload[9]);
                Info.PixelHeight = FromLittleEndian(incomingPayload[10], incomingPayload[11]);
                Info.TVStandard = incomingPayload[12];
                Info.CameraDetected = incomingPayload[13] > 0;
                Info.MaxFrameSize = FromLittleEndian(incomingPayload[14], incomingPayload[15]);
                Info.ContextStackSize = incomingPayload[16];
                Info.ID++;
            }

            return true;
        }

        private short FromLittleEndian(byte v1, byte v2)
        {
            return (short)((v2 << 8) | v1);
        }

        private byte Crc_(List<byte> payload)
        {
            byte crc = 0;

            foreach (var a in payload)
            {
                crc ^= a;
                for (int ii = 0; ii < 8; ++ii)
                {
                    if ((crc & 0x80) > 0)
                    {
                        crc = (byte)((byte)(crc << 1) ^ 0xD5);
                    }
                    else
                    {
                        crc = (byte)(crc << 1);
                    }
                }
            }

            return crc;
        }
    }
}
