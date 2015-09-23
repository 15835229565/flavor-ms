using System;
using System.Collections.Generic;

namespace Flavor.Common.Messaging.Almazov.Commands {
    using AsyncErrorReply = AsyncError<CommandCode>;

    class LAMCriticalError: AsyncErrorReply {
        readonly byte error;
        public LAMCriticalError(byte error) {
            this.error = error;
        }
        public LAMCriticalError(): this(0) { }
        enum Critical_Error {
            TIC_error_decode = 30, //Ошибка декодирования сообщения от TIC'a!
            TIC_error_noResponse = 31,//TIC не ответил!
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
        public LAMInternalError() : this(0) { }
        enum Internal_Error {
            USART_COMP = 1,       //Внутренняя ошибка приёма данных от ПК
            SPI = 2,              //SPI-устройства с таким номером нет  
            TIC_state = 3,        //Неверное состояние TIC таймера!
        }
        public override string Message {
            get { return "Internal error " + error; }
        }
    }
}
