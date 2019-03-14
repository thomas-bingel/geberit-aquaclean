using System;

namespace Geberit.AquaClean.Core.Frames
{
    internal class FirstConsFrame : Frame
    {
        private const int PAYLOAD_LENGTH = 18;
        public byte[] Payload { get; private set; } = new byte[PAYLOAD_LENGTH];
        public byte FrameCountOrNuber { get; private set; }

        public static FirstConsFrame CreateFirstConsFrame(byte[] data)
        {
            // TODO: Will be overwritten from factory
            FirstConsFrame frame = new FirstConsFrame
            {
                IsSubFrameCount = (data[0] & 0x80) > 0,
                SubFrameCountOrIndex = (data[0] >> 5 & 3),  // 0000 0011
                HasMessageTypeByte_b4 = (data[0] & 8) > 0,
                //TODO:  frame.frmeID_b765 = (byte)(data[0] & 7);
                FrameCountOrNuber = data[1]
            };

            Array.Copy(data, 2, frame.Payload, 0, PAYLOAD_LENGTH);
            return frame;
        }

        public override byte[] Serialize()
        {
            throw new NotImplementedException();
        }

        //byte[] serialize()
        //{
        //    byte[] var1 = super.serializeHdr();
        //    var1[1] = this.frameCntOrNo;
        //    Util.byteCpy(var1, 2, this.vPayLoad, 0, (short)18);
        //    return var1;
        //}

        public override string ToString()
        {
            return String.Format("FirstConsFrame: IsSubFrameCount={0}, SubFrameCountOrIndex={1}, HasMessageTypeByte_b4={2}, FrameCountOrNuber={3}",
                IsSubFrameCount, SubFrameCountOrIndex, HasMessageTypeByte_b4, FrameCountOrNuber);
        }
    }
}