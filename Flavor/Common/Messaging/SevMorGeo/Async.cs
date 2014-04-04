using AsyncReply = Flavor.Common.Messaging.ServicePacket<Flavor.Common.Messaging.SevMorGeo.CommandCode>.Async;

namespace Flavor.Common.Messaging.SevMorGeo {
    internal class requestCounts: AsyncReply, IAutomatedReply {
        #region IReply Members
        public ISend AutomatedReply() {
            //хорошо бы сюда на автомате очистку Commander.CustomMeasure...
            return new getCounts();
        }
        #endregion
    }

    internal class confirmVacuumReady: AsyncReply, IUpdateDevice {
        #region IUpdateDevice Members
        public void UpdateDevice() {
        }
        #endregion
    }

    internal class confirmShutdowned: AsyncReply { }

    internal class SystemReseted: AsyncReply { }

    internal class confirmHighVoltageOff: AsyncReply { }

    internal class confirmHighVoltageOn: AsyncReply { }
}