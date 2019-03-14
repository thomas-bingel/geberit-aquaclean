using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Geberit.AquaClean.Core.Clients
{
    public class AquaCleanClient : IAquaCleanClient
    {
        public event EventHandler<DeviceStateChangedEventArgs> DeviceStateChanged;
        public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

        private readonly IAquaCleanBaseClient baseClient;
        private readonly Timer stateChangedTimer;
        private readonly double STATE_CHANGED_TIMER_INTERVAL = 2500;
        private DeviceStateChangedEventArgs lastDeviceStateChangedEventArgs;

        public String SapNumber { get; private set; }
        public String SerialNumber { get; private set; }
        public String ProductionDate { get; private set; }
        public String Description { get; private set; }
        public string InitialOperationDate { get; private set; }

        internal AquaCleanClient(IBluetoothLeConnector bluetoothConnector)
        {
            this.baseClient = new AquaCleanBaseClient(bluetoothConnector);
            this.baseClient.ConnectionStatusChanged += (sender, args) =>
            {
                this.ConnectionStatusChanged?.Invoke(this, args);
            };
            this.stateChangedTimer = new Timer(STATE_CHANGED_TIMER_INTERVAL);
           
            this.stateChangedTimer.Elapsed += StateChangedTimer_Elapsed;
        }

        public async Task Connect(string deviceId)
        {
            await this.baseClient.ConnectAsync(deviceId);

            // Read available data from device
            var deviceIdentification = await baseClient.GetDeviceIdentificationAsync(0);
            this.SerialNumber = deviceIdentification.SerialNumber;
            this.SapNumber = deviceIdentification.SapNumber;
            this.ProductionDate = deviceIdentification.ProductionDate;
            this.Description = deviceIdentification.Description;

            // Get Firmware version
            //var firmwareVersion = await baseClient.GetFirmwareVersionList();
            //this.FirmwareVersion = firmwareVersion.

            var initialOperationDate = await baseClient.GetDeviceInitialOperationDate();
            this.InitialOperationDate = initialOperationDate;

            this.stateChangedTimer.Enabled = true;

        }

        private async void StateChangedTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var result = await baseClient.GetSystemParameterListAsync(new byte[] { 0, 1, 2, 3, 4, 5, 6, 9 });

            var deviceStateChangedEventArgs = new DeviceStateChangedEventArgs()
            {
                IsUserSitting = result.DataArray[0] != 0,
                IsAnalShowerRunning = result.DataArray[1] != 0,
                IsLadyShowerRunning = result.DataArray[2] != 0,
                IsDryerRunning = result.DataArray[3] != 0,
                //DescalingState = Deserializer.DeserializeToInt(result.DataArray, 4 * 5 + 1, 4),
                //DescalingDurationInMinutes = Deserializer.DeserializeToInt(result.DataArray, 5 * 5 + 1, 4),
                //LastErrorCode = Deserializer.DeserializeToInt(result.DataArray, 6 * 5 + 1, 4),
                //OrientationLightState = Deserializer.DeserializeToInt(result.DataArray, 7 * 5 + 1, 4),
            };

            if (lastDeviceStateChangedEventArgs == null)
            {
                // Invoke event for initial state of the device
                DeviceStateChanged?.Invoke(this, deviceStateChangedEventArgs);
            }
            else
            {
                // Only invoke event if something changed
                if (!deviceStateChangedEventArgs.Equals(lastDeviceStateChangedEventArgs))
                {
                    var dsc = deviceStateChangedEventArgs;
                    var ldsc = lastDeviceStateChangedEventArgs;
                    DeviceStateChanged?.Invoke(this, new DeviceStateChangedEventArgs()
                    {
                        IsUserSitting = dsc.IsUserSitting != ldsc.IsUserSitting ?
                                             dsc.IsUserSitting : null,
                        IsAnalShowerRunning = dsc.IsAnalShowerRunning != ldsc.IsAnalShowerRunning ?
                                             dsc.IsAnalShowerRunning : null,
                        IsLadyShowerRunning = dsc.IsLadyShowerRunning != ldsc.IsLadyShowerRunning ?
                                             dsc.IsLadyShowerRunning : null,
                        IsDryerRunning = dsc.IsDryerRunning != ldsc.IsDryerRunning ?
                                             dsc.IsDryerRunning : null,
                    });
                }

            }
            lastDeviceStateChangedEventArgs = deviceStateChangedEventArgs;


        }

        public async Task ToggleAnalShower()
        {
            await this.baseClient.SetCommandAsync(Commands.ToggleAnalShower);
        }

        public async Task ToggleLadyShower()
        {
            await this.baseClient.SetCommandAsync(Commands.ToggleLadyShower);
        }

        public async Task ToggleLidPosition()
        {
            await this.baseClient.SetCommandAsync(Commands.ToggleLidPosition);
        }


        public async Task<int> GetAnalShowerPosition()
        {
            return await this.baseClient.GetStoredProfileSettingAsync(ProfileSettings.AnalShowerPosition);
        }

        public async Task<int> GetAnalShowerPressure()
        {
            return await this.baseClient.GetStoredProfileSettingAsync(ProfileSettings.AnalShowerPressure);
        }

        public async Task<int> GetWaterTemperature()
        {
            return await this.baseClient.GetStoredProfileSettingAsync(ProfileSettings.WaterTemperature);
        }

        public async Task<int> GetLadyShowerPosition()
        {
            return await this.baseClient.GetStoredProfileSettingAsync(ProfileSettings.LadyShowerPosition);
        }

        public async Task<int> GetLadyShowerPressure()
        {
            return await this.baseClient.GetStoredProfileSettingAsync(ProfileSettings.LadyShowerPressure);
        }

        public async Task<bool> GetDryerState()
        {
            return await this.baseClient.GetStoredProfileSettingAsync(ProfileSettings.DryerState) == 1;
        }

        public async Task<bool> GetOscilationState()
        {
            return await this.baseClient.GetStoredProfileSettingAsync(ProfileSettings.OscillatorState) == 1;
        }

        public async Task<bool> GetOdourExtractionStateAsync()
        {
            return await this.baseClient.GetStoredProfileSettingAsync(ProfileSettings.OdourExtraction) == 1;
        }

        public async Task SetOdourExtractionStateAsync(bool state)
        {
            await this.baseClient.SetStoredProfileSettingAsync(ProfileSettings.OdourExtraction, state ? 1 : 0);
        }

        public async Task<int> GetDryerTemperature()
        {
            return await this.baseClient.GetStoredProfileSettingAsync(ProfileSettings.DryerState);
        }

        public async Task<int> GetWcSeatHeat()
        {
            return await this.baseClient.GetStoredProfileSettingAsync(ProfileSettings.WcSeatHeat);
        }

        public async Task<bool> GetSystemFlushState()
        {
            return await this.baseClient.GetStoredProfileSettingAsync(ProfileSettings.SystemFlush) == 1;
        }


    }
}
