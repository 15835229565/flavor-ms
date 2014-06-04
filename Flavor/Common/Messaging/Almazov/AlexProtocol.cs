using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Flavor.Common.Messaging.Almazov.Commands;

namespace Flavor.Common.Messaging.Almazov {
    class AlexProtocol: SyncAsyncCheckableProtocol<CommandCode> {
        public AlexProtocol(PortLevel port)
            : base(new AlexProtocolByteDispatcher(port, false)) { }

        protected override byte ComputeCS(IList<byte> data) {
            byte checkSum = 0;
            foreach (byte b in data)
                checkSum -= b;
            return checkSum;
        }
        protected override IList<byte> BuildPackBody(IList<byte> data, byte checksum) {
            var pack = new List<byte>(data);
            pack.Add(checksum);
            return pack;
        }

        public static IList<byte> collectData(CommandCode functCode, params object[] values) {
            List<byte> data = new List<byte>();
            data.Add((byte)functCode);
            foreach (object o in values) {
                if (o is byte)
                    data.Add((byte)o);
                if (o is string) {
                    data.AddRange(Encoding.ASCII.GetBytes(o as string));
                }
                //if (o is ushort)
                //    data.AddRange(ushort2ByteArray((ushort)o));
                //if (o is int)
                //    data.AddRange(int2ByteArray((int)o));
            }
            return data;
        }
        class AlexProtocolByteDispatcher: ByteDispatcher {
            const byte KEY = 250, LOCK = 251, SERVICE = 252;
            public AlexProtocolByteDispatcher(PortLevel port, bool singleByteDispatching)
                : base(port, singleByteDispatching) { }
            readonly List<byte> packetBuffer = new List<byte>();
            enum PacketingState {
                Idle,
                Wait,
                WaitLiteral,
            }
            PacketingState state = PacketingState.Idle;
            protected override void DispatchByte(byte data) {
                switch (state) {
                    case PacketingState.Idle:
                        if (data == KEY) {
                            packetBuffer.Clear();
                            state = PacketingState.Wait;
                        } else {
                            //Symbol outside packet
                            OnLog(string.Format("Symbol outside packet: <{0}>", data));
                        }
                        break;
                    case PacketingState.WaitLiteral:
                        switch (data) {
                            case 0:
                            case 1:
                            case 2:
                                data += 250;    
                                packetBuffer.Add(data);
                                break;
                            default:
                                //Wrong literal
                                OnLog(string.Format("Wrong literal: <{0}>", data));
                                break;
                        }
                        state = PacketingState.Wait;
                        break;
                    case PacketingState.Wait:
                        switch (data) {
                            case LOCK:
                                OnPackageReceived(packetBuffer);
                                OnLog("[in]", packetBuffer);
                                packetBuffer.Clear();
                                state = PacketingState.Idle;
                                break;
                            case SERVICE:
                                state = PacketingState.WaitLiteral;
                                break;
                            case KEY:
                                //Key inside packet
                                OnLog("Key inside packet");
                                break;
                            default:
                                packetBuffer.Add(data);
                                break;
                        }
                        break;
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
                    sb.Append((char)GetNibble(b >> 4));
                    sb.Append((char)GetNibble(b));
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
        protected override CommandDictionary<CommandCode> GetDictionary() {
            var d = new CommandDictionary<CommandCode>();
            
            Action<CommandCode, Predicate<int>, CodeAdder> add = (code, predicate, action) =>
                d[(byte)code] = new CommandRecord<CommandCode>(predicate, action((byte)code));
            // TODO: commands here
            add(CommandCode.CPU_Status, eq(4), sync(raw => new CPUStatusReply(raw[1], raw[2])));
            // strangely 4 bytes
            add(CommandCode.HVE, eq(4), sync(raw => new HighVoltagePermittedStatusReply(raw[2] == 0 ? true : false)));
            add(CommandCode.PRGE, eq(3), sync(raw => OperationBlockReply.Parse(raw[1])));
            add(CommandCode.TIC_Retransmit, moreeq(28), sync(raw => {
                Regex expression = new Regex(@"^=V902 ([0-7]);[0-7];[0-9]+;[0-9]+;[0-9]+;([0-4]);([0-4]);([0-4]);([0-9]+);[0-9]+\r$");
                Match match;
                var command = Encoding.ASCII.GetString(trim(raw).ToArray());
                match = expression.Match(command);
                if (match.Success) {
                    GroupCollection groups = match.Groups;
                    var turbo = groups[1].Value == "4";
                    var relay1 = groups[2].Value == "4";
                    var relay2 = groups[3].Value == "4";
                    var relay3 = groups[4].Value == "4";
                    int alert;
                    try {
                        alert = int.Parse(groups[5].Value);
                    } catch (FormatException) {
                        //error. wrong alert format.
                        alert = 0;
                    }
                    return new TICStatusReply(turbo, relay1, relay2, relay3, alert);
                } else {
                    OnErrorCommand(raw, "Wrong TIC status");
                    return null;
                }
            }));
            add(CommandCode.SEMV1, eq(3), sync(raw => {
                // TODO: move into command ctor
                bool? res;
                switch (raw[1]) {
                    case 0:
                        res = false;
                        break;
                    case 1:
                        res = true;
                        break;
                    default:
                        res = null;
                        break;
                }
                return new Valve1Reply(res);
            }));

            add(CommandCode.SPI_PSIS_SetVoltage, eq(2), sync(raw => new IonSourceSetReply()));
            add(CommandCode.SPI_DPS_SetVoltage, eq(2), sync(raw => new DetectorSetReply()));
            add(CommandCode.SPI_PSInl_SetVoltage, eq(2), sync(raw => new InletSetReply()));
            add(CommandCode.SPI_PSIS_GetVoltage, eq(4), sync(raw => 
                IonSourceGetReply.Parse(trim(raw))));
            add(CommandCode.SPI_DPS_GetVoltage, eq(4), sync(raw => DetectorGetReply.Parse(trim(raw))));
            add(CommandCode.SPI_PSInl_GetVoltage, eq(4), sync(raw => InletGetReply.Parse(trim(raw))));
            
            add(CommandCode.RTC_ReceiveResults, eq(18), sync(raw => new CountsReply(trim(raw))));

            // BAD temporary solution
            ActionGenerator<ServicePacket<CommandCode>> service = gen => (code => (list => {
                switch (list[1]) {
                    case 1:
                    case 10:
                        OnSyncErrorReceived(code, gen(list) as SyncError<CommandCode>);
                        break;
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                        OnAsyncCommandReceived(code, gen(list) as Async<CommandCode>);
                        break;
                    case 30:
                    case 31:
                        OnAsyncErrorReceived(code, gen(list) as AsyncError<CommandCode>);
                        break;
                    case 41:
                    case 42:
                        OnAsyncErrorReceived(code, gen(list) as AsyncError<CommandCode>);
                        break;
                    default:
                        break;
                }
            }));
            add(CommandCode.Service_Message, eq(3), service(raw => {
                byte code = raw[1];
                switch (code) {
                    case 1:
                    case 10:
                        return new SyncErrorReply(code);
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                        return LAMEvent.Parse(code);
                    case 30:
                    case 31:
                        return new LAMCriticalError(code);
                    case 41:
                    case 42:
                        return new LAMInternalError(code);
                    default:
                        return null;
                }
            }));
            //add(CommandCode.Sync_Error, eq(4), syncerr(raw => new SyncErrorReply(raw[1])));
            
            //add(CommandCode.LAM_Event, eq(3), async(raw => new LAMEvent(raw[1])));
            
            //add(CommandCode.LAM_CriticalError, eq(3), asyncerr(raw => new LAMCriticalError(raw[1])));
            // TODO: check length!
            //add(CommandCode.LAM_InternalError, eq(3), asyncerr(raw => new LAMInternalError(raw[1])));
            
            return d;
        }
    }
}
