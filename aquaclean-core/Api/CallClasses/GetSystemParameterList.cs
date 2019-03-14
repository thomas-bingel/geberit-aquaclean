using Geberit.AquaClean.Core.Common;
using System;

namespace Geberit.AquaClean.Core.Api
{
    [ApiCall(Context = 0x01, Procedure = 0x0D, Node = 0x01)]
    class GetSystemParameterList : IApiCall
    {

        /*
0 userIsSitting, 
1 analShowerIsRunning,
2 ladyShowerIsRunning,
3 dryerIsRunning,
4 descalingState, 
5 descalingDurationInMinutes,
6 lastErrorCode,
9 orientationLightState)
*/

        public byte[] ParameterList { get; private set; }

        public GetSystemParameterList(byte[] parameterList)
        {
            this.ParameterList = parameterList;
        }

        public byte[] GetPayload()
        {
            int argCount = Math.Min(this.ParameterList.Length, 12);
            byte[] data = new byte[13];
            data[0] = (byte)argCount;

            for (int i = 0; i < argCount; ++i)
            {
                data[i + 1] = this.ParameterList[i];
            }
            return data;
        }

        public SystemParameterList Result(byte[] data)
        {
            return Deserializer.Deserialize<SystemParameterList>(data);
        }
    }
}
