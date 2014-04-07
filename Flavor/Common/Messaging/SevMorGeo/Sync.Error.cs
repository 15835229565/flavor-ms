using System.Collections.Generic;
using CommandCode = Flavor.Common.Messaging.SevMorGeo.CommandCode;
using SyncError = Flavor.Common.Messaging.SyncError<Flavor.Common.Messaging.SevMorGeo.CommandCode>;

namespace Flavor.Common.Messaging.SevMorGeo {
    class logInvalidCommand: SyncError {
        //private byte[] command;

        public logInvalidCommand(IList<byte> errorcommand) {
            //command = errorcommand;
        }

        public override CommandCode Id {
            get { return CommandCode.InvalidCommand; }
        }
    }

    class logInvalidChecksum: SyncError {
        public override CommandCode Id {
            get { return CommandCode.InvalidChecksum; }
        }
    }

    class logInvalidPacket: SyncError {
        public override CommandCode Id {
            get { return CommandCode.InvalidPacket; }
        }
    }

    class logInvalidLength: SyncError {
        public override CommandCode Id {
            get { return CommandCode.InvalidLength; }
        }
    }

    class logInvalidData: SyncError {
        public override CommandCode Id {
            get { return CommandCode.InvalidData; }
        }
    }

    class logInvalidState: SyncError {
        public override CommandCode Id {
            get { return CommandCode.InvalidState; }
        }
    }
}