using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;


namespace Geberit.AquaClean.Core.LE
{
    internal static class BluetoothExtention
    {
        public static async Task<byte[]> ReadBytesAsync(this GattCharacteristic gatt)
        {
            var result = await gatt.ReadValueAsync(BluetoothCacheMode.Uncached);
            if (result.Status != GattCommunicationStatus.Success)
            {
                throw new Exception("Could not read from Characteristic UUID=" + gatt.Uuid);
            }
            var reader = DataReader.FromBuffer(result.Value);
            reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
            reader.ByteOrder = ByteOrder.LittleEndian;
            var data = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(data);
            return data;

        }

        public static async Task<GattDeviceService> GetGattServiceForUuidAsync(this BluetoothLEDevice bluetoothLEDevice, Guid uuid)
        {
            var serviceResult = await bluetoothLEDevice.GetGattServicesForUuidAsync(uuid);
            if (serviceResult.Status != GattCommunicationStatus.Success)
            {
                throw new Exception("Could not find GATT service with UUID=" + uuid.ToString());
            }
            return serviceResult.Services.FirstOrDefault();
        }

        public static async Task<IReadOnlyList<GattCharacteristic>> GetCharacteristics2Async(this GattDeviceService service)
        {
            var result = await service.GetCharacteristicsAsync();
            if (result.Status != GattCommunicationStatus.Success)
            {
                throw new Exception("Could not get characteristics from service");
            }
            return result.Characteristics;
        }

        public static async Task WriteBytesAsync(this GattCharacteristic gatt, byte[] data)
        {
            var writeStatus = await gatt.WriteValueWithResultAsync(data.AsBuffer());

            if (writeStatus.Status != GattCommunicationStatus.Success)
            {
                throw new Exception("Could not write data to Characteristic UUID=" + gatt.Uuid);
            }
        }
    }
}
