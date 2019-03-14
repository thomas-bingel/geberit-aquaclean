using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geberit.AquaClean.Core.Message
{
    abstract class Message
    {
        public const byte BLEMSG_ID_CRC_NTF = 6;
        public const byte BLEMSG_ID_CRC_REQ = 4;
        public const byte BLEMSG_ID_CRC_RSP = 5;
        public const byte BLEMSG_ID_PLAIN_NTF = 3;
        public const byte BLEMSG_ID_PLAIN_REQ = 1;
        public const byte BLEMSG_ID_PLAIN_RSP = 2;

        public int ID { get; set; }
        public int Segments { get; set; }

        internal static Message CreateFromStream(byte[] data)
        {
            switch (data[0])
            {
                case BLEMSG_ID_PLAIN_RSP: // 2
                case BLEMSG_ID_PLAIN_NTF: // 3
                    throw new Exception("Generation of 'PlainMessage' not supported yet");
//                    return PlainMessage.createFromStream(data);
                case BLEMSG_ID_CRC_RSP: // 5
                case BLEMSG_ID_CRC_NTF: // 6
                    return CrcMessage.CreateFromBytes(data);
                case BLEMSG_ID_CRC_REQ:
                default:
                    return null;
            }
        }

        internal abstract byte[] Serialize();
    }
}
