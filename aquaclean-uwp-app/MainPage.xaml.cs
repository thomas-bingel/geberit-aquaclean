using Geberit.AquaClean.Core;
using Geberit.AquaClean.Core.Api;
using Geberit.AquaClean.Core.Clients;
using Geberit.AquaClean.Core.Common;
using Geberit.AquaClean.Core.Frames;
using Geberit.AquaClean.Core.LE;
using Geberit.AquaClean.Core.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace geberit_aquaclean_mera
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly MqttService mqttService;

        public object TTask { get; private set; }

        public MainPage()
        {
            mqttService = new MqttService();

            this.InitializeComponent();

            Task.Run( async () => {

                try
                {
                    await mqttService.StartAsync();
                } catch
                {

                }
                await Test3();
            });


        }

        public async Task Test3()
        {

            String deviceId = "BluetoothLE#BluetoothLE00:1a:7d:da:71:08-94:e3:6d:63:8d:8b";
            var device = await DeviceInformation.CreateFromIdAsync(deviceId);
            await PairDeviceIfNecessary(device);



            var factory = new AquaCleanClientFactory(new BluetoothLeConnector());
            var client = factory.CreateClient();

            client.DeviceStateChanged += async (sender, args) =>
            {
                if (args.IsUserSitting.HasValue)
                {
                    Debug.WriteLine(String.Format("IsUserSitting={0}", args.IsUserSitting.Value));
                    await mqttService.SenDataAsync("/geberit/isUserSitting", args.IsUserSitting.Value.ToString());
                }

                if (args.IsAnalShowerRunning.HasValue)
                {
                    Debug.WriteLine(String.Format("IsAnalShowerRunning={0}", args.IsAnalShowerRunning.Value));
                    await mqttService.SenDataAsync("/geberit/isAnalShowerRunning", args.IsAnalShowerRunning.Value.ToString());
                }

                if (args.IsLadyShowerRunning.HasValue)
                {
                    Debug.WriteLine(String.Format("IsLadyShowerRunning={0}", args.IsLadyShowerRunning.Value));
                    await mqttService.SenDataAsync("/geberit/isLadyShowerRunning", args.IsLadyShowerRunning.Value.ToString());
                }

            };

            client.ConnectionStatusChanged += (sender, args) =>
            {
                Debug.WriteLine(String.Format("IsConnected={0}", args.IsConnected));
            };

            await client.Connect(deviceId);
            mqttService.ToggleLidPosition += (sender, args) =>
            {
                client.ToggleLidPosition();
            };

            //await baseClient.ConnectAsync();

            //await client.ToggleLidPosition();
            //await baseClient.SetCommandAsync(Commands.ToggleLidPosition);

            //await client.ToggleAnalShower();
            //await baseClient.SetCommandAsync(Commands.ToggleAnalShower);

            //await client.ToggleLadyShower();
            //await baseClient.SetCommandAsync(Commands.ToggleLadyShower);


            //IAquaClean client = AquaCleanClient.Create();
            //await client.ConnectAsync();

            //var data = await client.GetDeviceIdentificationAsync(0);
            //var paramList = await client.GetDeviceStateAsync();
            //Debug.WriteLine(String.Format("{0}", paramList.ToString()));

            //            await client.SetCommandAsync(Commands.ToggleLidPositon);

            //var x = await client.GetSystemParameterListAsync();

            //data = await client.GetDeviceIdentificationAsync(0);

            //await client.GetSystemParameterListAsync(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x6, 0x9 });

            //var odourExtraction = await client.GetOdourExtractionStateAsync();
            //Debug.WriteLine(String.Format("############ DATA: {0}", odourExtraction));

            //await client.SetOdourExtractionStateAsync(true);

            //odourExtraction = await client.GetOdourExtractionStateAsync();
            //Debug.WriteLine(String.Format("############ DATA: {0}", odourExtraction));

            //while (true)
            //{
            //    var data2 = await baseClient.GetDeviceStateAsync();
            //    Debug.WriteLine(String.Format("############ DATA: {0}", data2));
            //    try
            //    {
            //        await mqttService.SenDataAsync("/geberit/isUserSitting", data2.IsUserSitting.ToString());
            //        await mqttService.SenDataAsync("/geberit/isAnalShowerRunning", data2.IsAnalShowerRunning.ToString());
            //        await mqttService.SenDataAsync("/geberit/isLadyShowerRunning", data2.IsLadyShowerRunning.ToString());
            //    } catch { }

            //    await Task.Delay(2000);
            //}
            //await client.SetOdourExtractionStateAsync(false);

            //odourExtraction = await client.GetOdourExtractionStateAsync();
            //Debug.WriteLine(String.Format("############ DATA: {0}", odourExtraction));

            //await client.SetCommandAsync(Commands.ToggleLidPositon);

            //var data = await client.GetDeviceIdentificationAsync(0);
            //var initialDate = await client.GetDeviceInitialOperationDate();
            //var analShowerPosition = await client.GetAnalShowerPosition();
            //var analShowerPressure = await client.GetAnalShowerPressure();
            //var wcSeatHeat = await client.GetWcSeatHeat();
            //var waterTemperature = await client.GetWaterTemperature();

            //client.Disconnect();
        }

        //public async void Test2()
        //{
        //    var frameSender = new FrameSender();

        //    var bluetoothConnector = new LeConnector();
        //    var frameService = new FrameService();
        //    var messageService = new MessageService();

        //    // Process received data from Bluetooth
        //    bluetoothConnector.DataReceived += (sender, data) =>
        //    {
        //        frameService.ProcessData(data);
        //    };

        //    // Send Frame over Bluetooth
        //    frameService.SendData += async (sender, data) =>
        //    {
        //        await bluetoothConnector.SendMessageAsync(data);
        //    };

        //    // Process complete transaction
        //    frameService.TransactionComplete += (sender, data) =>
        //    {
        //        messageService.ParseMessage(data);
        //    };

        //    await bluetoothConnector.ConnectAsync("BluetoothLE#BluetoothLE00:1a:7d:da:71:08-94:e3:6d:63:8d:8b");

        //    //Thread.Sleep(3000);
        //    //var singleFrame = messageService.BuildMessage(new byte[] { 1, 0, 130, 0 });
        //    //await bluetoothConnector.SendMessage(singleFrame);

        //    //Thread.Sleep(4000);
        //    //singleFrame = SendCommand(1, 10);
        //    //await bluetoothConnector.SendMessage(singleFrame);




            
             
         
            

        //    //Task.Run(() =>
        //    //   {
        //    //       byte[] controlFrame;
        //    //       byte[] singleFrame;

        //    //       Thread.Sleep(6000);
        //    //       // GetDeviceIdentification
        //    //       singleFrame = messageService.sendMessageAsync(new byte[] { 1, 0, 130, 0 });
        //    //       bluetoothConnector.SendMessage(singleFrame);

        //    //       Thread.Sleep(3000);

        //    //       controlFrame = frameSender.BuildControlFrame(new byte[] { 0x0f, 0, 0, 0, 0, 0, 0, 0, 0 });
        //    //       bluetoothConnector.SendMessage(controlFrame);

        //    //       Thread.Sleep(3000);

        //    //       controlFrame = frameSender.BuildControlFrame(new byte[] { 0x3f, 0, 0, 0, 0, 0, 0, 0, 0 });
        //    //       bluetoothConnector.SendMessage(controlFrame);

        //    //       Thread.Sleep(100);
        //    //       //GetDeviceInitialOperationDate
        //    //       singleFrame = messageService.sendMessageAsync(new byte[] { 1, 0, 134, 0 });
        //    //       bluetoothConnector.SendMessage(singleFrame);

        //    //   });



        //}

        //byte[] SendCommand(int var1, int var2)
        //{
        //    var messageService = new MessageService();

        //    byte[] var3 = new byte[] { (byte)var1, 1, 9, 0, 0 };
        //    var3[3] = (byte)(var3.Length - 4);
        //    var3[4] = (byte)(var2 >> 0 & 255);
        //    return messageService.BuildMessage(var3);
        //}

        //public void Test()
        //{
        //    FrameFactory frameFactory = new FrameFactory();
        //    var frameService = new FrameService();
        //    var messageService = new MessageService();
        //    var frameSender = new FrameSender();

        //    //byte[] controlFrame = frameSender.BuildControlFrame();
        //    //Debug.WriteLine("ControlFrame: " + controlFrame.ToByteString());

        //    //IAquaClean aquaClean = null;
        //    //aquaClean.SetCommand(123);

        //    //byte[] data2 = messageService.sendMessage(new byte[] { 1, 0, 130, 0 });
        //    //Debug.WriteLine(string.Format("Processing new Frame: {0}", ByteArrayToString(data2)));

        //    //var frameData = frameSender.BuildSingleFrame(new byte[] { });
        //    //messageService.ParseFrame(frameData);
        //    //byte[] data2 = messageService.sendMessageAsync(new byte[] { 1, 0, 134, 0 });
        //    //Debug.WriteLine("Data: " + data2.ToByteString());


        //    frameService.TransactionComplete += (sender, data) =>
        //    {
        //        messageService.ParseMessage(data);
        //    };


        //    foreach (byte[] data in GetData())
        //    {
        //        frameService.ProcessData(data);
        //    }


        //}


        private List<byte[]> GetData()
        {
            List<byte[]> data = new List<byte[]>
            {
                //StringToByteArray("800130140c030003000000003130001200ff0800"), // Info
                //StringToByteArray("800130140c030003000000003130001200ff0800"), // Info
                //StringToByteArray("800130140c030003000000003130001200ff0800"), // Info
                //StringToByteArray("800130140c030003000000003130001200ff0800"), // Info
                //StringToByteArray("800130140c030003000000003130001200ff0800"), // Info
                //StringToByteArray("800130140c030003000000003130001200ff0800"), // Info
                //StringToByteArray("800130140c030003000000003130001200ff0800"), // Info
                //StringToByteArray("800130140c030003000000003130001200ff0800"), // Info
                //StringToByteArray("800130140c030003000000003130001200ff0800"), // Info
                //                 1104ff0004e4e401008200000000000000000000    // Command
                //                 70000C30010000000000000000FF090100000D2D
                StringToByteArray("70000c0c010000000000000000ff090100000d2d"), // Control 
                StringToByteArray("300605000057febf00010082523134362e323178"), // First 1/6
                StringToByteArray("52012e78782e7848423138303645553135333437"), // Cons 2/6
                StringToByteArray("54023100000000000030342e30362e3230313841"), // Cons 3/6
                StringToByteArray("5603717561436c65616e204d65726120436f6d66"), // Cons 4/6
                //                 700008000F000000000000000000000000000000    // Control 
                StringToByteArray("40046f7274000000000000000000000000000000"), // Cons 5/6
                StringToByteArray("4205000000000000000000000000000000000000"), // Cons 6/6
                //                 700008003f000000000000000000000000000000    // Control
                //                 1104ff0004282001008600000000000000000000    // Command              
                StringToByteArray("70000c0c010000000000000000ff090100000d2d"), // Control
                StringToByteArray("130500000fb968000100860a32362e30372e3230"), // Single 1/2
                StringToByteArray("1231382e00000000000000000000000000000000"), // Single 2/2
                //                 7000080003000000000000000000000000000000    // Control 
                //                 1104ff00045c1701014500000000000000000000    // Command
                StringToByteArray("70000c0c010000000000000000ff090100000d2d"), // Control 
                StringToByteArray("1305000015ff9d0001014510031d016d01320000"), // Single 1/2
                StringToByteArray("1200000000000000003000000000000000000000"), // Single 2/2
                //                 1104ff000451db01010500000000000000000000    // Control
                StringToByteArray("70000c0c010000000000000000ff090100000d2d"), // Control
                StringToByteArray("3008050000864c8f00010105810c030405060708"), // First 1/8
                StringToByteArray("5201090a0b0c0e0f000000000000000000000000"), // Cons 2/8
                StringToByteArray("5402000000000000000000000000000000000000"), // Cons 3/8
                StringToByteArray("5603000000000000000000000000000000000000"), // Cons 4/8
                                // 700008000f000000000000000000000000000000
                StringToByteArray("4004000000000000000000000000000000000000"), // Cons 5/8
                StringToByteArray("4205000000000000000000000000000000000000"), // Cons 6/8
                StringToByteArray("4406000000000000000000000000000000000000"), // Cons 7/8
                StringToByteArray("4607000000000000000000000000000000000000"), // Cons 8/8    527
                //                 70000800ff000000000000000000000000000000
                //                 1304ff0011dffe01010e0d0c030405060708090a
                //                 120b0c0e0f000000000000000000000000000000
                StringToByteArray("70000c0c010000000000000000ff090100000d2d"), // Control
                StringToByteArray("70000c0c030000000000000000ff090100000d2d"),
                StringToByteArray("170500004231550001010e3d0c0330381f000430"),

                StringToByteArray("1237230005303938000630372700073038220008"),
                StringToByteArray("1430371c0009303612000a303712000b30371600"),
                StringToByteArray("160c303610000e30371b000f3031000000000000"),

                StringToByteArray("70000c0c010000000000000000ff090100000d2d"),
                StringToByteArray("70000c0c030000000000000000ff090100000d2d"),
                StringToByteArray("1705000042d3e30001010e3d0101313682000000"),

                StringToByteArray("1200000000000000000000000000000000000000"),
                StringToByteArray("1400000000000000000000000000000000000000"),
                StringToByteArray("1600000000000000000000000000000000000000"),

                StringToByteArray("70000c0c010000000000000000ff090100000d2d"),
                StringToByteArray("1105000009c1f500010181043130120082000000"),

                StringToByteArray("70000c0c010000000000000000ff090100000d2d"),
                StringToByteArray("1105000005435c00010156003100000000000000"),

                StringToByteArray("70000c0c010000000000000000ff090100000d2d"),
                StringToByteArray("11050000056eec00010108003100000000000000"),

                StringToByteArray("70000c0c010000000000000000ff090100000d2d"),
                StringToByteArray("11050000056eec00010108003100000000000000"),
                // Value: 1104ff000757d001010803060500000000000000
                StringToByteArray("70000c0c010000000000000000ff090100000d2d"),
                StringToByteArray("11050000056eec00010108003100000000000000"),
                // Value: 1104ff0007971701010803050200000000000000
                StringToByteArray("70000c0c010000000000000000ff090100000d2d"),
                StringToByteArray("11050000056eec00010108003100000000000000"), //588
                // Value: 1104ff000743d501010803030000000000000000
                // Value: 6000080000000000000000000000000000000000
                StringToByteArray("70000c0c010000000000000000ff090100000d2d"),
                StringToByteArray("11050000056eec00010108003100000000000000"),
                // Value: 1104ff0007841401010803090000000000000000
                StringToByteArray("70000c0c010000000000000000ff090100000d2d"),
                StringToByteArray("11050000056eec00010108003100000000000000"),
                // Value: 1104ff00071e8401010803010100000000000000
                StringToByteArray("70000c0c010000000000000000ff090100000d2d"),
                StringToByteArray("11050000056eec00010108003100000000000000"),
                // Value: 1104ff00071a8501010803000000000000000000
                StringToByteArray("70000c0c010000000000000000ff090100000d2d"),
                StringToByteArray("11050000056eec00010108003100000000000000"),
                // Value: 1104ff00074cd101010803080500000000000000
                // Value: 6000080000000000000000000000000000000000
                StringToByteArray("70000c0c010000000000000000ff090100000d2d"), // Control Frame
                StringToByteArray("11050000056eec00010108003100000000000000"), // Single Frame
                // Value: 1104ff0007ca4601010803070300000000000000
                // Value: 6000080000000000000000000000000000000000
                StringToByteArray("70000c0c010000000000000000ff090100000d2d"), // Control Frame
                StringToByteArray("11050000056eec00010108003100000000000000"), // Single Frame
                // Value: 1104ff00063bde01015302000400000000000000
                StringToByteArray("70000c0c010000000000000000ff090100000d2d"),
                StringToByteArray("110500000777a100010153020200120000000000"), //623


                StringToByteArray("70000c0c030000000000000000ff090100000d2d"), // 757
                StringToByteArray("1705000042e0df000101593d0b0003000000011d"),
                StringToByteArray("12010000026d0100000332000000040000000005"),
                StringToByteArray("14000000000600000000072a0100000800000000"),
                StringToByteArray("1609000000000a00000000000000000000000000"),

                StringToByteArray("70000c0c030000000000000000ff090100000d2d"),
                StringToByteArray("170500004210690001010d3d0700000000000100"),

                StringToByteArray("1200000002000000000300000000040000000005"),
                StringToByteArray("1400000000060000000000000000000000000000"),
                StringToByteArray("1600000000000000000000000000000000000000"),

                StringToByteArray("70000c0c030000000000000000ff090100000d2d"),
                StringToByteArray("170500004210690001010d3d0700000000000100"),
                StringToByteArray("1200000002000000000300000000040000000005"),
                StringToByteArray("1400000000060000000000000000000000000000"),
                StringToByteArray("1600000000000000000000000000000000000000"),

                // Value: 1104ff000576ce010109010a0000000000000000   // Open command?
                StringToByteArray("70000c0c010000000000000000ff090100000d2d"),
                StringToByteArray("11050000055ddd00010109000700000000000000"), // Command Response 

                StringToByteArray("70000c0c030000000000000000ff090100000d2d"),
                StringToByteArray("170500004210690001010d3d0700000000000100"),
                StringToByteArray("1200000002000000000300000000040000000005"),
                StringToByteArray("1400000000060000000000000000000000000000"),
                StringToByteArray("1600000000000000000000000000000000000000"), // 798

                StringToByteArray("70000c0c010000000000000000ff090100000d2d"),
                StringToByteArray("11050000055ddd00010109000700000000000000"),

                StringToByteArray("70000c0c030000000000000000ff090100000d2d"),
                StringToByteArray("170500004210690001010d3d0700000000000100"),
                StringToByteArray("1200000002000000000300000000040000000005"),
                StringToByteArray("1400000000060000000000000000000000000000"),
                StringToByteArray("1600000000000000000000000000000000000000"),

                StringToByteArray("70000c0c010000000000000000ff090100000d2d"),
                StringToByteArray("11050000055ddd00010109000700000000000000"),

                StringToByteArray("70000c0c010000000000000000ff090100000d2d"),

                StringToByteArray("70000c0c030000000000000000ff090100000d2d"),
                StringToByteArray("170500004210690001010d3d0700000000000100"),
                StringToByteArray("1200000002000000000300000000040000000005"),
                StringToByteArray("1400000000060000000000000000000000000000"),
                StringToByteArray("1600000000000000000000000000000000000000"),

                StringToByteArray("70000c0c010000000000000000ff090100000d2d"),

                StringToByteArray("11050000055ddd00010109000700000000000000"), //844

                //StringToByteArray("11050000056eec00010108003100000000000000"),
                //StringToByteArray("11050000056eec00010108003100000000000000"),
                //StringToByteArray("11050000056eec00010108003100000000000000"),
                //StringToByteArray("11050000056eec00010108003100000000000000"),
                //StringToByteArray("11050000056eec00010108003100000000000000"),
                //StringToByteArray("11050000056eec00010108003100000000000000"),
                //StringToByteArray("11050000056eec00010108003100000000000000"),
                //StringToByteArray("11050000056eec00010108003100000000000000"),
                //StringToByteArray("11050000056eec00010108003100000000000000"),
                //StringToByteArray("11050000056eec00010108003100000000000000"),
                //StringToByteArray("11050000056eec00010108003100000000000000"),
                //StringToByteArray("11050000056eec00010108003100000000000000"),
                //StringToByteArray("11050000056eec00010108003100000000000000"),
                //StringToByteArray("11050000056eec00010108003100000000000000"),


            };
            return data;
        }



        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        private static async Task PairDeviceIfNecessary(DeviceInformation device)
        {
            if (device.Pairing.IsPaired)
            {
                return;
            }
            else
            {
                if (device.Pairing.CanPair)
                {
                    var customPairing = device.Pairing.Custom;
                    customPairing.PairingRequested += (sender, pairingRequestArgs) =>
                    {
                        pairingRequestArgs.Accept("00000");
                    };

                    var result = await customPairing.PairAsync(DevicePairingKinds.ConfirmOnly);
                    if ((result.Status == DevicePairingResultStatus.Paired) ||
                        (result.Status == DevicePairingResultStatus.AlreadyPaired))
                    {
                        return;
                    }
                    else
                    {
                        throw new Exception(String.Format("Could not pair device. Pairing status: {0}", result.Status));
                    }
                }
                throw new Exception("Pairing ot supported by device.");

            }
        }
    }

   
}
