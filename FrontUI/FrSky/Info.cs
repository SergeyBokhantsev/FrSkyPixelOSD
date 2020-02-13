using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontUI.FrSky
{
    public class Info
    {
        public int ID { get; set; }
        public byte VersionMajor { get; set; }
        public byte VersionMinor { get; set; }
        public byte VersionPatch { get; set; }
        public byte GridRows { get; set; }
        public byte GridColumns { get; set; }
        public short PixelWidth { get; set; }
        public short PixelHeight { get; set; }
        public byte TVStandard { get; set; }
        public bool CameraDetected { get; set; }
        public short MaxFrameSize { get; set; }
        public byte ContextStackSize { get; set; }
    }
}
