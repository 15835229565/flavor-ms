using System.Collections.Generic;
using CommandCode = Flavor.Common.Messaging.Almazov.CommandCode;
using SyncError = Flavor.Common.Messaging.SyncError<Flavor.Common.Messaging.Almazov.CommandCode>;

namespace Flavor.Common.Messaging.Almazov {
    class SyncErrorReply: SyncError {
        readonly byte code;
        public SyncErrorReply(byte code) {
            this.code = code;
        }
        enum Code: byte {
            CheckSum = 1,       //Неверная контрольная сумма
            Decoder = 10,       //Такой команды не существует
        }

        public override CommandCode Id {
            get { return CommandCode.Service_Message; }
        }
    }
}