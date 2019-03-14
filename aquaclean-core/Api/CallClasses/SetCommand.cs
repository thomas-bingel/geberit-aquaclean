using Geberit.AquaClean.Core.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geberit.AquaClean.Core.Api
{
    [ApiCall(Context=0x01, Procedure=0x09, Node=0x00)]
    class SetCommand : IApiCall
    {
        public Commands Command { get; set; }

        public SetCommand(Commands command)
        {
            this.Command = command;
        }

        public byte[] GetPayload()
        {
            return new byte[] { (byte) this.Command };
        }

    }
}
