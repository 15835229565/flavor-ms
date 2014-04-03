using CommandCode = Flavor.Common.Messaging.SevMorGeo.ModBus.CommandCode;
using SyncErrorReply = Flavor.Common.Messaging.ServicePacket<Flavor.Common.Messaging.SevMorGeo.ModBus.CommandCode>.SyncError;

namespace Flavor.Common.Messaging.SevMorGeo {
    internal class logInvalidCommand: SyncErrorReply {
        //private byte[] command;

        internal logInvalidCommand(byte[] errorcommand) {
            //command = errorcommand;
        }

        public override CommandCode Id {
            get { return CommandCode.InvalidCommand; }
        }
    }

    internal class logInvalidChecksum: SyncErrorReply {
        public override CommandCode Id {
            get { return CommandCode.InvalidChecksum; }
        }
    }

    internal class logInvalidPacket: SyncErrorReply {
        public override CommandCode Id {
            get { return CommandCode.InvalidPacket; }
        }
    }

    internal class logInvalidLength: SyncErrorReply {
        public override CommandCode Id {
            get { return CommandCode.InvalidLength; }
        }
    }

    internal class logInvalidData: SyncErrorReply {
        public override CommandCode Id {
            get { return CommandCode.InvalidData; }
        }
    }

    internal class logInvalidState: SyncErrorReply {
        public override CommandCode Id {
            get { return CommandCode.InvalidState; }
        }
    }
}