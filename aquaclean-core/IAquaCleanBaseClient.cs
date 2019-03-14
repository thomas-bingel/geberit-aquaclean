using Geberit.AquaClean.Core.Api;
using Geberit.AquaClean.Core.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geberit.AquaClean.Core
{
    public interface IAquaCleanBaseClient
    {
        event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

        Task ConnectAsync(String deviceId);
        Task SetCommandAsync(Commands command);
        Task<SystemParameterList> GetSystemParameterListAsync(byte[] parameterList);
        Task<DeviceIdentification> GetDeviceIdentificationAsync(int node);
        //Task<FirmwareVersionList> GetFirmwareVersionList(object arg1, object arg2);
        Task SetStoredProfileSettingAsync(ProfileSettings profileSetting, int settingValue);
        Task<int> GetStoredProfileSettingAsync(ProfileSettings profileSetting);
        Task<string> GetDeviceInitialOperationDate();
    }

    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        public ConnectionStatusChangedEventArgs(bool isConnected)
        {
            this.IsConnected = isConnected;
        }

        public bool IsConnected { get; private set; }
    }
}
