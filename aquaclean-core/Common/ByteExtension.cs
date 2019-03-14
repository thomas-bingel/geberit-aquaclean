using System;

namespace Geberit.AquaClean.Core.Common
{
    public static class ByteExtension
    {
        public static string ToByteString(this byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

    }
}
