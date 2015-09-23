using System.Collections.Generic;

namespace Flavor.Common.Messaging.Almazov.Commands {
    using SyncError = SyncError<CommandCode>;

    class SyncErrorReply: SyncError {
        readonly byte code;
        public SyncErrorReply(byte code) {
            this.code = code;
        }
        public SyncErrorReply() : this(0) { }
        enum Code: byte {
            CheckSum = 1,       //Неверная контрольная сумма
            Decoder = 10,       //Такой команды не существует
        }
        public override CommandCode Id {
            get { return CommandCode.Service_Message; }
        }
    }
}