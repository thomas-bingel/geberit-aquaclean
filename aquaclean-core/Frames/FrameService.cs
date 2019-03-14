using Geberit.AquaClean.Core.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("tests")]
namespace Geberit.AquaClean.Core.Frames
{
    internal class FrameService
    {
        private readonly FrameFactory frameFactory;
        private readonly FrameValidation frameValidator;
        private TlMsgOutCtl tlMsgOutCtl = new TlMsgOutCtl();
        private const int RX_RECEIVING_CONS_FRAMES = 1;
        private const int RX_IDLE = 0;

        private readonly FrameCollector frameCollector;
        private int infoFrameCount = 0;

        public FrameService()
        {
            this.frameFactory = new FrameFactory();
            this.frameValidator = new FrameValidation();
            this.frameCollector = new FrameCollector();
            this.InfoFrameReceived += (sender, arg) =>
            {
                infoFrameCount++;
            };

            this.frameCollector.SendControlFrame += (sender, data) =>
            {
                Debug.WriteLine("Send control frame: " + data.ToByteString());
                var controlFrameData = frameFactory.BuildControlFrame(data).Serialize();
                SendData?.Invoke(sender, controlFrameData);

            };
            this.frameCollector.TransactionComplete += (sender, data) =>
            {
                TransactionComplete.Invoke(sender, data);
            };


        }

        public event EventHandler<InfoFrame> InfoFrameReceived;
        public event EventHandler<byte[]> TransactionComplete;
        // Callback to send data to the bluetooth device
        public event EventHandler<byte[]> SendData;

        internal void ProcessData(byte[] data)
        {
            if (data.Length != FrameFactory.BLE_PAYLOAD_LEN)
            {
                throw new Exception("Payload lenght is not " + FrameFactory.BLE_PAYLOAD_LEN);
            }

            Frame frame = frameFactory.CreateFrameFromBytes(data);
            if (frame == null)
            {
                Debug.WriteLine("Frame type not recognized");
                throw new Exception("Frame type was not recognized");
            }

            Debug.WriteLine(string.Format("Processing new Frame: {0}", frame.ToString()));

            switch (frame.FrameType)
            {
                case FrameType.SINGLE:
                    var singleFrame = frame as SingleFrame;

                    if (singleFrame.IsSubFrameCount)
                    {
                        frameCollector.StartTransaction(singleFrame.SubFrameCountOrIndex + 1);
                        frameCollector.AddFrame(0, singleFrame.Payload);
                    } else
                    {
                        frameCollector.AddFrame(singleFrame.SubFrameCountOrIndex, singleFrame.Payload);
                    }

                    break;
                case FrameType.FIRST: // Receiving the first frame of X cons frames

                    var firstFrame = frame as FirstConsFrame;
                    frameCollector.StartTransaction(firstFrame.FrameCountOrNuber);
                    frameCollector.AddFrame(0, firstFrame.Payload);

                    break;
                case FrameType.CONS:

                    var consFrame  = frame as FirstConsFrame;
                    frameCollector.AddFrame(consFrame.FrameCountOrNuber, consFrame.Payload);

                    break;
                case FrameType.CONTROL:
                    if (this.HandleControlFrame(tlMsgOutCtl, (FlowControlFrame)frame) > 0)
                    {
                        // Message complete
                        //this.tlSession_.tl_incrementTxValidMsgCnt();
                        //if (this.tlCallbacks_ != null)
                        //{
                        //    this.tlCallbacks_.pMsgSendCompleteCb((byte)tlMsgOutCtl.nTxErr);
                        //    return 0;
                        //}
                    }
                    break;
                case FrameType.INFO:
                    var infoFrame = (InfoFrame)frame;
                    if (infoFrame.InfoFrmType == 1)
                    {
                        //this.tlSession_.info1Data = infoFrm.info1Data;
                        Debug.WriteLine(String.Format("rcv info frame, protocol={0}, rs={1}{2}, ts={3}",
                            infoFrame.ProtoVersion,
                            infoFrame.RsHi,
                            infoFrame.RsLo,
                            infoFrame.TsHi * 256 + infoFrame.TsLo));
                        InfoFrameReceived?.Invoke(this, infoFrame);
                    }
                    break;
                default:
                    break;
            }



        }

        internal async Task WaitForInfoFramesAsync()
        {
            // 10 Info frames are send after connection. 
            // Wait for these frames, bevore sending new commands.
            while(infoFrameCount < 10)
            {
                await Task.Delay(100);
            }
            infoFrameCount = 0;
            return;
        }

        internal async Task SendFrameAsync(Frame frame)
        {
            SendData?.Invoke(this, frame.Serialize());
        }

        private short HandleControlFrame(TlMsgOutCtl tlMsgOutCtl, FlowControlFrame frame)
        {
            if (frame.ErrorCode != 0 || tlMsgOutCtl.nTxState != 0)
            {
                // Error 
                return 0;
            }

            Array.Copy(frame.AckdFrameBitmask, 0, tlMsgOutCtl.vTxAckdFrameBitmask, 0, 8);
            frameValidator.MarkTransactionOkPackets(tlMsgOutCtl);

            short highestOkFrameCount = GetHighestOkFrameCnt(tlMsgOutCtl.nTxFrameCnt, tlMsgOutCtl.vTxBackLogCtr);
            Debug.WriteLine(String.Format("Highest OK Frame Count {0}", highestOkFrameCount));
            if (highestOkFrameCount == tlMsgOutCtl.nTxFrameCnt)
            {
                tlMsgOutCtl.nTxFrameCnt = 0;
                tlMsgOutCtl.nTxState =0;
                Debug.WriteLine(">>--------TX MSG SUCCESS----------<<");
                return 1;
            }
            else
            {
                tlMsgOutCtl.nTxLatencyMs = (short)Math.Max(frame.TransactionLatency, 10);
                tlMsgOutCtl.nTxUnackdFrameLimit = frame.UnackdFrameLimit;
                tlMsgOutCtl.nTxState = 2;
                Debug.WriteLine(String.Format("Waiting for {0} of {1}", highestOkFrameCount, tlMsgOutCtl.nTxFrameCnt));
                return 0;
            }
        }


        private short GetHighestOkFrameCnt(int txnFrameCnt, byte[] txnBackLogCtr)
        {
            short counter = 0;
               
            for (short i = 0; i < txnFrameCnt && txnBackLogCtr[i] == 255; ++i) 
            {
                ++counter;
            }

            return counter;
        }

    
      


    }
}
