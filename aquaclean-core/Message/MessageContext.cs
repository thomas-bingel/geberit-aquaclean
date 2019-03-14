namespace Geberit.AquaClean.Core.Message
{
    public class MessageContext
    {
        public object Result { get; set; }
        public byte Context { get; internal set; }
        public byte Procedure { get; internal set; }
        public byte[] ResultBytes { get; internal set; }
    }
}