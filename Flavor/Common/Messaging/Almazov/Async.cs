using AsyncReply = Flavor.Common.Messaging.Async<Flavor.Common.Messaging.Almazov.CommandCode>;

namespace Flavor.Common.Messaging.Almazov {
    class LAMEvent: AsyncReply {
        public readonly byte number;
        public LAMEvent(byte number) {
            this.number = number;
        }
        enum LAM: byte {
            RTC_end = 20,      //RTC закончил измерение
            SPI_conf_done = 21,//После включения HVE все SPI устройства были настроены!
            HVEnabled = 22,
            HVDisabled = 23,
        }
    }
}