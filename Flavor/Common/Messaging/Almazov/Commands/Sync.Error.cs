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
            CheckSum = 1,       //�������� ����������� �����
            Decoder = 10,       //����� ������� �� ����������
        }
        public override CommandCode Id {
            get { return CommandCode.Service_Message; }
        }
    }
}