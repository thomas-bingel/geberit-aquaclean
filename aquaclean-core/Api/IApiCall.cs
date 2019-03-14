using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geberit.AquaClean.Core.Api
{
    interface IApiCall
    {
        byte[] GetPayload();
    }
}
