using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Geberit.AquaClean.Core.Api;
using Geberit.AquaClean.Core.Frames;
using Geberit.AquaClean.Core.Message;

namespace Geberit.AquaClean.Core.Clients
{
    public class AquaCleanBaseClient : IAquaCleanBaseClient
    {
        public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

        private readonly IBluetoothLeConnector bluetoothLeConnector;
        private readonly FrameService frameService;
        private readonly FrameFactory frameFactory;

        private readonly MessageService messageService;

        private readonly Dictionary<ApiCallAttribute, Type> contextLookup = new Dictionary<ApiCallAttribute, Type>();
        private readonly EventWaitHandle eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private MessageContext messageContext;
        private static long callCount = 0;

        internal AquaCleanBaseClient(IBluetoothLeConnector bluetoothLeConnector)
        {
            this.bluetoothLeConnector = bluetoothLeConnector;
            frameService = new FrameService();
            frameFactory = new FrameFactory();
            messageService = new MessageService();

            // Build lookup
            var type = typeof(IApiCall);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            contextLookup = types
                    .Select(t => new
                    {
                        Type = t,
                        Api = (ApiCallAttribute)t.GetCustomAttributes(typeof(ApiCallAttribute), false).FirstOrDefault()
                    }
                       )
                       .Where(t => t.Api != null)
                       .ToDictionary(t => t.Api, t => t.Type);

            // Process received data from Bluetooth
            bluetoothLeConnector.DataReceived += (sender, data) =>
            {
                frameService.ProcessData(data);
            };

            // Connection status has changed
            bluetoothLeConnector.ConnectionStatusChanged += (sender, args) => {
                this.ConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs(args));
            };

            // Send Frame over Bluetooth
            frameService.SendData += async (sender, data) =>
            {
                await bluetoothLeConnector.SendMessageAsync(data);
            };

            // Process complete transaction
            frameService.TransactionComplete += (sender, data) =>
            {
                this.messageContext = messageService.ParseMessage(data);
                var context = new ApiCallAttribute()
                {
                    Context = messageContext.Context,
                    Procedure = messageContext.Procedure
                };
                if (contextLookup.ContainsKey(context))
                {
                    var cc = contextLookup[context];
                    Debug.WriteLine("Result for " + cc.Name);
                }

                eventWaitHandle.Set();
            };

        }

        public async Task ConnectAsync(string deviceId)
        {
            await bluetoothLeConnector.ConnectAsync(deviceId);
            await frameService.WaitForInfoFramesAsync();
        }

        public void Disconnect()
        {
            bluetoothLeConnector.Disconnect();
        }

        public async Task<SystemParameterList> GetSystemParameterListAsync(byte[] parameterList)
        {
            return (await SendRequest(new GetSystemParameterList(parameterList))).Result(messageContext.ResultBytes);
        }

        public async Task<DeviceIdentification> GetDeviceIdentificationAsync(int node)
        {
            return (await SendRequest(new GetDeviceIdentification())).Result(messageContext.ResultBytes);
        }

        public async Task<string> GetDeviceInitialOperationDate()
        {
            return (await SendRequest(new GetDeviceInitialOperationDate())).Result(messageContext.ResultBytes);
        }

        public async Task SetCommandAsync(Commands command)
        {
            await SendRequest(new SetCommand(command));
        }

        public async Task<int> GetStoredProfileSettingAsync(ProfileSettings profileSetting)
        {
            return (await SendRequest(new GetStoredProfileSetting(0, profileSetting))).Result(messageContext.ResultBytes);
        }

        public async Task SetStoredProfileSettingAsync(ProfileSettings profileSetting, int settingValue)
        {
            await SendRequest(new SetStoredProfileSetting(profileSetting, settingValue));
        }

        public async Task<SOCApplicationVersion> GetSOCApplicationVersionsAsync()
        {
            return (await SendRequest(new GetSOCApplicationVersions())).Result(messageContext.ResultBytes);
        }


        private async Task<T> SendRequest<T>(T apiCall) where T : IApiCall
        {
            Debug.WriteLine(String.Format("Sending {0}", apiCall.GetType().Name));

            while (Interlocked.Read(ref callCount) > 0)
            {
                Thread.Sleep(100);
            }

            Interlocked.Increment(ref callCount);

            var data = BuildPayload(apiCall);
            var message = messageService.BuildMessage(data);
            var frame = frameFactory.BuildSingleFrame(message.Serialize());
            await frameService.SendFrameAsync(frame);
            eventWaitHandle.WaitOne();

            Interlocked.Decrement(ref callCount);

            return apiCall;
        }


        private byte[] BuildPayload(IApiCall apiCall)
        {
            var apiCallAttribute = (ApiCallAttribute)apiCall.GetType()
                .GetCustomAttributes(typeof(ApiCallAttribute), true).FirstOrDefault();

            if (apiCallAttribute == null)
            {
                throw new Exception("No ApiCallAttribute set on object");
            }

            var payload = apiCall.GetPayload();

            byte[] data = new byte[payload.Length + 4];
            data[0] = apiCallAttribute.Node;
            data[1] = apiCallAttribute.Context;
            data[2] = apiCallAttribute.Procedure;
            data[3] = (byte)(payload.Length);
            Array.Copy(payload, 0, data, 4, payload.Length);
            return data;
        }

    }
}
