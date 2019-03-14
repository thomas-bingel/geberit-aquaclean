using System;
using System.Collections.Generic;
using System.Text;

namespace Geberit.AquaClean.Core.Api
{
    public struct StatisticsDescale
    {
        [DeSerialize(Length = 1)]
        public int UnpostedShowerCycles { get; set; }
        [DeSerialize(Length = 2)]
        public int DaysUntilNextDescale { get; set; }
        [DeSerialize(Length = 2)]
        public int DaysUntilShowerRestricted { get; set; }
        [DeSerialize(Length = 1)]
        public int ShowerCyclesUntilConfirmation { get; set; }
        [DeSerialize(Length = 4)]
        public int DateTimeAtLastDescale { get; set; }
        [DeSerialize(Length = 4)]
        public int DateTimeAtLastDescalePrompt { get; set; }
        [DeSerialize(Length = 2)]
        public int NumberOfDescaleCycles { get; set; }

    }
}
