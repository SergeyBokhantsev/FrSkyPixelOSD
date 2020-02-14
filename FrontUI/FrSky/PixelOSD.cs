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

        private enum IncomingState { Hdr1Waiting, Hdr2Waiting, FrameLenReading, DataReading, CrcReading }

        private List<byte> outcomingBuffer = new List<byte>();

        private IncomingState incomingState;
        private DateTime lastRead;
        private byte[] incomingBuffer = new byte[128];
        private int incomingBufferLen;

        // Frame is: [SIZE bytes (variable)][CMD (1 byte)][Payload (variable)]
        // CRC is applied to whole Frame
        private int payloadOffset;
        private int payloadSize;
        
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

        public int LastErrorOnCommand { get; private set; }
        public int LastErrorCode { get; private set; }
        public int ErrorCount { get; private set; }

        public Info Info { get; }

        public CharData FontInfo { get; }

        public PixelOSD(IPort port)
        {
            this.port = port ?? throw new ArgumentNullException(nameof(port));
            Info = new Info();
            FontInfo = new CharData();
        }

        public void RequiestInfo()
        {
            PrepareToSend(CMD.OSD_CMD_INFO, new Uint8(1));
        }

        public void ReadFont(short ch)
        {
            PrepareToSend(CMD.OSD_CMD_READ_FONT, new Uint16(ch));
        }

        public void WriteFont(short ch, byte[] data, byte[] meta)
        {
            PrepareToSend(CMD.OSD_CMD_WRITE_FONT, new Uint16(ch), new CharData(data, meta));
        }

        public void SetOsdEnabled(bool enable)
        {
            PrepareToSend(CMD.OSD_CMD_SET_OSD_ENABLED, new Uint8(enable ? (byte)1 : (byte)0));
        }

        public void TransactionBegin()
        {
            PrepareToSend(CMD.OSD_CMD_TRANSACTION_BEGIN);
        }
        
        public void TransactionBeginProfiled(int x, int y)
        {
            PrepareToSend(CMD.OSD_CMD_TRANSACTION_BEGIN_PROFILED, new Point(x, y));
        }

        public void TransactionBeginResetDrawing()
        {
            PrepareToSend(CMD.OSD_CMD_TRANSACTION_BEGIN_RESET_DRAWING);
        }

        public void TransactionCommit()
        {
            PrepareToSend(CMD.OSD_CMD_TRANSACTION_COMMIT);
        }

        public void SetStrokeColor(Color color)
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_SET_STROKE_COLOR, new Uint8((byte)color));
        }

        public void SetFillColor(Color color)
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_SET_FILL_COLOR, new Uint8((byte)color));
        }

        public void SetStrokeAndFillColor(Color color)
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_SET_STROKE_AND_FILL_COLOR, new Uint8((byte)color));
        }

        public void SetColorInversion(bool enable)
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_SET_COLOR_INVERSION, new Uint8(enable ? (byte)1 : (byte)0));
        }

        public void SetStrokeWidth(byte width)
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_SET_STROKE_WIDTH, new Uint8(width));
        }

        public void SetLineOutlineType(Outline outline)
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_SET_LINE_OUTLINE_TYPE, new Uint8((byte)outline));
        }

        public void SetLineOutlineColor(Color color)
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_SET_LINE_OUTLINE_COLOR, new Uint8((byte)color));
        }

        public void ClipToRect(int x, int y, int w, int h)
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_CLIP_TO_RECT, new Rect(x, y, w, h));
        }

        public void ClearScreen()
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_CLEAR_SCREEN);
        }

        public void ClearRect(int x, int y, int w, int h)
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_CLEAR_RECT, new Rect(x, y, w, h));
        }

        public void MoveCursor(int x, int y)
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_MOVE_TO_POINT, new Point(x, y));
        }

        public void DrawPixel(int x, int y, Color color)
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_SET_PIXEL, new Point(x, y), new Uint8((byte)color));
        }

        public void DrawPixelStroke(int x, int y)
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_SET_PIXEL_TO_STROKE_COLOR, new Point(x, y));
        }

        public void DrawPixelFill(int x, int y)
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_SET_PIXEL_TO_FILL_COLOR, new Point(x, y));
        }

        public void DrawLine(int x, int y)
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_STROKE_LINE_TO_POINT, new Point(x, y));
        }

        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, bool stroke, bool fill)
        {
            CMD cmd = stroke && fill ? CMD.OSD_CMD_DRAWING_FILL_STROKE_TRIANGLE
                      : stroke ? CMD.OSD_CMD_DRAWING_STROKE_TRIANGLE
                      : fill ? CMD.OSD_CMD_DRAWING_FILL_TRIANGLE
                      : default(CMD);

            if (cmd != default(CMD))
            {
                PrepareToSend(cmd, new Point(x1, y1), new Point(x2, y2), new Point(x3, y3));
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
                PrepareToSend(cmd, new Rect(x, y, w, h));
            }
        }

        public void DrawChar(int x, int y, short ch)
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_DRAW_CHAR, new Point(x, y), new Uint16(ch), new BitmapOpts());
        }

        public void DrawCharMask(int x, int y, short ch, Color color)
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_DRAW_CHAR_MASK, new Point(x, y), new Uint16(ch), new BitmapOpts(), new Uint8((byte)color));
        }

        public void DrawString(int x, int y, string str)
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_DRAW_STRING, new Point(x, y), new BitmapOpts(), new Uvariant(str.Length), new CString(str));
        }

        public void DrawStringMask(int x, int y, string str, Color color)
        {
            PrepareToSend(CMD.OSD_CMD_DRAWING_DRAW_STRING_MASK, new Point(x, y), new BitmapOpts(), new Uint8((byte)color), new Uvariant(str.Length), new CString(str));
        }

        private void PrepareToSend(CMD command, params OsdType[] parameters)
        {
            var len = new Types.Uvariant(1 /*cmd*/ + Enumerable.Sum(parameters, p => p.Lenght) + 0 /*crc*/);

            var frame = new List<byte>(64);
            frame.AddRange(len.Serialize());
            frame.Add((byte)command);

            foreach (var p in parameters)
                frame.AddRange(p.Serialize());

            byte crc = Crc_(frame, 0, frame.Count);

            frame.Add(crc);

            lock (outcomingBuffer)
            {
                outcomingBuffer.AddRange(Header);
                outcomingBuffer.AddRange(frame);
            }
        }

        private void Send()
        {
            byte[] bytes;

            lock (outcomingBuffer)
            {
                bytes = outcomingBuffer.ToArray();
                outcomingBuffer.Clear();
            }

            if (bytes.Any())
            {
                port.Write(bytes, 0, bytes.Length);
                port.Flush();
            }
        }

        public void Tick()
        {
            Send();
            Read();
        }

        private void Read()
        {
            int readed = 0;

            while (port.Available && readed < 5)
            {
                var buffer = new byte[1];

                if (1 != port.Read(buffer, 0, 1))
                    throw new Exception();

                readed++;

                if (incomingBufferLen == incomingBuffer.Length)
                {
                    incomingState = IncomingState.Hdr1Waiting;
                    incomingBufferLen = 0;
                }

                lastRead = DateTime.Now;

                switch (incomingState)
                {
                    case IncomingState.Hdr1Waiting:
                        if (buffer[0] == (byte)'$')
                            incomingState = IncomingState.Hdr2Waiting;
                        break;

                    case IncomingState.Hdr2Waiting:
                        if (buffer[0] == (byte)'A')
                            incomingState = IncomingState.FrameLenReading;
                        else
                            incomingState = default(IncomingState);
                        break;

                    case IncomingState.FrameLenReading:
                        incomingBuffer[incomingBufferLen++] = buffer[0];
                        if (ParseResponseLen())
                        {
                            if (payloadOffset + payloadSize <= incomingBuffer.Length)
                                incomingState = IncomingState.DataReading;
                            else
                                incomingState = IncomingState.Hdr1Waiting;
                        }
                        break;

                    case IncomingState.DataReading:
                        incomingBuffer[incomingBufferLen++] = buffer[0];
                        if (incomingBufferLen == payloadOffset + payloadSize)
                            incomingState = IncomingState.CrcReading;
                        break;

                    case IncomingState.CrcReading:
                        incomingState = IncomingState.Hdr1Waiting;
                        byte crc = buffer[0];
                        if (Crc_(incomingBuffer, 0, incomingBufferLen) == crc)
                            ParseResponse();
                        break;
                }
            }
        }

        private bool ParseResponseLen()
        {
            int commandOffset = Uvariant.Deserialize(incomingBuffer, 0, incomingBufferLen, out int cmdAndPayloadSize);

            if (commandOffset > 0)
            {
                payloadOffset = commandOffset + 1/*cmd*/;
                payloadSize = cmdAndPayloadSize - 1/*cmd*/;
                return true;
            }
            else
                return false;
        }

        private void ParseResponse()
        {
            var cmd = (CMD)incomingBuffer[payloadOffset - 1/*cmd*/];

            switch (cmd)
            {
                case CMD.OSD_CMD_RESPONSE_ERROR:
                    ParseError(payloadOffset, payloadSize);
                    break;

                case CMD.OSD_CMD_INFO:
                    ParseInfo(payloadOffset, payloadSize);
                    break;

                case CMD.OSD_CMD_READ_FONT:
                case CMD.OSD_CMD_WRITE_FONT:
                    ParseFont(payloadOffset, payloadSize);
                    break;
            }
        }

        private void ParseError(int payloadOffset, int payloadSize)
        {
            ErrorCount++;

            if (payloadSize >= 2)
            {
                LastErrorOnCommand = incomingBuffer[payloadOffset];
                LastErrorCode = incomingBuffer[payloadOffset + 1];
            }
        }

        private void ParseFont(int payloadOffset, int payloadSize)
        {
            if (payloadSize == 66)
            {
                Array.Copy(incomingBuffer, payloadOffset, FontInfo.Data, 0, 54);
                Array.Copy(incomingBuffer, payloadOffset + 54, FontInfo.Meta, 0, 10);
            }
        }

        private void ParseInfo(int offset, int size)
        {
            if (size == 17)
            {
                Info.VersionMajor = incomingBuffer[offset + 3];
                Info.VersionMinor = incomingBuffer[offset + 4];
                Info.VersionPatch = incomingBuffer[offset + 5];
                Info.GridRows = incomingBuffer[offset + 6];
                Info.GridColumns = incomingBuffer[offset + 7];
                Info.PixelWidth = FromLittleEndian(incomingBuffer[offset + 8], incomingBuffer[offset + 9]);
                Info.PixelHeight = FromLittleEndian(incomingBuffer[offset + 10], incomingBuffer[offset + 11]);
                Info.TVStandard = incomingBuffer[offset + 12];
                Info.CameraDetected = incomingBuffer[offset + 13] > 0;
                Info.MaxFrameSize = FromLittleEndian(incomingBuffer[offset + 14], incomingBuffer[offset + 15]);
                Info.ContextStackSize = incomingBuffer[offset + 16];
                Info.ID++;
            }
        }

        private short FromLittleEndian(byte v1, byte v2)
        {
            return (short)((v2 << 8) | v1);
        }

        private byte Crc_(IEnumerable<byte> payload, int offset, int count)
        {
            byte crc = 0;

            foreach (var a in payload.Skip(offset).Take(count))
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
