using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Geberit.AquaClean.Core
{
    public interface IAquaCleanClient
    {

        event EventHandler<DeviceStateChangedEventArgs> DeviceStateChanged;
        event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

        Task Connect(string deviceId);
        Task ToggleLidPosition();
        Task ToggleLadyShower();
        Task ToggleAnalShower();

        Task<int> GetAnalShowerPosition();
        Task<int> GetAnalShowerPressure();
        Task<int> GetWaterTemperature();
        Task<int> GetLadyShowerPosition();
        Task<int> GetLadyShowerPressure();
        Task<bool> GetDryerState();
        Task<bool> GetOscilationState();
        Task<bool> GetOdourExtractionStateAsync();
        Task SetOdourExtractionStateAsync(bool state);
        Task<int> GetDryerTemperature();
        Task<int> GetWcSeatHeat();
        Task<bool> GetSystemFlushState();


    }


    public class DeviceStateChangedEventArgs: EventArgs
    {
        public bool? IsUserSitting { get; set; }
        public bool? IsAnalShowerRunning { get; set; }
        public bool? IsLadyShowerRunning { get; set; }
        public bool? IsDryerRunning { get; set; }
        //public int DescalingState { get; set; }
        //public int DescalingDurationInMinutes { get; set; }
        //public int LastErrorCode { get; set; }
        //public int OrientationLightState { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is DeviceStateChangedEventArgs eq))
                return false;

            if (IsUserSitting == null && eq.IsUserSitting != null)
                return false;
            if (!IsUserSitting.Equals(eq.IsUserSitting))
                return false;

            if (IsUserSitting == null && eq.IsAnalShowerRunning != null)
                return false;
            if (!IsUserSitting.Equals(eq.IsAnalShowerRunning))
                return false;

            if (IsUserSitting == null && eq.IsLadyShowerRunning != null)
                return false;
            if (!IsUserSitting.Equals(eq.IsLadyShowerRunning))
                return false;

            if (IsUserSitting == null && eq.IsDryerRunning != null)
                return false;
            if (!IsUserSitting.Equals(eq.IsDryerRunning))
                return false;

            return true;
        }

        //public override string ToString()
        //{
        //    return string.Format("DeviceState: IsUserSitting={0}, IsAnalShowerRunning={1}, IsLadyShowerRunning={2}, IsDryerRunning={3}, DescalingState={4}, DescalingDurationInMinutes={5}, LastErrorCode={6}, OrientationLightState={7}",
        //        IsUserSitting, IsAnalShowerRunning, IsLadyShowerRunning, IsDryerRunning, DescalingState, DescalingDurationInMinutes, LastErrorCode, OrientationLightState);
        //}
    }
}
