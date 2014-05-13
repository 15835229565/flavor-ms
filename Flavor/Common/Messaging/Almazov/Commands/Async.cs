using AsyncReply = Flavor.Common.Messaging.Async<Flavor.Common.Messaging.Almazov.CommandCode>;

namespace Flavor.Common.Messaging.Almazov.Commands {
    abstract class LAMEvent: AsyncReply {
        enum LAM: byte {
            RTC_end = 20,      //RTC measure ended
            SPI_conf_done = 21,//Wait for several seconds until this message
            HVEnabled = 22,    //After this HVE=true
            HVDisabled = 23,   //After this HVE=false
        }
        public static LAMEvent Parse(byte number) {
            switch (number) {
                case 20:
                    return new RTCMeasureEndLAM();
                case 21:
                    return new SPIConfDoneLAM();
                case 22:
                    return new HVEnabledLAM();
                case 23:
                    return new HVDisabledLAM();
                default:
                    return null;
            }
        }
    }
    class RTCMeasureEndLAM: LAMEvent { }
    class SPIConfDoneLAM: LAMEvent, IUpdateDevice {
        public void UpdateDevice(IDevice device) {
            device.OperationBlock(false);
        }
        public void UpdateDevice() {
            throw new System.NotSupportedException();
        }
    }
    class HVEnabledLAM: LAMEvent, IUpdateDevice {
        public void UpdateDevice(IDevice device) {
            device.OperationReady(true);
        }
        public void UpdateDevice() {
            throw new System.NotSupportedException();
        }
    }
    class HVDisabledLAM: LAMEvent, IUpdateDevice {
        public void UpdateDevice(IDevice device) {
            device.OperationReady(false);
        }
        public void UpdateDevice() {
            throw new System.NotSupportedException();
        }
    }
}