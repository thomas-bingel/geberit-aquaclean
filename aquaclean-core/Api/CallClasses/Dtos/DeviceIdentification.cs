using System;
using System.Collections.Generic;
using System.Text;

namespace Geberit.AquaClean.Core.Api
{
    public struct DeviceIdentification
    {
        [DeSerialize(Length = 12)]
        public string SapNumber { get; set; }
        [DeSerialize(Length = 20)]
        public string SerialNumber { get; set; }
        [DeSerialize(Length = 10)]
        public string ProductionDate { get; set; }
        [DeSerialize(Length = 40)]
        public string Description { get; set; }

        public override string ToString()
        {
            return String.Format("DeviceIdentification: SapNumber={0}, SerialNumber={1}, ProductionDate={2}, Description={3}",
                SapNumber, SerialNumber, ProductionDate, Description);
        }
    }
}
