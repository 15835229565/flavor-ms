using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor.Common.Messaging.Almazov {
    class AlexProtocol: CheckableProtocol<CommandCode> {
        public AlexProtocol(PortLevel port, CommandDictionary<CommandCode> dictionary)
            : base(new AlexProtocolByteDispatcher(port, false), GetDictionary()) { }

        protected override byte ComputeCS(IList<byte> data) {
            byte checkSum = 0;
            foreach (byte b in data)
                checkSum -= b;
            return checkSum;
        }
        protected override IList<byte> buildPackBody(IList<byte> data, byte checksum) {
            var pack = new List<byte>(data);
            pack.Add(checksum);
            return pack;
        }

        public static IList<byte> collectData(byte functCode, params object[] values) {
            List<byte> data = new List<byte>();
            data.Add(functCode);
            foreach (object o in values) {
                if (o is byte)
                    data.Add((byte)o);
                //if (o is ushort)
                //    data.AddRange(ushort2ByteArray((ushort)o));
                //if (o is int)
                //    data.AddRange(int2ByteArray((int)o));
            }
            return data;
        }
        class AlexProtocolByteDispatcher: ByteDispatcher {
            readonly byte LOCK = 13;
            readonly byte KEY = 58;
            public AlexProtocolByteDispatcher(PortLevel port, bool singleByteDispatching)
                : base(port, singleByteDispatching) { }
            readonly List<byte> packetBuffer = new List<byte>();
            enum PacketingState {
                Idle,
                Wait,
            }
            PacketingState state = PacketingState.Idle;
            protected override void DispatchByte(byte data) {
                switch (state) {
                    case PacketingState.Idle: {
                            if (data == KEY) {
                                packetBuffer.Clear();
                                state = PacketingState.Wait;
                            } else {
                                //Symbol outside packet
                                OnLog(string.Format("Error({0})", data));
                            }
                            break;
                        }
                    case PacketingState.Wait: {
                            if (data == LOCK) {
                                OnPackageReceived(packetBuffer);
                                OnLog("[in]", packetBuffer);
                                packetBuffer.Clear();
                                state = PacketingState.Idle;
                            } else {
                                packetBuffer.Add(data);
                            }
                            break;
                        }
                }
            }
            ICollection<byte> buildPack(ICollection<byte> data) {
                var pack = new List<byte>(data.Count + 2);
                pack.Add(KEY);
                pack.AddRange(data);
                pack.Add(LOCK);
                return pack;
            }
            ICollection<byte> buildPackBody(ICollection<byte> data) {
                var pack = new List<byte>(data.Count);
                foreach (byte b in data) {
                    pack.Add(b);
                }
                return pack;
            }
            void OnLog(string prefix, ICollection<byte> pack) {
                var sb = new StringBuilder(prefix);
                foreach (byte b in pack) {
                    sb.Append((char)b);
                    sb.Append(GetNibble(b >> 4));
                    sb.Append(GetNibble(b));
                }
                OnLog(sb.ToString());
            }
            byte GetNibble(int data) {
                data &= 0x0F;
                if (data < 10) {
                    data += (int)'0';
                } else {
                    data += (int)'a';
                    data -= 10;
                }
                return (byte)data;
            }
            #region IByteDispatcher Members
            public override void Transmit(ICollection<byte> pack) {
                base.Transmit(buildPack(pack));
                OnLog("[out]", pack);
            }
            #endregion
        }
        delegate Predicate<int> PredicateGenerator(int value);
        static CommandDictionary<CommandCode> GetDictionary() {
            var d = new CommandDictionary<CommandCode>();
            PredicateGenerator eq = value => (l => l == value);
            PredicateGenerator more = value => (l => l > value);
            PredicateGenerator moreeq = value => (l => l >= value);
            Action<IList<byte>> trim = l => {
                l.RemoveAt(0);
                l.RemoveAt(l.Count - 1);
            };
            Action<CommandCode, Predicate<int>, CommandRecord<CommandCode>.Parser> add = (code, predicate, parser) => d[(byte)code] = new CommandRecord<CommandCode>(predicate, parser);
            // TODO: commands here
            add(CommandCode.CPU_Status, eq(4), rawCommand => new CPUStatusReply(rawCommand[1], rawCommand[2]));
            add(CommandCode.Sync_Error, eq(4), rawCommand => new SyncErrorReply(rawCommand[1], rawCommand[2]));
            add(CommandCode.LAM_Event, eq(3), rawCommand => new LAMEvent(rawCommand[1]));
            add(CommandCode.LAM_CriticalError, eq(3), rawCommand => new LAMCriticalError(rawCommand[1]));
            // TODO: check length!
            //add(CommandCode.LAM_InternalError, eq(3), rawCommand => new LAMInternalError(rawCommand[1]));
            
            return d;
        }
    }
}
