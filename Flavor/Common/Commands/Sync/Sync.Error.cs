using CommandCode = Flavor.Common.Messaging.ModBusNew.CommandCode;

namespace Flavor.Common.Messaging.Commands {
    internal abstract class SyncErrorReply: ServicePacket.Sync {
        internal class logInvalidCommand: SyncErrorReply {
            //private byte[] command;

            internal logInvalidCommand(byte[] errorcommand) {
                //command = errorcommand;
            }

            internal override CommandCode Id {
                get { return CommandCode.InvalidCommand; }
            }
        }

        internal class logInvalidChecksum: SyncErrorReply {
            internal override CommandCode Id {
                get { return CommandCode.InvalidChecksum; }
            }
        }

        internal class logInvalidPacket: SyncErrorReply {
            internal override CommandCode Id {
                get { return CommandCode.InvalidPacket; }
            }
        }

        internal class logInvalidLength: SyncErrorReply {
            internal override CommandCode Id {
                get { return CommandCode.InvalidLength; }
            }
        }

        internal class logInvalidData: SyncErrorReply {
            internal override CommandCode Id {
                get { return CommandCode.InvalidData; }
            }
        }

        internal class logInvalidState: SyncErrorReply {
            internal override CommandCode Id {
                get { return CommandCode.InvalidState; }
            }
        }
    }
}