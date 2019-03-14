using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geberit.AquaClean.Core.Message
{
    class CrcMessage : Message
    {
        public int Crc16Hi { get; private set; }
        public int Crc16Lo { get; private set; }
        public int LenHi { get; set; }
        public int LenLo { get; set; }
        public byte[] Body { get; set; } = new byte[256];

        public static CrcMessage CreateFromBytes(byte[] data)
        {
            CrcMessage message = new CrcMessage() {
                ID = data[0],
                Segments = data[1],
                LenHi = data[2],
                LenLo = data[3],
                Crc16Hi = data[4],
                Crc16Lo = data[5],
            };
            
            Array.Copy(data, 6, message.Body, 0, Math.Min(message.LenHi * 256 + message.LenLo, 256));
            return message;
        }

        internal static CrcMessage Create(int messageId, int messageSegment, byte[] data)
        {
            CrcMessage crcMessage = new CrcMessage();
            crcMessage.ID = messageId;
            crcMessage.Segments = messageSegment;
            Array.Copy(data, 0, crcMessage.Body, 0, data.Length);
            crcMessage.LenHi = (byte)(data.Length / 256);
            crcMessage.LenLo = (byte)(data.Length % 256);

            int var7 = crcMessage.Crc16Calculation(crcMessage.Body, data.Length);
            crcMessage.Crc16Hi = (byte)(var7 / 256);
            crcMessage.Crc16Lo = (byte)(var7 % 256);

            return crcMessage;
        }

        internal static int SizeOfHeader()
        {
            return 6;
        }


        internal override byte[] Serialize()
        {
            byte[] var3 = new byte[262];
            int var2 = 0 + 1;
            var3[0] = (byte)ID;
            int var1 = var2 + 1;
            var3[var2] = (byte)Segments;
            var2 = var1 + 1;
            var3[var1] = (byte)LenHi;
            var1 = var2 + 1;
            var3[var2] = (byte)LenLo;
            var2 = var1 + 1;
            var3[var1] = (byte)Crc16Hi;
            var3[var2] = (byte)Crc16Lo;
            Array.Copy(Body, 0, var3, var2 + 1, 256);
            return var3;
        }

        internal bool IsValid
        {
            get
            {
                int length = (LenHi << 8) + LenLo;
                int i2 = Crc16Calculation(this.Body, length);
                return i2 == (Crc16Hi << 8) + Crc16Lo;
            }
        }

        private int Crc16Calculation(byte[] data, int length)
        {
            int i2 = 4660;
            for (int i3 = 0; i3 < length; i3 = (short)(i3 + 1))
            {
                i2 = ((((i2 << 8) & 65280) | ((i2 >> 8) & 255)) ^ (data[i3] & 255)) & 65535;
                i2 = (i2 ^ ((i2 & 255) >> 4)) & 65535;
                i2 = (i2 ^ ((i2 << 8) << 4)) & 65535;
                i2 = (i2 ^ (((i2 & 255) << 4) << 1)) & 65535;
            }

            return i2;
        }

        public override string ToString()
        {
            return String.Format("ID='{0}', Segments='{1}', LenHi='{2}', LenLo='{3}', Crc16Hi='{4}', Crc16Low='{5}'",
                ID, Segments, LenHi, LenLo, Crc16Hi, Crc16Lo);
        }

    }
}
