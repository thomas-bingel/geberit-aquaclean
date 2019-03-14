using Geberit.AquaClean.Core.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geberit.AquaClean.Core.Frames
{
    /// <summary>
    /// Responsible to collect all frames which belong together
    /// </summary>
    class FrameCollector
    {
        private readonly object syncObj = new object();
        private Dictionary<int, byte[]> frameData;
        private byte[] bitmap;
        private int expectedFrames;
        private bool transactionInProgress;
        private Dictionary<int, byte[]> tempFrameData = new Dictionary<int, byte[]>();

        public event EventHandler<byte[]> SendControlFrame;
        public event EventHandler<byte[]> TransactionComplete;

        public void StartTransaction(int expectedFrames)
        {
            lock(syncObj)
            {
                this.frameData = new Dictionary<int, byte[]>();
                this.expectedFrames = expectedFrames;
                bitmap = new byte[8];
                transactionInProgress = true;

                foreach (var dict in tempFrameData)
                {
                    AddFrame(dict.Key, dict.Value);
                }
                tempFrameData.Clear();

            }
        }

        private void SetBitmap(int frameNumber)
        {
            int var3 = frameNumber / 8;
            int mask = 1 << frameNumber % 8;
            bitmap[var3] = (byte) (bitmap[var3] | mask );
            Debug.WriteLine(String.Format(
                "Controlling bitmap changed with frameNumber {0} => {1} Bitmap: {2}", 
                frameNumber, Convert.ToString(bitmap[var3], 2).PadLeft(8, '0'), bitmap.ToByteString()));
        }

        internal void AddFrame(int frameIndex, byte[] payload)
        {
            lock (syncObj)
            {
                if (!transactionInProgress)
                {
                    tempFrameData.Add(frameIndex, payload);
                    return;
                }

                Debug.WriteLine(String.Format("Received frame {0} of {1}: Payload={2}",
                    frameIndex + 1, expectedFrames, payload.ToByteString()));


                frameData.Add(frameIndex, payload);
                SetBitmap(frameIndex);
                if (frameData.Count % 4 == 0 || expectedFrames == frameData.Count)
                {
                    var bitmapClone = (byte[])bitmap.Clone();
                    Debug.Write(String.Format("Raising SendControlFrame with data {0}", bitmapClone.ToByteString()));
                    SendControlFrame?.Invoke(this, bitmapClone);
                }

                if (frameData.Count != expectedFrames)
                {
                    return;
                }

                // Build messsage
                byte[] data = new byte[payload.Length * expectedFrames];
                for (int i=0; i< expectedFrames; i++)
                {
                    Array.Copy(frameData[i], 0, data, i * payload.Length, payload.Length);
                }

                Debug.WriteLine("receive complete");
                transactionInProgress = false;
                tempFrameData.Clear();
                TransactionComplete?.Invoke(this, data);
            }

        }
    }
}
