using System;
using System.Collections.Generic;
using AsyncErrorReply = Flavor.Common.Messaging.AsyncError<Flavor.Common.Messaging.Almazov.CommandCode>;

namespace Flavor.Common.Messaging.Almazov {
    class LAMCriticalError: AsyncErrorReply {
        readonly byte error;
        public LAMCriticalError(byte error) {
            this.error = error;
        }
        enum Critical_Error {
            Unknown = 0,
            HVE_error_decode = 1, //Ошибка декодирования сообщения от TIC'a!
            HVE_error_noResponse = 2,//TIC не ответил!
        }
        public override string Message {
            get { return "Critical error " + error; }
        }
    }
    [Obsolete]
    class LAMInternalError: AsyncErrorReply {
        readonly byte error;
        public LAMInternalError(byte error) {
            this.error = error;
        }
        enum Critical_Error {
            Unknown = 0,
            USART_COMP = 1,       //Внутренняя ошибка приёма данных от ПК
            SPI = 2,              //SPI-устройства с таким номером нет  
            TIC_state = 3,        //Неверное состояние TIC таймера!
        }
        public override string Message {
            get { return "Internal error " + error; }
        }
    }
}
