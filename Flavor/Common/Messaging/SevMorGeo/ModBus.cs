using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor.Common.Messaging.SevMorGeo {
    internal class ModBus: IProtocol<CommandCode> {
        #region IProtocol Members
        public event EventHandler<CommandReceivedEventArgs<CommandCode>> CommandReceived;
        protected void OnCommandReceived(ServicePacket<CommandCode> command) {
            if (CommandReceived != null)
                CommandReceived(this, new CommandReceivedEventArgs<CommandCode>(command));
        }
        public event EventHandler<ErrorCommandEventArgs> ErrorCommand;
        protected void OnErrorCommand(byte[] data, string message) {
            if (ErrorCommand != null)
                ErrorCommand(this, new ErrorCommandEventArgs(data, message));
        }
        public void Send(byte[] message) {
            byteDispatcher.Transmit(buildPackBody(message, ComputeChecksum(message)));
        }
        #endregion
        //TODO: structure code-length
        private readonly IByteDispatcher byteDispatcher;
        public ModBus(PortLevel port) {
            byteDispatcher = new ModbusByteDispatcher(port, false);
            byteDispatcher.PackageReceived += Parse;
            byteDispatcher.Log += OnLog;
        }

        private static byte ComputeChecksum(byte[] data) {
            byte checkSum = 0;
            for (int i = 0; i < data.Length; i++) {
                checkSum -= data[i];
            }
            return checkSum;
        }
        private static bool checkCS(byte[] data) {
            return true ^ Convert.ToBoolean(ComputeChecksum(data));
        }
        private void Parse(object sender, ByteArrayEventArgs e) {
            var raw_command = e.Data;
            int minLength = 2;
            if (raw_command.Length < minLength) {
                OnErrorCommand(raw_command, "Короткий пакет");
                return;
            }
            if (!checkCS(raw_command)) {
                OnErrorCommand(raw_command, "Неверная контрольная сумма");
                return;
            }
            CommandCode commandcode = (CommandCode)raw_command[0];
            ServicePacket<CommandCode> packet = null;
            switch (commandcode) {
                case CommandCode.GetState:
                    if (raw_command.Length == 3)
                        packet = new updateState(raw_command[1]);
                    break;
                case CommandCode.GetStatus:
                    if (raw_command.Length == 29)
                        packet = new updateStatus(raw_command[1],
                                                raw_command[2],
                                                (ushort)((ushort)raw_command[3] + ((ushort)raw_command[4] << 8)),
                                                (ushort)((ushort)raw_command[5] + ((ushort)raw_command[6] << 8)),
                                                (ushort)((ushort)raw_command[7] + ((ushort)raw_command[8] << 8)),
                                                (ushort)((ushort)raw_command[9] + ((ushort)raw_command[10] << 8)),
                                                (ushort)((ushort)raw_command[11] + ((ushort)raw_command[12] << 8)),
                                                (ushort)((ushort)raw_command[13] + ((ushort)raw_command[14] << 8)),
                                                (ushort)((ushort)raw_command[15] + ((ushort)raw_command[16] << 8)),
                                                (ushort)((ushort)raw_command[17] + ((ushort)raw_command[18] << 8)),
                                                (ushort)((ushort)raw_command[19] + ((ushort)raw_command[20] << 8)),
                                                (ushort)((ushort)raw_command[21] + ((ushort)raw_command[22] << 8)),
                                                (ushort)((ushort)raw_command[23] + ((ushort)raw_command[24] << 8)),
                                                raw_command[25],
                                                (ushort)((ushort)raw_command[26] + ((ushort)raw_command[27] << 8)));
                    break;
                case CommandCode.Shutdown:
                    if (raw_command.Length == 2)
                        packet = new confirmShutdown();
                    break;
                case CommandCode.Init:
                    if (raw_command.Length == 2)
                        packet = new confirmInit();
                    break;
                case CommandCode.SetHeatCurrent:
                    if (raw_command.Length == 2)
                        packet = new confirmHCurrent();
                    break;
                case CommandCode.SetEmissionCurrent:
                    if (raw_command.Length == 2)
                        packet = new confirmECurrent();
                    break;
                case CommandCode.SetIonizationVoltage:
                    if (raw_command.Length == 2)
                        packet = new confirmIVoltage();
                    break;
                case CommandCode.SetFocusVoltage1:
                    if (raw_command.Length == 2)
                        packet = new confirmF1Voltage();
                    break;
                case CommandCode.SetFocusVoltage2:
                    if (raw_command.Length == 2)
                        packet = new confirmF2Voltage();
                    break;
                case CommandCode.SetScanVoltage:
                    if (raw_command.Length == 2)
                        packet = new confirmSVoltage();
                    break;
                case CommandCode.SetCapacitorVoltage:
                    if (raw_command.Length == 2)
                        packet = new confirmCP();
                    break;
                case CommandCode.Measure:
                    if (raw_command.Length == 2)
                        packet = new confirmMeasure();
                    break;
                case CommandCode.GetCounts:
                    if (raw_command.Length == 8)
                        packet = new updateCounts((int)raw_command[1] + ((int)raw_command[2] << 8) + ((int)raw_command[3] << 16),
                                                (int)raw_command[4] + ((int)raw_command[5] << 8) + ((int)raw_command[6] << 16));
                    break;
                case CommandCode.heatCurrentEnable:
                    if (raw_command.Length == 2)
                        packet = new confirmHECurrent();
                    break;
                case CommandCode.EnableHighVoltage:
                    if (raw_command.Length == 2)
                        packet = new confirmHighVoltage();
                    break;
                case CommandCode.GetTurboPumpStatus:
                    if (raw_command.Length == 17)
                        packet = new updateTurboPumpStatus((ushort)((ushort)raw_command[1] + ((ushort)raw_command[2] << 8)),
                                                (ushort)((ushort)raw_command[3] + ((ushort)raw_command[4] << 8)),
                                                (ushort)((ushort)raw_command[5] + ((ushort)raw_command[6] << 8)),
                                                (ushort)((ushort)raw_command[7] + ((ushort)raw_command[8] << 8)),
                                                (ushort)((ushort)raw_command[9] + ((ushort)raw_command[10] << 8)),
                                                (ushort)((ushort)raw_command[11] + ((ushort)raw_command[12] << 8)),
                                                raw_command[13],
                                                raw_command[14],
                                                raw_command[15]);
                    break;
                case CommandCode.SetForvacuumLevel:
                    if (raw_command.Length == 2)
                        packet = new confirmForvacuumLevel();
                    break;
                case CommandCode.InvalidCommand:
                    if (raw_command.Length > 2) {
                        //TODO: do not copy CS (last byte)!
                        byte[] tempArray = new byte[raw_command.Length - 1];
                        raw_command.CopyTo(tempArray, 1);
                        packet = new logInvalidCommand(tempArray);
                    }
                    break;
                case CommandCode.InvalidChecksum:
                    if (raw_command.Length == 2)
                        packet = new logInvalidChecksum();
                    break;
                case CommandCode.InvalidPacket:
                    if (raw_command.Length == 2)
                        packet = new logInvalidPacket();
                    break;
                case CommandCode.InvalidLength:
                    if (raw_command.Length == 2)
                        packet = new logInvalidLength();
                    break;
                case CommandCode.InvalidData:
                    if (raw_command.Length == 2)
                        packet = new logInvalidData();
                    break;
                case CommandCode.InvalidState:
                    if (raw_command.Length == 2)
                        packet = new logInvalidState();
                    break;
                case CommandCode.InternalError:
                    if (raw_command.Length == 3)
                        packet = new logInternalError(raw_command[1]);
                    break;
                case CommandCode.InvalidSystemState:
                    if (raw_command.Length == 2)
                        packet = new logInvalidSystemState();
                    break;
                case CommandCode.VacuumCrash:
                    if (raw_command.Length == 3)
                        packet = new logVacuumCrash(raw_command[1]);
                    break;
                case CommandCode.TurboPumpFailure:
                    if (raw_command.Length == 17)
                        packet = new logTurboPumpFailure(raw_command);
                    break;
                case CommandCode.PowerFail:
                    if (raw_command.Length == 2)
                        packet = new logPowerFail();
                    break;
                case CommandCode.InvalidVacuumState:
                    if (raw_command.Length == 2)
                        packet = new logInvalidVacuumState();
                    break;
                case CommandCode.AdcPlaceIonSrc:
                    //!!!
                    if (raw_command.Length >= 2)
                        packet = new logAdcPlaceIonSrc(raw_command);
                    break;
                case CommandCode.AdcPlaceScanv:
                    //!!!
                    if (raw_command.Length >= 2)
                        packet = new logAdcPlaceScanv(raw_command);
                    break;
                case CommandCode.AdcPlaceControlm:
                    //!!!
                    if (raw_command.Length >= 2)
                        packet = new logAdcPlaceControlm(raw_command);
                    break;
                case CommandCode.Measured:
                    if (raw_command.Length == 2)
                        packet = new requestCounts();
                    break;
                case CommandCode.VacuumReady:
                    if (raw_command.Length == 2)
                        packet = new confirmVacuumReady();
                    break;
                case CommandCode.SystemShutdowned:
                    if (raw_command.Length == 2)
                        packet = new confirmShutdowned();
                    break;
                case CommandCode.SystemReseted:
                    if (raw_command.Length == 2)
                        packet = new SystemReseted();
                    break;
                case CommandCode.HighVoltageOff:
                    if (raw_command.Length == 2)
                        packet = new confirmHighVoltageOff();
                    break;
                case CommandCode.HighVoltageOn:
                    if (raw_command.Length == 2)
                        packet = new confirmHighVoltageOn();
                    break;
                default:
                    OnErrorCommand(raw_command, "Неверная команда");
                    return;
            }
            if (packet == null) {
                OnErrorCommand(raw_command, "Неверная длина");
                return;
            }
            OnCommandReceived(packet);
        }

        internal static byte[] collectData(params object[] values) {
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
        internal static byte[] collectData(byte functCode) {
            return new byte[] { functCode };
        }
        internal static byte[] collectData(byte functCode, byte value) {
            return new byte[] { functCode, value };
        }
        internal static byte[] collectData(byte functCode, ushort value) {
            byte[] data = ushort2ByteArray(value);
            return new byte[] { functCode, data[0], data[1] };
        }
        internal static byte[] collectData(byte functCode, ushort value1, ushort value2) {
            byte[] data1 = ushort2ByteArray(value1);
            byte[] data2 = ushort2ByteArray(value2);
            return new byte[] { functCode, data1[0], data1[1], data2[0], data2[1] };
        }
        internal static byte[] collectData(byte functCode, int value1, int value2) {
            List<byte> data = new List<byte>();
            data.Add(functCode);
            data.AddRange(int2ByteArray(value1));
            data.AddRange(int2ByteArray(value2));
            return data.ToArray();
        }
        private static byte[] ushort2ByteArray(ushort value) {
            if (value < 0) value = 0;
            if (value > 4095) value = 4095;
            return new byte[] { (byte)(value), (byte)(value >> 8) };
        }
        private static byte[] int2ByteArray(int value) {
            if (value < 0) value = 0;
            if (value > 16777215) value = 16777215;
            return new byte[] { (byte)(value), (byte)(value >> 8), (byte)(value >> 16) };
        }
        private static ICollection<byte> buildPackBody(byte[] data, byte checksum) {
            var pack = new List<byte>(data);
            pack.Add(checksum);
            return pack;
        }

        #region IDisposable Members
        public void Dispose() {
            byteDispatcher.Dispose();
        }
        #endregion
        #region ILog Members
        public event MessageHandler Log;
        protected virtual void OnLog(string message) {
            if (Log != null)
                Log(message);
        }
        #endregion
        private class ModbusByteDispatcher: ByteDispatcher {
            public ModbusByteDispatcher(PortLevel port, bool singleByteDispatching)
                : base(port, singleByteDispatching) { }
            private readonly List<byte> PacketBuffer = new List<byte>();
            private enum PacketingState {
                Idle,
                WaitUpper,
                WaitLower
            }
            private PacketingState PackState = PacketingState.Idle;
            private byte UpperNibble;
            protected override void DispatchByte(byte data) {
                switch (PackState) {
                    case PacketingState.Idle: {
                            if (data == (byte)':') {
                                PacketBuffer.Clear();
                                PackState = PacketingState.WaitUpper;
                            } else {
                                //Symbol outside packet
                                OnLog(string.Format("Error({0})", data));
                            }
                            break;
                        }
                    case PacketingState.WaitUpper: {
                            if (data == 0x0d) {
                                OnPackageReceived(PacketBuffer.ToArray());
                                // BAD!
                                var sb = new StringBuilder("[in]");
                                foreach (byte b in buildPackBody(PacketBuffer.ToArray())) {
                                    sb.Append((char)b);
                                }
                                OnLog(sb.ToString());
                                
                                PacketBuffer.Clear();
                                PackState = PacketingState.Idle;
                            } else {
                                UpperNibble = GetInt(data);
                                PackState = PacketingState.WaitLower;
                            }
                            break;
                        }
                    case PacketingState.WaitLower: {
                            byte LowerNibble = GetInt(data);
                            LowerNibble |= (byte)(UpperNibble << 4);
                            PacketBuffer.Add(LowerNibble);
                            PackState = PacketingState.WaitUpper;
                            break;
                        }
                }
            }
            private static ICollection<byte> buildPack(ICollection<byte> data) {
                var pack = new List<byte>(2 * data.Count + 2);
                pack.Add((byte)':');
                pack.AddRange(buildPackBody(data));
                pack.Add((byte)'\r');
                return pack;
            }
            private static ICollection<byte> buildPackBody(ICollection<byte> data) {
                var pack = new List<byte>(2 * data.Count);
                foreach (byte b in data) {
                    pack.Add(GetNibble(b >> 4));
                    pack.Add(GetNibble(b));
                }
                return pack;
            }
            private static byte GetNibble(int data) {
                data &= 0x0F;
                if (data < 10) {
                    return (byte)(data + (int)'0');
                } else {
                    return (byte)(data + (int)'a' - 10);
                }
            }
            private static byte GetInt(int data) {
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

            #region IByteDispatcher Members
            public override void Transmit(ICollection<byte> pack) {
                var message = buildPack(pack);
                base.Transmit(message);
                //var sb = new StringBuilder("[out]");
                //foreach (byte b in message) {
                //    sb.Append((char)b);
                //}
                //OnLog(sb.ToString());
            }
            #endregion
        }
    }
}
