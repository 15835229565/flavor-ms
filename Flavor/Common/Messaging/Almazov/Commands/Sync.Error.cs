using System.Collections.Generic;
using CommandCode = Flavor.Common.Messaging.Almazov.CommandCode;
using SyncError = Flavor.Common.Messaging.SyncError<Flavor.Common.Messaging.Almazov.CommandCode>;

namespace Flavor.Common.Messaging.Almazov.Commands {
    class SyncErrorReply: SyncError {
        readonly byte code;
        public SyncErrorReply(byte code) {
            this.code = code;
        }
        public SyncErrorReply() : this(0) { }
        enum Code: byte {
            CheckSum = 1,       //�������� ����������� �����
            Decoder = 10,       //����� ������� �� ����������
        }
        public override CommandCode Id {
            get { return CommandCode.Service_Message; }
        }
    }
}