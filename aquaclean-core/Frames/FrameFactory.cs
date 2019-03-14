using Geberit.AquaClean.Core.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("tests")]
namespace Geberit.AquaClean.Core.Frames
{
    class FrameFactory
    {

        public const int BLE_PAYLOAD_LEN = 20;

        /// <summary>
        ///     18 = 0 = 0x12
        ///     19 = 0 = 0x13
        /// Ab  32 = 1 = 0x20
        /// Ab  64 = 2 = 0x40
        /// Ab  96 = 3 = 0x60
        ///    112 = 3 = 0x70
        /// Ab 128 = 4 = 0x80
        /// </summary>
        /// <param name="headerByte"></param>
        /// <returns></returns>
        internal FrameType getFrameTypeFromHeaderByte(byte headerByte)
        {
            return (FrameType)(headerByte >> 5 & 7); 
        }

        public Frame CreateFrameFromBytes(byte[] data)
        {


            FrameType frameType = getFrameTypeFromHeaderByte(data[0]);
            Frame tlFrame = null;
            switch (frameType)
            {
                case FrameType.SINGLE: // 0
                    tlFrame = SingleFrame.CreateSingleFrame(data);
                    break;
                case FrameType.FIRST: // 1
                case FrameType.CONS: // 2
                    tlFrame = FirstConsFrame.CreateFirstConsFrame(data);
                    break;
                case FrameType.CONTROL: // 3
                    tlFrame = FlowControlFrame.CreateFlowControlFrame(data);
                    break;
                case FrameType.INFO: // 4
                    tlFrame = InfoFrame.CreateInfoFrame(data);
                    break;
            }

            if (tlFrame != null)
            {
                tlFrame.FrameType = frameType;
                tlFrame.HasMessageTypeByte_b4 = (data[0] & 16) > 0;
                tlFrame.SubFrameCountOrIndex = data[0] >> 1 & 3;
                tlFrame.IsSubFrameCount = (data[0] & 1) > 0;
            }

            return tlFrame;

            //string yourByteString = Convert.ToString(headerByte, 2).PadLeft(8, '0');
        }

        public FlowControlFrame BuildControlFrame(byte[] bitmap)
        {
            FlowControlFrame flowControlFrame = new FlowControlFrame();
            flowControlFrame.HasMessageTypeByte_b4 = true;
            flowControlFrame.FrameType = FrameType.CONTROL;
            flowControlFrame.UnackdFrameLimit = 8;
            flowControlFrame.TransactionLatency = 0;
            Array.Copy(bitmap, 0, flowControlFrame.AckdFrameBitmask, 0, 8);
            byte[] var3 = flowControlFrame.AckdFrameBitmask;
            Debug.WriteLine("Bitmask: " + var3.ToByteString());
            return flowControlFrame;
        }

        public SingleFrame BuildSingleFrame(byte[] data)
        {
            SingleFrame singleFrm = new SingleFrame();
            singleFrm.FrameType = FrameType.SINGLE;
            singleFrm.HasMessageTypeByte_b4 = true;

            singleFrm.IsSubFrameCount = true;
            singleFrm.SubFrameCountOrIndex = 0;

            Array.Copy(data, 0, singleFrm.Payload, 0, 19);
            return singleFrm;
        }



    }
}
