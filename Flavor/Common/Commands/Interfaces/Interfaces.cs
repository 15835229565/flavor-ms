using CommandCode = Flavor.Common.Messaging.ModBusNew.CommandCode;

namespace Flavor.Common.Messaging.Commands {
    internal abstract class ServicePacket {
        internal abstract class Sync: ServicePacket {
            internal abstract CommandCode Id { get; }
        }
    }

    internal interface ISend {
        byte[] Data { get; }
    }

    interface IAutomatedReply {
        UserRequest AutomatedReply();
    }

    interface IUpdateDevice {
        void UpdateDevice();
    }

    interface IUpdateGraph {
    }
}