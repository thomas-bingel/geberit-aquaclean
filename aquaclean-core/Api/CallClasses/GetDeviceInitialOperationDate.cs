using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geberit.AquaClean.Core.Api
{
    [ApiCall(Context = 0x00, Procedure = 0x86, Node = 0x01)]
    class GetDeviceInitialOperationDate : IApiCall
    {
        public byte[] GetPayload()
        {
            return new byte[0];
        }

        public string Result(byte[] data)
        {
            return Encoding.UTF8.GetString(data).Replace("\0", String.Empty).Trim();
        }
    }
}
