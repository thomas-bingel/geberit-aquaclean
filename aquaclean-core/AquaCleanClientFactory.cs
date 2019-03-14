using Geberit.AquaClean.Core.Clients;
using System;

namespace Geberit.AquaClean.Core
{
    public class AquaCleanClientFactory
    {
        internal IBluetoothLeConnector BluetoothLeConnector { get; }

        public AquaCleanClientFactory(IBluetoothLeConnector bluetoothLeConnector)
        {
            this.BluetoothLeConnector = bluetoothLeConnector;
        }

        public IAquaCleanClient CreateClient()
        {
            return new AquaCleanClient(this.BluetoothLeConnector);
        }

        public IAquaCleanBaseClient CreateBaseClient()
        {
            return new AquaCleanBaseClient(this.BluetoothLeConnector);
        }
    }
}
