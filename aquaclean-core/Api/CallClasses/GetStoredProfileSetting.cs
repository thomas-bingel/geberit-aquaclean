using Geberit.AquaClean.Core.Clients;
using Geberit.AquaClean.Core.Common;

namespace Geberit.AquaClean.Core.Api
{
    [ApiCall(Context = 0x01, Procedure = 0x53, Node = 0x01)]
    class GetStoredProfileSetting : IApiCall
    {
        public byte ProfileId { get; set; }
        public ProfileSettings ProfileSetting { get; set; }

        public GetStoredProfileSetting(byte profileId, ProfileSettings profileSetting)
        {
            this.ProfileId = profileId;
            this.ProfileSetting = profileSetting;
        }
        public byte[] GetPayload()
        {
            return new byte[] { this.ProfileId, (byte)this.ProfileSetting };
        }

        public int Result(byte[] data)
        {
            return Deserializer.DeserializeToInt(data, 0, 2); ;
        }
    }
}
