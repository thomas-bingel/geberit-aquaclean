using Geberit.AquaClean.Core.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geberit.AquaClean.Core.Api
{
    [ApiCall(Context = 0x01, Procedure = 0x54, Node = 0x01)]
    class SetStoredProfileSetting : IApiCall
    {

        public ProfileSettings ProfileSetting { get; private set; }
        public int Value { get; private set; }

        public SetStoredProfileSetting(ProfileSettings profileSetting, int value) {
            this.ProfileSetting = profileSetting;
            this.Value = value;
        }


        public byte[] GetPayload()
        {
            var profileId = 0;
            var data = new byte[4];
            data[0] = (byte)profileId;
            data[1] = (byte)this.ProfileSetting;
            data[2] = (byte)(this.Value >> 0 & 255);
            data[3] = (byte)(this.Value >> 8 & 255);

            return data;
        }
    }
}
