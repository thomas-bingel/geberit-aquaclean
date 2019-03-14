using Geberit.AquaClean.Core.Api;
using Geberit.AquaClean.Core.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Geberit.AquaClean.Core.Common
{
    internal class Deserializer
    {
        public static int DeserializeToInt(byte[] data, int position, int length)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(data, position, length);
            }

            uint var0 = 0;
            for (int i = position; i < position + length; ++i)
            {
                var0 = (ushort)(var0 << 8);
                var0 = (ushort)(var0 | data[i]);
            }

            return (int)var0;
        }

        internal static T Deserialize<T>(byte[] data) where T : struct
        {
            // Deserialize structs (more complex types) and its properties
            Object result = Activator.CreateInstance(typeof(T));

            int startPos = 0;
            foreach (var prop in typeof(T).GetProperties())
            {
                var info = prop.GetCustomAttribute<DeSerializeAttribute>(false);
                var propertyType = prop.PropertyType;
                var byteLength = info.Length;
                object value = null;

                if (propertyType == typeof(int) && byteLength == 1) // 1 Byte
                {
                    value = (int)data[startPos];
                    Debug.WriteLine(String.Format("Arg: {0}", value));
                }
                else if (propertyType == typeof(int) && byteLength == 2) // 2 Bytes => Short
                {
                    value = Deserializer.DeserializeToInt(data, startPos, byteLength);
                    Debug.WriteLine(String.Format("Arg: {0}", value));
                }
                else if (propertyType == typeof(int) && byteLength == 4) // 4 Bytes => Integer
                {
                    value = Deserializer.DeserializeToInt(data, startPos, byteLength);
                    Debug.WriteLine(String.Format("Arg: {0}", value));
                }
                else if (propertyType == typeof(string))
                {
                    value = Encoding.UTF8.GetString(data, startPos, byteLength).Replace("\0", String.Empty).Trim();
                    Debug.WriteLine(String.Format("Arg: {0}", value));
                }
                else if (propertyType == typeof(byte[]))
                {
                    value = new byte[byteLength];
                    Array.Copy(data, startPos, (byte[])value, 0, byteLength);
                    Debug.WriteLine(String.Format("Arg: {0}", ((byte[])value).ToByteString()));
                }
                else if (propertyType == typeof(int[]))
                {
                    var arraySize = byteLength / 5;
                    value = new int[arraySize];
                    for (int i = 0; i < arraySize; i++)
                    {
                        var number = Deserializer.DeserializeToInt(data, i * 5 + 1 + startPos, 4);
                        ((int[])value)[i] = number;
                    }
                    Debug.WriteLine(String.Format("Arg: {0}", value));
                }
                prop.SetValue(result, value, null);
                startPos += byteLength;

            }


            return (T)result;

        }
    }
}
