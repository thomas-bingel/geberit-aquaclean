using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geberit.AquaClean.Core.Api
{
    internal class ApiCallAttribute : Attribute
    {
        public byte Context { get; set; }
        public byte Procedure { get; set; }
        public byte Node { get; set; }
    }
}
