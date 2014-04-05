using System.Collections.Generic;
using CommandCode = Flavor.Common.Messaging.SevMorGeo.CommandCode;
using SyncError = Flavor.Common.Messaging.SyncError<Flavor.Common.Messaging.SevMorGeo.CommandCode>;

namespace Flavor.Common.Messaging.SevMorGeo {
    internal class logInvalidCommand: SyncError {
        //private byte[] command;

        internal logInvalidCommand(IList<byte> errorcommand) {
            //command = errorcommand;
        }

        public override CommandCode Id {
            get { return CommandCode.InvalidCommand; }
        }
    }

    internal class logInvalidChecksum: SyncError {
        public override CommandCode Id {
            get { return CommandCode.InvalidChecksum; }
        }
    }

    internal class logInvalidPacket: SyncError {
        public override CommandCode Id {
            get { return CommandCode.InvalidPacket; }
        }
    }

    internal class logInvalidLength: SyncError {
        public override CommandCode Id {
            get { return CommandCode.InvalidLength; }
        }
    }

    internal class logInvalidData: SyncError {
        public override CommandCode Id {
            get { return CommandCode.InvalidData; }
        }
    }

    internal class logInvalidState: SyncError {
        public override CommandCode Id {
            get { return CommandCode.InvalidState; }
        }
    }
}