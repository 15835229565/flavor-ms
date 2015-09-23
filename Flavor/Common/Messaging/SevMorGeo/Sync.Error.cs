using System.Collections.Generic;

namespace Flavor.Common.Messaging.SevMorGeo {
    using SyncError = SyncError<CommandCode>;

    class logInvalidCommand: SyncError {
        public logInvalidCommand(IList<byte> errorcommand) { }
        public logInvalidCommand() { }
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