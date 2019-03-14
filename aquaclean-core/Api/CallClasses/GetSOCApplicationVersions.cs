using Geberit.AquaClean.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Geberit.AquaClean.Core.Api
{
    [ApiCall(Context = 0x01, Procedure = 0x81, Node = 0x01)] //TODO: Node ??
    class GetSOCApplicationVersions : IApiCall
    {
        public byte[] GetPayload()
        {
            return new byte[0];
        }

        public SOCApplicationVersion Result(byte[] data)
        {
            return Deserializer.Deserialize<SOCApplicationVersion>(data);
        }
    }
}
