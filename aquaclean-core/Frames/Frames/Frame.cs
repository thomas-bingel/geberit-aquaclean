using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geberit.AquaClean.Core.Frames
{

    abstract class Frame
    {
        private const int BLE_PAYLOADLEN = 20;

        internal bool HasMessageTypeByte_b4 { get; set; }
        internal int SubFrameCountOrIndex { get; set; }
        internal bool IsSubFrameCount { get; set; }

        public FrameType FrameType { get; set; }

        protected byte[] SerializeHdr()
        {
            byte[] data = new byte[BLE_PAYLOADLEN];
            data[0] = 0; // Info Header ???
            data[0] |= (byte)((int)FrameType << 5);
            if (this.HasMessageTypeByte_b4)
            {
                data[0] = (byte)(data[0] | 16);
            }

            data[0] |= (byte)(SubFrameCountOrIndex << 1); ;
            if (this.IsSubFrameCount)
            {
                data[0] = (byte)(data[0] | 1);
            }

            return data;
        }

        abstract public byte[] Serialize();
    }
}
