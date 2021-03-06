﻿using Flavor.Common.Data.Controlled;

namespace Flavor.Common.Messaging.Almazov.Commands {
    using AsyncReply = Async<CommandCode>;

    abstract class LAMEvent: AsyncReply {
        enum LAM: byte {
            RTC_end = 20,      //RTC measure ended
            SPI_conf_done = 21,//Wait for several seconds until this message
            HVEnabled = 22,    //After this HVE=true
            HVDisabled = 23,   //After this HVE=false
        }
        public static LAMEvent Parse(byte number, params byte[] data) {
            switch (number) {
                case 20:
                    return new RTCMeasureEndLAM();
                case 21:
                    return new SPIConfDoneLAM();
                case 22:
                    return new HVEnabledLAM();
                case 23:
                    return new HVDisabledLAM(data[0]);
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
        public HVDisabledLAM(byte reason) {
            //TODO: check reason
        }
        public HVDisabledLAM() { }
        public void UpdateDevice(IDevice device) {
            device.OperationReady(false);
        }
        public void UpdateDevice() {
            throw new System.NotSupportedException();
        }
    }
}