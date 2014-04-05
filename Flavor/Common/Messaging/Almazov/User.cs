using System.Collections.Generic;
using CommandCode = Flavor.Common.Messaging.Almazov.CommandCode;
using UserRequest = Flavor.Common.Messaging.UserRequest<Flavor.Common.Messaging.Almazov.CommandCode>;

namespace Flavor.Common.Messaging.Almazov {
    class CPUStatusRequest: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.CPU_Status; }
        }
        public override IList<byte> Data {
            get { return AlexProtocol.collectData((byte)Id); }
        }
    }
}