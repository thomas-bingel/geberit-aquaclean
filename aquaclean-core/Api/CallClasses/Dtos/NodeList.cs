using System;
using System.Collections.Generic;
using System.Text;

namespace Geberit.AquaClean.Core.Api
{
    public struct NodeList
    {
        [DeSerialize(Length = 1)]
        public int A { get; set; }
        [DeSerialize(Length = 128)]
        public byte[] B { get; set; }

    }
}
