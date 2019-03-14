using Geberit.AquaClean.Core.Common;

namespace Geberit.AquaClean.Core.Api
{
    [ApiCall(Context = 0x00, Procedure = 0x82, Node = 0x01)]
    class GetDeviceIdentification : IApiCall
    {
        public byte[] GetPayload()
        {
            return new byte[0];
        }

        public DeviceIdentification Result(byte[] data)
        {
            return Deserializer.Deserialize<DeviceIdentification>(data);
        }
    }
}
