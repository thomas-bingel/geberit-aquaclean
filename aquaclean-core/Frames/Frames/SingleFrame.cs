using System;

namespace Geberit.AquaClean.Core.Frames
{
    internal class SingleFrame : Frame
    {
        private const int PAYLOAD_LENGTH = 19;
        public byte[] Payload { get; private set; } = new byte[PAYLOAD_LENGTH];

        public static SingleFrame CreateSingleFrame(byte[] data)
        {
            SingleFrame frame = new SingleFrame();
            Array.Copy(data, 1, frame.Payload, 0, PAYLOAD_LENGTH);
            return frame;
        }

        public override byte[] Serialize()
        {
            byte[] var1 = base.SerializeHdr();
            Array.Copy(this.Payload, 0, var1, 1, 19);
            return var1;
        }
    }
}