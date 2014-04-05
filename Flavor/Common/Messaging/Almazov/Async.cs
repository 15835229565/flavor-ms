using AsyncReply = Flavor.Common.Messaging.Async<Flavor.Common.Messaging.Almazov.CommandCode>;

namespace Flavor.Common.Messaging.Almazov {
    class LAMEvent: AsyncReply {
        readonly byte number;
        public LAMEvent(byte number) {
            this.number = number;
        }
        enum LAM : byte {
            Unknown = 0,
            RTC_end = 1,      //RTC закончил измерение
            SPI_conf_done = 2,//После включения HVE все SPI устройства были настроены!
        }
    }
}