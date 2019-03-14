
using System;
using System.Collections.Generic;
using System.Text;

namespace Geberit.AquaClean.Core.Api
{
    public struct SystemParameterList
    {
        [DeSerialize(Length = 1)]
        public int A { get; set; }
        [DeSerialize(Length = 60)]
        public int[] DataArray { get; set; }
    }
}
