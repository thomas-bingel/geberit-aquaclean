using System;

namespace Geberit.AquaClean.Core.Frames
{
    internal class InfoFrame : Frame
    {
        public static byte INFO_PROTOVERS_2_0 = 32;
        public static byte INFO_PROTVERS_CAPABILITIES = 1;
        public static  byte INFO_RXCAP_TXNOWAIT_AFTER_FF = 1;
        private byte infoFrmType;

        public byte InfoFrmType { get; private set; }
        public byte ProtoVersion { get; private set; }
        public byte MaxPacketLen { get; private set; }
        public byte MaxPacketCount { get; private set; }
        public byte CapaFlags0 { get; private set; }
        public byte CapaFlags1 { get; private set; }
        public byte ModeFlags0 { get; private set; }
        public byte ModeFlags1 { get; private set; }
        public byte CtrlFlags0 { get; private set; }
        public byte CtrlFlags1 { get; private set; }
        public byte Cmd { get; private set; }
        public byte RsHi { get; private set; }
        public byte RsLo { get; private set; }
        public byte TsHi { get; private set; }
        public byte TsLo { get; private set; }

        internal static InfoFrame CreateInfoFrame(byte[] data)
        {
            InfoFrame frame = new InfoFrame
            {
                InfoFrmType = data[1],
                ProtoVersion = data[2],
                MaxPacketLen = data[3],
                MaxPacketCount = data[4],
                CapaFlags0 = data[5],
                CapaFlags1 = data[6],
                ModeFlags0 = data[7],
                ModeFlags1 = data[8],
                CtrlFlags0 = data[9],
                CtrlFlags1 = data[10],
                Cmd = data[11],
                RsHi = data[12],
                RsLo = data[13],
                TsHi = data[14],
                TsLo = data[15]
            };
            return frame;
        }


        public override byte[] Serialize()
        {
            byte[] var1 = base.SerializeHdr();
            var1[1] = InfoFrmType;
            var1[2] = ProtoVersion;
            var1[3] = MaxPacketLen;
            var1[4] = MaxPacketCount;
            var1[5] = CapaFlags0;
            var1[6] = CapaFlags1;
            var1[7] = ModeFlags0;
            var1[8] = ModeFlags1;
            var1[9] = CtrlFlags0;
            var1[10] =CtrlFlags1;
            var1[11] =Cmd;
            var1[12] =RsHi;
            var1[13] =RsLo;
            var1[14] =TsHi;
            var1[15] =TsLo;
            return var1;
        }

        public override string ToString()
        {
            var text = "InfoFrmType = {1:X2}, " +
                   "ProtoVersion = {2:X2}, " +
                   "MaxPacketLen = {3:X2}, " +
                   "MaxPacketCount = {4:X2}, " +
                   "CapaFlags0 = {5:X2}, " +
                   "CapaFlags1 = {6:X2}, " +
                   "ModeFlags0 = {7:X2}, " +
                   "ModeFlags1 = {8:X2}, " +
                   "CtrlFlags0 = {9:X2}, " +
                   "CtrlFlags1 = {10:X2}," +
                   "Cmd = {11:X2}, " +
                   "RsHi = {12:X2}, " +
                   "RsLo = {13:X2}, " +
                   "TsHi = {14:X2}, " +
                   "TsLo = {15:X2}  ";
            return String.Format(text, null,
                 InfoFrmType,
                 ProtoVersion,
                 MaxPacketLen,
                 MaxPacketCount,
                 CapaFlags0,
                 CapaFlags1,
                 ModeFlags0,
                 ModeFlags1,
                 CtrlFlags0,
                 CtrlFlags1,
                 Cmd,
                 RsHi,
                 RsLo,
                 TsHi,
                 TsLo
                );




           





        }

    }
}