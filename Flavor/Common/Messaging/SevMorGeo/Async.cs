namespace Flavor.Common.Messaging.SevMorGeo {
    using AsyncReply = Async<CommandCode>;
    
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