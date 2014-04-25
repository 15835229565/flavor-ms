using System.Collections.Generic;
using CommandCode = Flavor.Common.Messaging.Almazov.CommandCode;
using UserRequest = Flavor.Common.Messaging.UserRequest<Flavor.Common.Messaging.Almazov.CommandCode>;

namespace Flavor.Common.Messaging.Almazov {
    class CPUStatusRequest: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.CPU_Status; }
        }
        public override IList<byte> Data {
            get { return AlexProtocol.collectData(Id); }
        }
    }
    class HighVoltagePermittedStatusRequest: UserRequest {
        public override IList<byte> Data {
            get { return AlexProtocol.collectData(Id); }
        }
        public override CommandCode Id {
            get { return CommandCode.HVE; }
        }
    }
    class OperationBlockRequest: UserRequest {
        readonly bool? on;
        public OperationBlockRequest(bool? on) {
            this.on = on;
        }
        public override IList<byte> Data {
            get {
                if (on.HasValue) {
                    ;
                }
                return AlexProtocol.collectData(Id, on.HasValue ? (byte)(on.Value ? 1 : 0) : byte.MaxValue);
            }
        }
        public override CommandCode Id {
            get { return CommandCode.PRGE; }
        }
    }
    class TICStatusRequest: UserRequest {
        public readonly string Request = "?V902\r";
        public override IList<byte> Data {
            get { return AlexProtocol.collectData(Id, Request); }
        }
        public override CommandCode Id {
            get { return CommandCode.TIC_Retransmit; }
        }
        public override bool Equals(object other) {
            // BAD: asymmetric
            if (base.Equals(other))
                return (other as TICStatusRequest).Request.Equals(this.Request);
            return false;
        }
        public override int GetHashCode() {
            return base.GetHashCode() + 17 * Request.GetHashCode();
        }
    }
    class Valve1Request: UserRequest {
        readonly bool? on;
        public Valve1Request(bool? on) {
            this.on = on;
        }
        public override IList<byte> Data {
            get { return AlexProtocol.collectData(Id, on.HasValue ? (byte)(on.Value ? 1 : 0) : byte.MaxValue); }
        }
        public override CommandCode Id {
            get { return CommandCode.SEMV1; }
        }
    }
}