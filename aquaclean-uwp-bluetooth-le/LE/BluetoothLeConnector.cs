using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using Geberit.AquaClean.Core.Common;

namespace Geberit.AquaClean.Core.LE
{
    public class BluetoothLeConnector : IBluetoothLeConnector
    {
        private BluetoothLEDevice bluetoothLeDevice;

        private Guid SERVICE_UUID = Guid.Parse("3334429d-90f3-4c41-a02d-5cb3a03e0000");
        private static Guid BULK_CHAR_BULK_WRITE_0_UUID_STRING = Guid.Parse("3334429d-90f3-4c41-a02d-5cb3a13e0000");
        private static Guid BULK_CHAR_BULK_WRITE_1_UUID_STRING = Guid.Parse("3334429d-90f3-4c41-a02d-5cb3a23e0000");
        private static Guid BULK_CHAR_BULK_WRITE_2_UUID_STRING = Guid.Parse("3334429d-90f3-4c41-a02d-5cb3a33e0000");
        private static Guid BULK_CHAR_BULK_WRITE_3_UUID_STRING = Guid.Parse("3334429d-90f3-4c41-a02d-5cb3a43e0000");
        private static Guid BULK_CHAR_BULK_READ_0_UUID_STRING = Guid.Parse("3334429d-90f3-4c41-a02d-5cb3a53e0000");
        private static Guid BULK_CHAR_BULK_READ_1_UUID_STRING = Guid.Parse("3334429d-90f3-4c41-a02d-5cb3a63e0000");
        private static Guid BULK_CHAR_BULK_READ_2_UUID_STRING = Guid.Parse("3334429d-90f3-4c41-a02d-5cb3a73e0000");
        private static Guid BULK_CHAR_BULK_READ_3_UUID_STRING = Guid.Parse("3334429d-90f3-4c41-a02d-5cb3a83e0000");
        private static Guid CCC_UUID_STRING = Guid.Parse("00002902-0000-1000-8000-00805f9b34fb");

        Dictionary<Guid, EventHandler<byte[]>> readCharacteristics = new Dictionary<Guid, EventHandler<byte[]>>();
        private readonly object syncObj = new object();
        public event EventHandler<byte[]> DataReceived;
        public event EventHandler<bool> ConnectionStatusChanged;
        private GattCharacteristic writeCharacteristic;

        public void Disconnect()
        {
            bluetoothLeDevice.Dispose();
        }

        public async Task ConnectAsync(string id)
        {
            Debug.WriteLine("Connecting to " + id);
            bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(id);

            if (bluetoothLeDevice == null)
            {
                throw new Exception("AquaClean device not found");
            }
            Debug.WriteLine("Connected");

            bluetoothLeDevice.ConnectionStatusChanged += async (BluetoothLEDevice device, object obj) =>
            {
                Debug.WriteLine("Connection status changed to: " + device.ConnectionStatus);
                ConnectionStatusChanged?.Invoke(this, device.ConnectionStatus == BluetoothConnectionStatus.Connected);
            };

            // Register read characteristics
            readCharacteristics.Add(BULK_CHAR_BULK_READ_0_UUID_STRING, DataReceived);
            readCharacteristics.Add(BULK_CHAR_BULK_READ_1_UUID_STRING, DataReceived);
            readCharacteristics.Add(BULK_CHAR_BULK_READ_2_UUID_STRING, DataReceived);
            readCharacteristics.Add(BULK_CHAR_BULK_READ_3_UUID_STRING, DataReceived);

            // Load write characteristic
            var service = await bluetoothLeDevice.GetGattServiceForUuidAsync(SERVICE_UUID);
            var characteristicsResult = await service.GetCharacteristicsAsync();
            var characteristics = characteristicsResult.Characteristics;
            writeCharacteristic = characteristics.First((c) => c.Uuid == BULK_CHAR_BULK_WRITE_0_UUID_STRING);


            await ListServices();
        }

        private async Task ListServices()
        {
            var service = await bluetoothLeDevice.GetGattServiceForUuidAsync(SERVICE_UUID);

            foreach (var characteristics in await service.GetCharacteristics2Async())
            {
                if (readCharacteristics.ContainsKey(characteristics.Uuid))
                {
                    Debug.WriteLine("Registering characteristics Uuid=" + characteristics.Uuid);
                    characteristics.ValueChanged += (GattCharacteristic sender, GattValueChangedEventArgs args) =>
                    {
                        var eventHandler = readCharacteristics.GetValueOrDefault(characteristics.Uuid);
                        var data = ReadData(args);
                        Debug.WriteLine(String.Format("{2}: Received data from characteristic {0} data: {1}",
                            characteristics.Uuid, data.ToByteString(), DateTime.Now));
                        lock (syncObj)
                        {
                            eventHandler?.Invoke(this, data);
                        }
                    };
                    await characteristics.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                }
            }

            var accessStatus = await service.RequestAccessAsync();
            var openStatus = await service.OpenAsync(GattSharingMode.Exclusive);


        }

        private static byte[] ReadData(GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
            reader.ByteOrder = ByteOrder.LittleEndian;
            var byteArray = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(byteArray);
            return byteArray;
        }

        public async Task SendMessageAsync(byte[] data)
        {
            Debug.WriteLine(String.Format("{2}: Sending data to characteristic {0} data: {1}",
                writeCharacteristic.Uuid, data.ToByteString(), DateTime.Now));
            await writeCharacteristic.WriteBytesAsync(data);
        }

    }
}
