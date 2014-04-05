using System.Collections.Generic;
using CommandCode = Flavor.Common.Messaging.Almazov.CommandCode;
using SyncError = Flavor.Common.Messaging.SyncError<Flavor.Common.Messaging.Almazov.CommandCode>;

namespace Flavor.Common.Messaging.Almazov {
    class SyncErrorReply: SyncError {
        readonly byte code, data;
        public SyncErrorReply(byte code, byte data) {
            this.code = code;
            this.data = data;
        }
        enum Code : byte {
            Unknown = 0,
            Decoder = 1,       //Такой команды не существует
            CheckSum = 2,       //Неверная контрольная сумма
        }

        public override CommandCode Id {
            get { return CommandCode.Sync_Error; }
        }
    }
}