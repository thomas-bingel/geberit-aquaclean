namespace Geberit.AquaClean.Core.Frames
{
    internal class TlMsgOutCtl
    {
        internal int nTxState;
        internal byte[] vTxAckdFrameBitmask = new byte[255];
        internal int nTxFrameCnt;
        internal short nTxLatencyMs;
        internal int nTxUnackdFrameLimit;
        internal byte[] vTxBackLogCtr = new byte[255];
        internal bool bTxHasMsgTypeByte;
        internal short nDataLen;
    }
}