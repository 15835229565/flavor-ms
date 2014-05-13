using AsyncReply = Flavor.Common.Messaging.Async<Flavor.Common.Messaging.SevMorGeo.CommandCode>;

namespace Flavor.Common.Messaging.SevMorGeo {
    class requestCounts: AsyncReply {
    }

    class confirmVacuumReady: AsyncReply, IUpdateDevice {
        #region IUpdateDevice Members
        public void UpdateDevice(IDevice device) {
            throw new System.NotImplementedException();
        }
        public void UpdateDevice() { }
        #endregion
    }

    class confirmShutdowned: AsyncReply { }

    class SystemReseted: AsyncReply { }

    class confirmHighVoltageOff: AsyncReply { }

    class confirmHighVoltageOn: AsyncReply { }
}