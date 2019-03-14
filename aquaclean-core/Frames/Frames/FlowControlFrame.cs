using Geberit.AquaClean.Core.Common;
using System;

namespace Geberit.AquaClean.Core.Frames
{
    internal class FlowControlFrame : Frame
    {
        private const int BITMASK_LENGTH = 8;

        public int ErrorCode { get; private set; }
        /// <summary>
        /// In milliseconds
        /// </summary>
        public int TransactionLatency { get; set; }
        public int UnackdFrameLimit { get; set; }
        public byte[] AckdFrameBitmask { get; set; } = new byte[BITMASK_LENGTH];

        internal static FlowControlFrame CreateFlowControlFrame(byte[] data)
        {
            FlowControlFrame frame = new FlowControlFrame
            {
                ErrorCode = data[1],
                UnackdFrameLimit = data[2],
                TransactionLatency = data[3]
            };
            Array.Copy(data, 4, frame.AckdFrameBitmask, 0, BITMASK_LENGTH);
            return frame;
        }
        
        public override byte[] Serialize()
        {
            byte[] var1 = base.SerializeHdr();
            var1[1] = (byte)ErrorCode;
            var1[2] = (byte)UnackdFrameLimit;
            var1[3] = (byte)TransactionLatency;
            Array.Copy(AckdFrameBitmask, 0, var1, 4, 8);
            return var1;
        }

        public override string ToString()
        {
            return String.Format("FlowControlFrame: ErrorCode=0x{0:X2}, UncheckedFrameLimit=0x{1:X2}, TransactionLatency=0x{2:X2}, AckdFrameBitmask={3}",
                this.ErrorCode, this.UnackdFrameLimit, this.TransactionLatency, AckdFrameBitmask.ToByteString());
        }

    }
}