using System;
using System.Collections.Generic;
using System.Text;

namespace Geberit.AquaClean.Core.Api
{
    public struct SOCApplicationVersion
    {
        [DeSerialize(Length = 2)]
        public byte[] A { get; set; }

        [DeSerialize(Length = 1)]
        public int B { get; set; }
    }
}
