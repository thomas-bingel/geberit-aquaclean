using System;
using System.Collections.Generic;
using System.Text;

namespace Geberit.AquaClean.Core.Frames
{
    class FrameValidation
    {

        bool tl_isValidAckBitmap(TlMsgOutCtl tlMsgOutCtl, byte[] bitmask)
        {
            short var5 = 0;

            for (int index = 0; index < bitmask.Length; index++)
            {
                byte var7 = 1;
                byte var8 = 0;
                for (short var6 = 0; var6 < 8; var7 = var8)
                {
                    var8 = var7;
                    if ((bitmask[index] & var7) == var7)
                    {
                        if (tlMsgOutCtl.vTxBackLogCtr[var5] == 0)
                        {
                            return false;
                        }

                        var8 = (byte)(var7 << 1);
                    }

                    ++var5;
                    if (var5 == tlMsgOutCtl.nTxFrameCnt)
                    {
                        return true;
                    }

                    ++var6;
                }

            }
            return true;

        }

        public void MarkTransactionOkPackets(TlMsgOutCtl tlMsgOutCtl)
        {
            short var4 = -1;
            byte var2 = 1;

            for (short i = 0; i < tlMsgOutCtl.nTxFrameCnt; ++i)
            {
                if (i % 8 == 0)
                {
                    var2 = 1;
                    ++var4;
                }
                else
                {
                    var2 = (byte)(var2 << 1);
                }

                if ((tlMsgOutCtl.vTxAckdFrameBitmask[var4] & var2) == var2)
                {
                    tlMsgOutCtl.vTxBackLogCtr[i] = 255;
                }
            }

        }



        // Not used
        private void tl_setFrameBit(byte[] bitmap, int destination)
        {
            int var3 = destination / 8;
            bitmap[var3] = (byte)(bitmap[var3] | 1 << destination % 8);
        }


        // Not used
        short tl_getHighestOkFrameNo(byte[] var1, short var2)
        {
            short var3 = 0;
            byte var6 = 1;

            short var5;
            for (var5 = 0; var5 < var2 && (var1[var5] & 255) == 255; ++var5)
            {
                var3 = (short)(var3 + 8);
            }

            short var4 = var3;
            if (var5 < var2)
            {
                var2 = 0;

                while (true)
                {
                    var4 = var3;
                    if (var2 >= 8)
                    {
                        break;
                    }

                    var4 = var3;
                    if ((var1[var5] & var6) != var6)
                    {
                        break;
                    }

                    ++var3;
                    var6 = (byte)(var6 << 1);
                    ++var2;
                }
            }

            return var4;
        }
    }
}
