using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Timers;
using Flavor.Common.Commands;
using Flavor.Common;

namespace Flavor.Xmega32A4U_testBoard {
    internal class NewProtocol {
        public class CommandReceivedEventArgs: EventArgs {
            private readonly CommandCode code;
            private readonly ServicePacket command;
            public CommandCode Code {
                get { return code; }
            }
            public ServicePacket Command {
                get { return command; }
            }
            public CommandReceivedEventArgs(CommandCode code, ServicePacket command) {
                this.code = code;
                this.command = command;
            }
        }
        public delegate void CommandReceivedDelegate(object sender, CommandReceivedEventArgs e);
        public event CommandReceivedDelegate CommandReceived;
        protected void OnCommandReceived(CommandCode code, ServicePacket command) {
            if (CommandReceived != null)
                CommandReceived(this, new CommandReceivedEventArgs(code, command));
        }
        //HI+
        //TODO: structure code-length
        internal enum CommandCode: byte {
            None = 0x00,// & min length
            //sync
            GetState = 0x01,
            GetStatus = 0x02,
            Shutdown = 0x03,
            Init = 0x04,
            SetHeatCurrent = 0x05,
            SetEmissionCurrent = 0x06,
            SetIonizationVoltage = 0x07,
            SetFocusVoltage1 = 0x08,
            SetFocusVoltage2 = 0x09,
            SetScanVoltage = 0x0A,
            SetCapacitorVoltage = 0x0B,
            Measure = 0x0C,
            GetCounts = 0x0D,
            //heatCurrentEnable = 0x0E,
            //emissionCurrentEnable = 0x0F,
            heatCurrentEnable = 0x11,
            EnableHighVoltage = 0x14,
            GetTurboPumpStatus = 0x15,
            SetForvacuumLevel = 0x16,
            //syncerr
            InvalidCommand = 0x40,
            InvalidChecksum = 0x80,
            InvalidPacket = 0x81,
            InvalidLength = 0x82,
            InvalidData = 0x83,
            InvalidState = 0x84,
            //asyncerr
            InternalError = 0xC0,
            InvalidSystemState = 0xC1,
            VacuumCrash = 0xC2,//+ еще что-то..
            TurboPumpFailure = 0xC3,
            PowerFail = 0xC4,
            InvalidVacuumState = 0xC5,
            AdcPlaceIonSrc = 0xC8,
            AdcPlaceScanv = 0xC9,
            AdcPlaceControlm = 0xCA,
            //async
            Measured = 0xE0,
            VacuumReady = 0xE1,
            SystemShutdowned = 0xE2,
            SystemReseted = 0xE3,//+ еще что-то..
            HighVoltageOff = 0xE5,
            HighVoltageOn = 0xE6
        }
        //HI
        private enum PacketingState {
            Idle,
            WaitUpper,
            WaitLower
        }
        //LOW
        internal enum PortStates {
            Closed,
            Opened,
            Closing,
            Opening,
            ErrorClosing,
            ErrorOpening
        }
        //HI
        private PacketingState PackState = PacketingState.Idle;
        //HI+
        private List<byte> PacketBuffer = new List<byte>();
        //HI
        private byte UpperNibble;
        //LOW
        private SerialPort _serialPort = null;
        //HI+
        private List<byte[]> PacketReceived = new List<byte[]>();
        //LOW
        internal static string[] AvailablePorts {
            get { return SerialPort.GetPortNames(); }
        }
        //LOW
        internal PortStates Open() {
            if (_serialPort != null) {
                if (_serialPort.IsOpen){
                    return PortStates.Opened;
                    //В лог: Уже открыт
                }
                _serialPort.Dispose();
            }
            _serialPort = new SerialPort();
            _serialPort.PortName = Flavor.Common.Config.Port;
            _serialPort.BaudRate = Flavor.Common.Config.BaudRate;
            _serialPort.DataBits = 8;
            _serialPort.Parity = System.IO.Ports.Parity.None;
            _serialPort.StopBits = System.IO.Ports.StopBits.One;
            _serialPort.ReadTimeout = 1000;
            _serialPort.WriteTimeout = 1000;

            try {
                _serialPort.Open();
            } catch (Exception Error) {
                System.Windows.Forms.MessageBox.Show(Error.Message, "Ошибка обращения к последовательному порту");
                return PortStates.ErrorOpening;
            }
            Receiving();
            return PortStates.Opening;
        }
        //LOW
        internal PortStates Close() {
            if (_serialPort == null) {
                System.Windows.Forms.MessageBox.Show("Порт не инициализирован", "Ошибка обращения к последовательному порту");
                return PortStates.ErrorClosing;
            }
            if (!_serialPort.IsOpen) {
                return PortStates.Closed;
                //В лог Уже закрыт
            }
            try {
                StopReceiving();
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
            } catch (Exception Error) {
                System.Windows.Forms.MessageBox.Show(Error.Message, "Ошибка обращения к последовательному порту");
                return PortStates.ErrorClosing;
            }
            return PortStates.Closing;
        }
        //LOW
        internal void Send(byte[] message) {
            try {
                _serialPort.Write(message, 0, message.Length);
            } catch {
                // BAD! consider revising
                Flavor.Common.ConsoleWriter.WriteLine("Error writing this command to serial port:");
                //throw new ModBusException();
            } finally {
                Flavor.Common.ConsoleWriter.Write("[out]");
                foreach (byte b in message) {
                    Flavor.Common.ConsoleWriter.Write((char)b);
                }
                Flavor.Common.ConsoleWriter.WriteLine();
            }
        }
        //HI
        private void Receiving() {
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPort_DataReceived);
        }
        //HI
        private void StopReceiving() {
            _serialPort.DataReceived -= new SerialDataReceivedEventHandler(_serialPort_DataReceived);
        }
        //HI
        private void _serialPort_DataReceived(object sender, EventArgs e) {
            SerialPort port = sender as SerialPort;
            while (port.IsOpen && port.BytesToRead > 0) {
                byte ch;
                try {
                    ch = (byte)port.ReadByte();
                } catch {
                    Flavor.Common.ConsoleWriter.WriteLine("Error(reading byte)");
                    continue;
                    // не получилось;
                }
                Flavor.Common.ConsoleWriter.Write((char)ch);
                DispatchByte(ch);
            }
            foreach (byte[] raw_command in PacketReceived) {
                //if (Commander.pState != Commander.programStates.Start)
                // very bad! rise an even here!
                Flavor.Common.Commander.Realize(Parse(raw_command));
            }
            PacketReceived.Clear();
        }
        //HI
        private void DispatchByte(byte data) {
            switch (PackState) {
                case PacketingState.Idle: {
                        if (data == (byte)':') {
                            PacketBuffer.Clear();
                            PackState = PacketingState.WaitUpper;
                        } else {
                            // rise a logging event here
                            Flavor.Common.ConsoleWriter.WriteLine("Error({0})", data);
                            //Symbol outside packet
                        }
                        break;
                    }
                case PacketingState.WaitUpper: {
                        if (data == 0x0d) {
                            PacketReceived.Add(PacketBuffer.ToArray());
                            PacketBuffer.Clear();
                            Flavor.Common.ConsoleWriter.WriteLine();
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
        private static void _Parse(byte[] raw_command) {
            int minLength = 2;
            if (raw_command.Length < minLength)
                return; // TODO: raise logging event, too short
            if (!checkCS(raw_command))
                return; // TODO: raise logging event, wrong CS
            CommandCode commandcode = (CommandCode)raw_command[0];
            if (false) // check for command
                return; // TODO: raise logging event, wrong command
            if (false) // check for length using code-length table
                return; // TODO: raise logging event, wrong command
            ServicePacket packet;
            switch (commandcode) {
                default:
                    return; // TODO: raise logging event
            }
        }
        //HI+
        internal static ServicePacket Parse(byte[] raw_command) {
            ///<summary> CS проверка <summary>
            if (raw_command.Length >= 2) {
                if (checkCS(raw_command)) {
                    //ConsoleWriter.WriteLine("Контрольная сумма в порядке");
                    ///<summary> Отделяем функциональный код команды,
                    ///отталкиваясь от него принимаем решение о создании команды
                    ///<summary>
                    // TODO: raise events here with commands as event args
                    switch ((CommandCode)raw_command[0]) {
                        case CommandCode.GetState:
                            if (raw_command.Length == 3)
                                return new SyncReply.updateState(raw_command[1]);
                            break;
                        case CommandCode.GetStatus:
                            if (raw_command.Length == 29)
                                return new SyncReply.updateStatus(raw_command[1],
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
                                return new SyncReply.confirmShutdown();
                            break;
                        case CommandCode.Init:
                            if (raw_command.Length == 2)
                                return new SyncReply.confirmInit();
                            break;
                        case CommandCode.SetHeatCurrent:
                            if (raw_command.Length == 2)
                                return new SyncReply.confirmHCurrent();
                            break;
                        case CommandCode.SetEmissionCurrent:
                            if (raw_command.Length == 2)
                                return new SyncReply.confirmECurrent();
                            break;
                        case CommandCode.SetIonizationVoltage:
                            if (raw_command.Length == 2)
                                return new SyncReply.confirmIVoltage();
                            break;
                        case CommandCode.SetFocusVoltage1:
                            if (raw_command.Length == 2)
                                return new SyncReply.confirmF1Voltage();
                            break;
                        case CommandCode.SetFocusVoltage2:
                            if (raw_command.Length == 2)
                                return new SyncReply.confirmF2Voltage();
                            break;
                        case CommandCode.SetScanVoltage:
                            if (raw_command.Length == 2)
                                return new SyncReply.confirmSVoltage();
                            break;
                        case CommandCode.SetCapacitorVoltage:
                            if (raw_command.Length == 2)
                                return new SyncReply.confirmCP();
                            break;
                        case CommandCode.Measure:
                            if (raw_command.Length == 2)
                                return new SyncReply.confirmMeasure();
                            break;
                        case CommandCode.GetCounts:
                            if (raw_command.Length == 8)
                                return new SyncReply.updateCounts((int)raw_command[1] + ((int)raw_command[2] << 8) + ((int)raw_command[3] << 16),
                                                        (int)raw_command[4] + ((int)raw_command[5] << 8) + ((int)raw_command[6] << 16),
                                                        delegate {
                                                            if (Commander.CurrentMeasureMode == null) {
                                                                // fake packet. BAD solution
                                                                return;
                                                            }
                                                            // Not the best place for automatic refresh!
                                                            // move further to Commander, rise an event!
                                                            Commander.CurrentMeasureMode.updateGraph();});
                            break;
                        case CommandCode.heatCurrentEnable:
                            if (raw_command.Length == 2)
                                return new SyncReply.confirmHECurrent();
                            break;
                        case CommandCode.EnableHighVoltage:
                            if (raw_command.Length == 2)
                                return new SyncReply.confirmHighVoltage();
                            break;
                        case CommandCode.GetTurboPumpStatus:
                            if (raw_command.Length == 17)
                                return new SyncReply.updateTurboPumpStatus((ushort)((ushort)raw_command[1] + ((ushort)raw_command[2] << 8)),
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
                                return new SyncReply.confirmForvacuumLevel();
                            break;
                        case CommandCode.InvalidCommand:
                            byte[] tempArray = new byte[raw_command.Length - 1];
                            raw_command.CopyTo(tempArray, 1);
                            return new SyncErrorReply.logInvalidCommand(tempArray);
                        case CommandCode.InvalidChecksum:
                            if (raw_command.Length == 2)
                                return new SyncErrorReply.logInvalidChecksum();
                            break;
                        case CommandCode.InvalidPacket:
                            if (raw_command.Length == 2)
                                return new SyncErrorReply.logInvalidPacket();
                            break;
                        case CommandCode.InvalidLength:
                            if (raw_command.Length == 2)
                                return new SyncErrorReply.logInvalidLength();
                            break;
                        case CommandCode.InvalidData:
                            if (raw_command.Length == 2)
                                return new SyncErrorReply.logInvalidData();
                            break;
                        case CommandCode.InvalidState:
                            if (raw_command.Length == 2)
                                return new SyncErrorReply.logInvalidState();
                            break;
                        case CommandCode.InternalError:
                            if (raw_command.Length == 3)
                                return new AsyncErrorReply.logInternalError(raw_command);
                            break;
                        case CommandCode.InvalidSystemState:
                            if (raw_command.Length == 2) {
                                return new AsyncErrorReply.logInvalidSystemState(raw_command);
                            }
                            break;
                        case CommandCode.VacuumCrash:
                            if (raw_command.Length == 3)
                                return new AsyncErrorReply.logVacuumCrash(raw_command);
                            break;
                        case CommandCode.TurboPumpFailure:
                            if (raw_command.Length == 17)
                                return new AsyncErrorReply.logTurboPumpFailure(raw_command);
                            break;
                        case CommandCode.PowerFail:
                            if (raw_command.Length == 2)
                                return new AsyncErrorReply.logPowerFail(raw_command);
                            break;
                        case CommandCode.InvalidVacuumState:
                            if (raw_command.Length == 2)
                                return new AsyncErrorReply.logInvalidVacuumState(raw_command);
                            break;
                        case CommandCode.AdcPlaceIonSrc:
                            //!!!
                            if (raw_command.Length >= 2)
                                return new AsyncErrorReply.logAdcPlaceIonSrc(raw_command);
                            break;
                        case CommandCode.AdcPlaceScanv:
                            //!!!
                            if (raw_command.Length >= 2)
                                return new AsyncErrorReply.logAdcPlaceScanv(raw_command);
                            break;
                        case CommandCode.AdcPlaceControlm:
                            //!!!
                            if (raw_command.Length >= 2)
                                return new AsyncErrorReply.logAdcPlaceControlm(raw_command);
                            break;
                        case CommandCode.Measured:
                            if (raw_command.Length == 2)
                                return new AsyncReply.requestCounts();
                            break;
                        case CommandCode.VacuumReady:
                            if (raw_command.Length == 2)
                                return new AsyncReply.confirmVacuumReady();
                            break;
                        case CommandCode.SystemShutdowned:
                            if (raw_command.Length == 2)
                                return new AsyncReply.confirmShutdowned();
                            break;
                        case CommandCode.SystemReseted:
                            if (raw_command.Length == 2)
                                return new AsyncReply.SystemReseted();
                            break;
                        case CommandCode.HighVoltageOff:
                            if (raw_command.Length == 2)
                                return new AsyncReply.confirmHighVoltageOff();
                            break;
                        case CommandCode.HighVoltageOn:
                            if (raw_command.Length == 2)
                                return new AsyncReply.confirmHighVoltageOn();
                            break;
                        default:
                            ConsoleWriter.WriteLine("Неверная команда");
                            break;
                    }
                } else {
                    ConsoleWriter.WriteLine("Неверная контрольная сумма");
                }
            } else {
                ConsoleWriter.WriteLine("Короткий пакет");
            }
            return ServicePacket.ZERO;
        }
        //HI
        private static byte ComputeChecksum(byte[] data) {
            byte checkSum = 0;
            for (int i = 0; i < data.Length; i++) {
                checkSum -= data[i];
            }
            return checkSum;
        }
        //HI
        private static bool checkCS(byte[] data) {
            return true ^ Convert.ToBoolean(ComputeChecksum(data));
        }
        //HI
        internal static byte[] buildPack(byte[] data) {
            List<byte> pack = new List<byte>();
            pack.Add((byte)':');
            buildPackBody(pack, data);
            pack.Add((byte)'\r');
            return pack.ToArray();
        }
        //HI
        internal static void buildPackBody(List<byte> pack, byte[] data) {
            for (int i = 0; i < data.Length; i++) {
                pack.Add(GetNibble(data[i] >> 4));
                pack.Add(GetNibble(data[i]));
            }
            byte cs = ComputeChecksum(data);
            pack.Add(GetNibble(cs >> 4));
            pack.Add(GetNibble(cs));
        }
        //HI
        internal static byte[] collectData(byte functCode) {
            return new byte[] { functCode };
        }
        //HI
        internal static byte[] collectData(byte functCode, byte value) {
            return new byte[] { functCode, value };
        }
        //HI
        internal static byte[] collectData(byte functCode, ushort value) {
            byte[] data = ushort2ByteArray(value);
            return new byte[] { functCode, data[0], data[1] };
        }
        //HI
        internal static byte[] collectData(byte functCode, ushort value1, ushort value2) {
            byte[] data1 = ushort2ByteArray(value1);
            byte[] data2 = ushort2ByteArray(value2);
            return new byte[] { functCode, data1[0], data1[1], data2[0], data2[1] };
        }
        //HI
        internal static byte[] collectData(byte functCode, int value1, int value2) {
            List<byte> Data = new List<byte>();
            Data.Add(functCode);
            Data.AddRange(int2ByteArray(value1));
            Data.AddRange(int2ByteArray(value2));
            return Data.ToArray();
        }
        //HI
        internal static byte[] ushort2ByteArray(ushort value) {
            if (value < 0) value = 0;
            if (value > 4095) value = 4095;
            return new byte[] { (byte)(value), (byte)(value >> 8) };
        }
        //HI
        internal static byte[] int2ByteArray(int value) {
            if (value < 0) value = 0;
            if (value > 16777215) value = 16777215;
            return new byte[] { (byte)(value), (byte)(value >> 8), (byte)(value >> 16) };
        }
        //HI
        private static byte GetNibble(int data) {
            data &= 0x0F;
            if (data < 10) {
                return (byte)(data + (int)'0');
            } else {
                return (byte)(data + (int)'a' - 10);
            }
        }
        //HI
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
    }
}
