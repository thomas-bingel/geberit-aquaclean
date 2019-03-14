using System;
using System.Threading.Tasks;

namespace Geberit.AquaClean.Core
{
    public interface IBluetoothLeConnector
    {
        event EventHandler<byte[]> DataReceived;
        event EventHandler<bool> ConnectionStatusChanged;

        void Disconnect();
        Task ConnectAsync(string id);
        Task SendMessageAsync(byte[] data);
    }
}