using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor.Common.Messaging.SevMorGeo {
    class ModBus: SyncAsyncCheckableProtocol<CommandCode> {
        public ModBus(PortLevel port)
            : base(new ModbusByteDispatcher(port, false)) { }
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

        public static IList<byte> collectData(byte functCode, params object[] values) {
            List<byte> data = new List<byte>();
            data.Add(functCode);
            foreach (object o in values) {
                if (o is byte)
                    data.Add((byte)o);
                if (o is ushort)
                    data.AddRange(ushort2ByteArray((ushort)o));
                if (o is int)
                    data.AddRange(int2ByteArray((int)o));
            }
            return data;
        }
        static byte[] ushort2ByteArray(ushort value) {
            if (value < 0) value = 0;
            if (value > 4095) value = 4095;
            return new byte[] { (byte)value, (byte)(value >> 8) };
        }
        static byte[] int2ByteArray(int value) {
            if (value < 0) value = 0;
            if (value > 16777215) value = 16777215;
            return new byte[] { (byte)value, (byte)(value >> 8), (byte)(value >> 16) };
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
                OnLog("[out]", buildPackBody(pack));
            }
            #endregion
        }
        protected override CommandDictionary<CommandCode> GetDictionary() {
            var d = new CommandDictionary<CommandCode>();
            Action<CommandCode, Predicate<int>, CodeAdder> add = (code, predicate, action) =>
                d[(byte)code] = new CommandRecord<CommandCode>(predicate, action((byte)code));
            
            add(CommandCode.GetState, eq(3), sync(raw => new updateState(raw[1])));
            add(CommandCode.GetStatus, eq(29), sync(raw => new updateStatus(raw[1],
                                                raw[2],
                                                (ushort)((ushort)raw[3] + ((ushort)raw[4] << 8)),
                                                (ushort)((ushort)raw[5] + ((ushort)raw[6] << 8)),
                                                (ushort)((ushort)raw[7] + ((ushort)raw[8] << 8)),
                                                (ushort)((ushort)raw[9] + ((ushort)raw[10] << 8)),
                                                (ushort)((ushort)raw[11] + ((ushort)raw[12] << 8)),
                                                (ushort)((ushort)raw[13] + ((ushort)raw[14] << 8)),
                                                (ushort)((ushort)raw[15] + ((ushort)raw[16] << 8)),
                                                (ushort)((ushort)raw[17] + ((ushort)raw[18] << 8)),
                                                (ushort)((ushort)raw[19] + ((ushort)raw[20] << 8)),
                                                (ushort)((ushort)raw[21] + ((ushort)raw[22] << 8)),
                                                (ushort)((ushort)raw[23] + ((ushort)raw[24] << 8)),
                                                raw[25],
                                                (ushort)((ushort)raw[26] + ((ushort)raw[27] << 8)))));
            add(CommandCode.Shutdown, eq(2), sync(raw => new confirmShutdown()));
            add(CommandCode.Init, eq(2), sync(raw => new confirmInit()));
            add(CommandCode.SetHeatCurrent, eq(2), sync(raw => new confirmHCurrent()));
            add(CommandCode.SetEmissionCurrent, eq(2), sync(raw => new confirmECurrent()));
            add(CommandCode.SetIonizationVoltage, eq(2), sync(raw => new confirmIVoltage()));
            add(CommandCode.SetFocusVoltage1, eq(2), sync(raw => new confirmF1Voltage()));
            add(CommandCode.SetFocusVoltage2, eq(2), sync(raw => new confirmF2Voltage()));
            add(CommandCode.SetScanVoltage, eq(2), sync(raw => new confirmSVoltage()));
            add(CommandCode.SetCapacitorVoltage, eq(2), sync(raw => new confirmCP()));
            add(CommandCode.Measure, eq(2), sync(raw => new confirmMeasure()));
            add(CommandCode.GetCounts, eq(8), sync(raw => new updateCounts((int)raw[1] + ((int)raw[2] << 8) + ((int)raw[3] << 16),
                                                (int)raw[4] + ((int)raw[5] << 8) + ((int)raw[6] << 16))));
            add(CommandCode.heatCurrentEnable, eq(2), sync(raw => new confirmHECurrent()));
            add(CommandCode.EnableHighVoltage, eq(2), sync(raw => new confirmHighVoltage()));
            add(CommandCode.GetTurboPumpStatus, eq(17), sync(raw => new updateTurboPumpStatus((ushort)((ushort)raw[1] + ((ushort)raw[2] << 8)),
                                                (ushort)((ushort)raw[3] + ((ushort)raw[4] << 8)),
                                                (ushort)((ushort)raw[5] + ((ushort)raw[6] << 8)),
                                                (ushort)((ushort)raw[7] + ((ushort)raw[8] << 8)),
                                                (ushort)((ushort)raw[9] + ((ushort)raw[10] << 8)),
                                                (ushort)((ushort)raw[11] + ((ushort)raw[12] << 8)),
                                                raw[13],
                                                raw[14],
                                                raw[15])));
            add(CommandCode.SetForvacuumLevel, eq(2), sync(raw => new confirmForvacuumLevel()));

            add(CommandCode.InvalidCommand, moreeq(3), syncerr(raw => new logInvalidCommand(trim(raw))));
            add(CommandCode.InvalidChecksum, eq(2), syncerr(raw => new logInvalidChecksum()));
            add(CommandCode.InvalidPacket, eq(2), syncerr(raw => new logInvalidPacket()));
            add(CommandCode.InvalidLength, eq(2), syncerr(raw => new logInvalidLength()));
            add(CommandCode.InvalidData, eq(2), syncerr(raw => new logInvalidData()));
            add(CommandCode.InvalidState, eq(2), syncerr(raw => new logInvalidState()));
            
            add(CommandCode.InternalError, eq(3), asyncerr(raw => new logInternalError(raw[1])));
            add(CommandCode.InvalidSystemState, eq(2), asyncerr(raw => new logInvalidSystemState()));
            add(CommandCode.VacuumCrash, eq(3), asyncerr(raw => new logVacuumCrash(raw[1])));
            // see GetTurboPumpStatus!
            add(CommandCode.TurboPumpFailure, eq(17), asyncerr(raw => new logTurboPumpFailure(raw)));
            add(CommandCode.PowerFail, eq(2), asyncerr(raw => new logPowerFail()));
            add(CommandCode.InvalidVacuumState, eq(2), asyncerr(raw => new logInvalidVacuumState()));
            add(CommandCode.AdcPlaceIonSrc, moreeq(2), asyncerr(raw => new logAdcPlaceIonSrc(trim(raw))));
            add(CommandCode.AdcPlaceScanv, moreeq(2), asyncerr(raw => new logAdcPlaceScanv(trim(raw))));
            add(CommandCode.AdcPlaceControlm, moreeq(2), asyncerr(raw => new logAdcPlaceControlm(trim(raw))));
            
            add(CommandCode.Measured, eq(2), async(raw => new requestCounts()));
            add(CommandCode.VacuumReady, eq(2), async(raw => new confirmVacuumReady()));
            add(CommandCode.SystemShutdowned, eq(2), async(raw => new confirmShutdowned()));
            add(CommandCode.SystemReseted, eq(2), async(raw => new SystemReseted()));
            add(CommandCode.HighVoltageOff, eq(2), async(raw => new confirmHighVoltageOff()));
            add(CommandCode.HighVoltageOn, eq(2), async(raw => new confirmHighVoltageOn()));
            return d;
        }
    }
}
