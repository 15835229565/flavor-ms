using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor.Common.Messaging.SevMorGeo {
    class ModBus: CheckableProtocol<CommandCode> {
        public ModBus(PortLevel port)
            : base(new ModbusByteDispatcher(port, false), GetDictionary()) { }
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

        public static byte[] collectData(params object[] values) {
            List<byte> data = new List<byte>();
            foreach (object o in values) {
                if (o is byte)
                    data.Add((byte)o);
                if (o is ushort)
                    data.AddRange(ushort2ByteArray((ushort)o));
                if (o is int)
                    data.AddRange(int2ByteArray((int)o));
            }
            return data.ToArray();
        }
        public static byte[] collectData(byte functCode) {
            return new byte[] { functCode };
        }
        public static byte[] collectData(byte functCode, byte value) {
            return new byte[] { functCode, value };
        }
        public static byte[] collectData(byte functCode, ushort value) {
            byte[] data = ushort2ByteArray(value);
            return new byte[] { functCode, data[0], data[1] };
        }
        public static byte[] collectData(byte functCode, ushort value1, ushort value2) {
            byte[] data1 = ushort2ByteArray(value1);
            byte[] data2 = ushort2ByteArray(value2);
            return new byte[] { functCode, data1[0], data1[1], data2[0], data2[1] };
        }
        public static byte[] collectData(byte functCode, int value1, int value2) {
            List<byte> data = new List<byte>();
            data.Add(functCode);
            data.AddRange(int2ByteArray(value1));
            data.AddRange(int2ByteArray(value2));
            return data.ToArray();
        }
        static byte[] ushort2ByteArray(ushort value) {
            if (value < 0) value = 0;
            if (value > 4095) value = 4095;
            return new byte[] { (byte)(value), (byte)(value >> 8) };
        }
        static byte[] int2ByteArray(int value) {
            if (value < 0) value = 0;
            if (value > 16777215) value = 16777215;
            return new byte[] { (byte)(value), (byte)(value >> 8), (byte)(value >> 16) };
        }
        class ModbusByteDispatcher: ByteDispatcher {
            readonly byte START = (byte)':';
            readonly byte END = 0x0d;
            public ModbusByteDispatcher(PortLevel port, bool singleByteDispatching)
                : base(port, singleByteDispatching) { }
            readonly List<byte> packetBuffer = new List<byte>();
            enum PacketingState {
                Idle,
                WaitUpper,
                WaitLower
            }
            PacketingState state = PacketingState.Idle;
            byte upperNibble;
            readonly List<byte> byteBuffer = new List<byte>();
            protected override void DispatchByte(byte data) {
                switch (state) {
                    case PacketingState.Idle: {
                            if (data == START) {
                                packetBuffer.Clear();
                                byteBuffer.Clear();
                                state = PacketingState.WaitUpper;
                            } else {
                                //Symbol outside packet
                                OnLog(string.Format("Error({0})", data));
                            }
                            break;
                        }
                    case PacketingState.WaitUpper: {
                            if (data == END) {
                                OnPackageReceived(packetBuffer);
                                OnLog("[in]", byteBuffer);
                                
                                packetBuffer.Clear();
                                byteBuffer.Clear();
                                state = PacketingState.Idle;
                            } else {
                                upperNibble = GetInt(data);
                                byteBuffer.Add(data);
                                state = PacketingState.WaitLower;
                            }
                            break;
                        }
                    case PacketingState.WaitLower: {
                            byte lowerNibble = GetInt(data);
                            lowerNibble |= (byte)(upperNibble << 4);
                            packetBuffer.Add(lowerNibble);
                            byteBuffer.Add(data);
                            state = PacketingState.WaitUpper;
                            break;
                        }
                }
            }
            ICollection<byte> buildPack(ICollection<byte> data) {
                var pack = new List<byte>(2 * data.Count + 2);
                pack.Add(START);
                pack.AddRange(buildPackBody(data));
                pack.Add(END);
                return pack;
            }
            ICollection<byte> buildPackBody(ICollection<byte> data) {
                var pack = new List<byte>(2 * data.Count);
                foreach (byte b in data) {
                    pack.Add(GetNibble(b >> 4));
                    pack.Add(GetNibble(b));
                }
                return pack;
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
            byte GetInt(int data) {
                if (data >= (byte)'0' && data <= (int)'9') {
                    return (byte)(data - (int)'0');
                }
                if (data >= (byte)'a' && data <= (int)'f') {
                    return (byte)(data - (int)'a' + 10);
                }
                if (data >= (byte)'A' && data <= (int)'F') {
                    return (byte)(data - (int)'A' + 10);
                }
                return 0;
            }

            void OnLog(string prefix, ICollection<byte> pack) {
                var sb = new StringBuilder(prefix);
                foreach (byte b in pack) {
                    sb.Append((char)b);
                }
                OnLog(sb.ToString());
            }
            #region IByteDispatcher Members
            public override void Transmit(ICollection<byte> pack) {
                var message = buildPack(pack);
                base.Transmit(message);
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
            add(CommandCode.GetState, eq(3), rawCommand => new updateState(rawCommand[1]));
            add(CommandCode.GetStatus, eq(29), rawCommand => new updateStatus(rawCommand[1],
                                                rawCommand[2],
                                                (ushort)((ushort)rawCommand[3] + ((ushort)rawCommand[4] << 8)),
                                                (ushort)((ushort)rawCommand[5] + ((ushort)rawCommand[6] << 8)),
                                                (ushort)((ushort)rawCommand[7] + ((ushort)rawCommand[8] << 8)),
                                                (ushort)((ushort)rawCommand[9] + ((ushort)rawCommand[10] << 8)),
                                                (ushort)((ushort)rawCommand[11] + ((ushort)rawCommand[12] << 8)),
                                                (ushort)((ushort)rawCommand[13] + ((ushort)rawCommand[14] << 8)),
                                                (ushort)((ushort)rawCommand[15] + ((ushort)rawCommand[16] << 8)),
                                                (ushort)((ushort)rawCommand[17] + ((ushort)rawCommand[18] << 8)),
                                                (ushort)((ushort)rawCommand[19] + ((ushort)rawCommand[20] << 8)),
                                                (ushort)((ushort)rawCommand[21] + ((ushort)rawCommand[22] << 8)),
                                                (ushort)((ushort)rawCommand[23] + ((ushort)rawCommand[24] << 8)),
                                                rawCommand[25],
                                                (ushort)((ushort)rawCommand[26] + ((ushort)rawCommand[27] << 8))));
            add(CommandCode.Shutdown, eq(2), rawCommand => new confirmShutdown());
            add(CommandCode.Init, eq(2), rawCommand => new confirmInit());
            add(CommandCode.SetHeatCurrent, eq(2), rawCommand => new confirmHCurrent());
            add(CommandCode.SetEmissionCurrent, eq(2), rawCommand => new confirmECurrent());
            add(CommandCode.SetIonizationVoltage, eq(2), rawCommand => new confirmIVoltage());
            add(CommandCode.SetFocusVoltage1, eq(2), rawCommand => new confirmF1Voltage());
            add(CommandCode.SetFocusVoltage2, eq(2), rawCommand => new confirmF2Voltage());
            add(CommandCode.SetScanVoltage, eq(2), rawCommand => new confirmSVoltage());
            add(CommandCode.SetCapacitorVoltage, eq(2), rawCommand => new confirmCP());
            add(CommandCode.Measure, eq(2), rawCommand => new confirmMeasure());
            add(CommandCode.GetCounts, eq(8), rawCommand => new updateCounts((int)rawCommand[1] + ((int)rawCommand[2] << 8) + ((int)rawCommand[3] << 16),
                                                (int)rawCommand[4] + ((int)rawCommand[5] << 8) + ((int)rawCommand[6] << 16)));
            add(CommandCode.heatCurrentEnable, eq(2), rawCommand => new confirmHECurrent());
            add(CommandCode.EnableHighVoltage, eq(2), rawCommand => new confirmHighVoltage());
            add(CommandCode.GetTurboPumpStatus, eq(17), rawCommand => new updateTurboPumpStatus((ushort)((ushort)rawCommand[1] + ((ushort)rawCommand[2] << 8)),
                                                (ushort)((ushort)rawCommand[3] + ((ushort)rawCommand[4] << 8)),
                                                (ushort)((ushort)rawCommand[5] + ((ushort)rawCommand[6] << 8)),
                                                (ushort)((ushort)rawCommand[7] + ((ushort)rawCommand[8] << 8)),
                                                (ushort)((ushort)rawCommand[9] + ((ushort)rawCommand[10] << 8)),
                                                (ushort)((ushort)rawCommand[11] + ((ushort)rawCommand[12] << 8)),
                                                rawCommand[13],
                                                rawCommand[14],
                                                rawCommand[15]));
            add(CommandCode.SetForvacuumLevel, eq(2), rawCommand => new confirmForvacuumLevel());
            add(CommandCode.InvalidCommand, more(2), rawCommand => {
                trim(rawCommand);
                return new logInvalidCommand(rawCommand);
            });
            add(CommandCode.InvalidChecksum, eq(2), rawCommand => new logInvalidChecksum());
            add(CommandCode.InvalidPacket, eq(2), rawCommand => new logInvalidPacket());
            add(CommandCode.InvalidLength, eq(2), rawCommand => new logInvalidLength());
            add(CommandCode.InvalidData, eq(2), rawCommand => new logInvalidData());
            add(CommandCode.InvalidState, eq(2), rawCommand => new logInvalidState());
            add(CommandCode.InternalError, eq(3), rawCommand => new logInternalError(rawCommand[1]));
            add(CommandCode.InvalidSystemState, eq(2), rawCommand => new logInvalidSystemState());
            add(CommandCode.VacuumCrash, eq(3), rawCommand => new logVacuumCrash(rawCommand[1]));
            // see GetTurboPumpStatus!
            add(CommandCode.TurboPumpFailure, eq(17), rawCommand => new logTurboPumpFailure(rawCommand));
            add(CommandCode.PowerFail, eq(2), rawCommand => new logPowerFail());
            add(CommandCode.InvalidVacuumState, eq(2), rawCommand => new logInvalidVacuumState());
            add(CommandCode.AdcPlaceIonSrc, moreeq(2), rawCommand => {
                trim(rawCommand);
                return new logAdcPlaceIonSrc(rawCommand);
            });
            add(CommandCode.AdcPlaceScanv, moreeq(2), rawCommand => {
                trim(rawCommand);
                return new logAdcPlaceScanv(rawCommand);
            });
            add(CommandCode.AdcPlaceControlm, moreeq(2), rawCommand => {
                trim(rawCommand);
                return new logAdcPlaceControlm(rawCommand);
            });
            add(CommandCode.Measured, eq(2), rawCommand => new requestCounts());
            add(CommandCode.VacuumReady, eq(2), rawCommand => new confirmVacuumReady());
            add(CommandCode.SystemShutdowned, eq(2), rawCommand => new confirmShutdowned());
            add(CommandCode.SystemReseted, eq(2), rawCommand => new SystemReseted());
            add(CommandCode.HighVoltageOff, eq(2), rawCommand => new confirmHighVoltageOff());
            add(CommandCode.HighVoltageOn, eq(2), rawCommand => new confirmHighVoltageOn());
            return d;
        }
    }
}
