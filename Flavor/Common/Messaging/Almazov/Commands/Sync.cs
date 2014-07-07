using System;
using System.Collections.Generic;
using CommandCode = Flavor.Common.Messaging.Almazov.CommandCode;
using SyncReply = Flavor.Common.Messaging.SyncReply<Flavor.Common.Messaging.Almazov.CommandCode>;

namespace Flavor.Common.Messaging.Almazov.Commands {
    class CPUStatusReply: SyncReply {
        readonly byte incTIC, incMS;
        // experimental parameter order
        public CPUStatusReply(byte incMS, byte incTIC) {
            this.incTIC = incTIC;
            this.incMS = incMS;
        }
        public CPUStatusReply()
            : this(0, 0) { }
        [Flags]
        enum Incident_PC : byte {
            LOCKisLost = 0x01,       //МК принимал пакет, но в последний байт пакета не затвор
            TooShortPacket = 0x02,   //Длина пакета меньше минимального
            TooFast = 0x04,          //МК ещё не выполнил предыдущую команду, а уже приходит другая.
            Silence = 0x08,          //МК больше 60 секунд не связывался с ПК
            Noise = 0x10,           //На линии МК-ПК был замечен шум
        }
        [Flags]
        enum Incident_TIC: byte {
            LOCKisLost = 0x01,       //МК принимал пакет, но в последний байт пакета не затвор
            TooShortPacket = 0x02,   //Длина пакета меньше минимального
            HVE_TimeOut = 0x04,       //Ошибка системы мониторинга высокого напряжения (таймаут)
            Silence = 0x08,          //TIC не отвечает
            Noise = 0x10,           //На линии МК-TIC был замечен шум
            HVE_error = 0x20,       //Ошибка системы мониторинга высокого напряжения (неверные данные)
            wrongTimerState = 0x40, //Таймер TIC'a находился в неверном состоянии!
        }
        public override CommandCode Id {
            get { return CommandCode.CPU_Status; }
        }
    }
    class TICStatusReply: SyncReply, ITIC, IUpdateDevice {
        public string Request { get { return "?V902\r"; } }
        readonly bool turbo, relay1, relay2, relay3;
        readonly int alert;
        public TICStatusReply(bool turbo, bool relay1, bool relay2, bool relay3, int alert) {
            this.turbo = turbo;
            this.relay1 = relay1;
            this.relay2 = relay2;
            this.relay3 = relay3;
            this.alert = alert;
        }
        public TICStatusReply() : this(false, false, false, false, 0) { }
        public override CommandCode Id {
            get { return CommandCode.TIC_Retransmit; }
        }
        public void UpdateDevice(IDevice device) {
            device.UpdateStatus(turbo, relay1, relay2, relay3, alert);
        }
        public void UpdateDevice() {
            throw new NotImplementedException();
        }
        public override bool Equals(object other) {
            // BAD: asymmetric
            if (base.Equals(other))
                return (other as ITIC).Request.Equals(this.Request);
            return false;
        }
        public override int GetHashCode() {
            return base.GetHashCode() + 17 * Request.GetHashCode();
        }
    }
    class HighVoltagePermittedStatusReply: SyncReply, IUpdateDevice {
        readonly bool enabled;
        public HighVoltagePermittedStatusReply(bool enabled) {
            this.enabled = enabled;
        }
        public HighVoltagePermittedStatusReply(): this(false) { }
        public override CommandCode Id {
            get { return CommandCode.HVE; }
        }
        #region IUpdateDevice Members
        public void UpdateDevice(IDevice device) {
            device.OperationReady(enabled);
        }
        public void UpdateDevice() {
            throw new System.NotImplementedException();
        }
        #endregion
    }
    class OperationBlockReply: SyncReply {
        public static OperationBlockReply Parse(byte b) {
            switch (b) {
                case 0:
                    return new OperationBlockOffReply();
                case 1:
                    return new OperationBlockOnReply();
                case 254:
                default:
                    return new OperationBlockReply();
            }
        }
        public override CommandCode Id {
            get { return CommandCode.PRGE; }
        }
    }
    abstract class OperationBlockOnOffReply: OperationBlockReply, IUpdateDevice {
        readonly bool on;
        protected OperationBlockOnOffReply(bool on) {
            this.on = on;
        }
        #region IUpdateDevice Members
        public void UpdateDevice(IDevice device) {
            device.OperationBlock(on);
        }
        public void UpdateDevice() {
            throw new NotImplementedException();
        }
        #endregion
    }
    class OperationBlockOnReply: OperationBlockOnOffReply {
        public OperationBlockOnReply()
            : base(true) { }
    }
    class OperationBlockOffReply: OperationBlockOnOffReply {
        public OperationBlockOffReply()
            : base(false) { }
    }
    abstract class FlagReply: SyncReply, IUpdateDevice {
        readonly bool? on;
        protected FlagReply(byte b) {
            switch (b) {
                case 0:
                    on = false;
                    break;
                case 1:
                    on = true;
                    break;
                default:
                    on = null;
                    break;
            }
        }
        #region IUpdateDevice Members
        public void UpdateDevice() {
            throw new NotImplementedException();
        }
        abstract public void UpdateDevice(IDevice device);
        #endregion
    }
    class Valve1Reply: FlagReply {
        public Valve1Reply(byte b)
            : base(b) { }
        public Valve1Reply()
            : this(byte.MaxValue) { }
        public override CommandCode Id {
            get { return CommandCode.SEMV1; }
        }
        #region IUpdateDevice Members
        public override void UpdateDevice(IDevice device) {
            // TODO: check Valve1 is turned on/off according to Relay1
            //if (on.HasValue)
            //    device.UpdateStatus();
        }
        #endregion
    }
    class Valve2Reply: FlagReply {
        public Valve2Reply(byte b)
            : base(b) { }
        public Valve2Reply()
            : this(byte.MaxValue) { }
        public override CommandCode Id {
            get { return CommandCode.SEMV2; }
        }
        #region IUpdateDevice Members
        public override void UpdateDevice(IDevice device) {
            // TODO:
            //if (on.HasValue)
            //    device.UpdateStatus();
        }
        #endregion
    }
    class Valve3Reply: FlagReply {
        public Valve3Reply(byte b)
            : base(b) { }
        public Valve3Reply()
            : this(byte.MaxValue) { }
        public override CommandCode Id {
            get { return CommandCode.SEMV3; }
        }
        #region IUpdateDevice Members
        public override void UpdateDevice(IDevice device) {
            // TODO:
            //if (on.HasValue)
            //    device.UpdateStatus();
        }
        #endregion
    }
    class MicroPumpReply: FlagReply {
        public MicroPumpReply(byte b)
            : base(b) { }
        public MicroPumpReply()
            : this(byte.MaxValue) { }
        public override CommandCode Id {
            get { return CommandCode.SPUMP; }
        }
        #region IUpdateDevice Members
        public override void UpdateDevice(IDevice device) {
            // TODO:
            //if (on.HasValue)
            //    device.UpdateStatus();
        }
        #endregion
    }
    class IonSourceSetReply: SyncReply {
        public override CommandCode Id {
            get { return CommandCode.SPI_PSIS_SetVoltage; }
        }
    }
    class DetectorSetReply: SyncReply {
        public override CommandCode Id {
            get { return CommandCode.SPI_DPS_SetVoltage; }
        }
    }
    class InletSetReply: SyncReply {
        public override CommandCode Id {
            get { return CommandCode.SPI_PSInl_SetVoltage; }
        }
    }
    class CapacitorVoltageSetReply: SyncReply {
        public override CommandCode Id {
            get { return CommandCode.SPI_CP_SetVoltage; }
        }
    }
    class ScanVoltageSetReply: SyncReply {
        public override CommandCode Id {
            get { return CommandCode.SPI_Scan_SetVoltage; }
        }
    }
    abstract class ADCGetReply: SyncReply, IChannel {
        readonly byte channel;
        readonly ushort voltage;
        protected ADCGetReply(byte channel, ushort voltage) {
            this.channel = channel;
            this.voltage = voltage;
        }
        // what to do with request?
        public override bool Equals(object other) {
            // BAD: asymmetric
            if (base.Equals(other))
                return (other as IChannel).Channel == this.Channel;
            return false;
        }
        protected static void Parse(IList<byte> data, out byte channel, out ushort voltage) {
            voltage = data[0];
            voltage &= 0xF;
            voltage <<= 8;
            voltage += data[1];
            channel = data[0];
            channel >>= 4;
            ++channel;
        }
        public override int GetHashCode() {
            return base.GetHashCode() + 19 * Channel.GetHashCode();
        }

        #region IChannel Members
        public byte Channel { get { return channel; } }
        #endregion
    }
    abstract class IonSourceGetReply: ADCGetReply {
        protected IonSourceGetReply(byte channel, ushort voltage)
            : base(channel, voltage) { }
        protected IonSourceGetReply(byte channel)
            : base(channel, 0) { }
        public static IonSourceGetReply Parse(IList<byte> data) {
            ushort voltage;
            byte channel;
            ADCGetReply.Parse(data, out channel, out voltage);
            switch (channel) {
                case 1:
                    return new GetEmissionCurrentReply(voltage);
                case 2:
                    return new GetIonizationVoltageReply(voltage);
                case 3:
                    return new GetF1VoltageReply(voltage);
                case 4:
                    return new GetF2VoltageReply(voltage);
                default:
                    return null;
            }
        }
        public override CommandCode Id {
            get { return CommandCode.SPI_PSIS_GetVoltage; }
        }
    }
    class GetEmissionCurrentReply: IonSourceGetReply {
        public GetEmissionCurrentReply(ushort voltage)
            : base(1, voltage) {
        }
        public GetEmissionCurrentReply()
            : base(1) { }
    }
    class GetIonizationVoltageReply: IonSourceGetReply {
        public GetIonizationVoltageReply(ushort voltage)
            : base(2, voltage) { }
        public GetIonizationVoltageReply()
            : base(2) { }
    }
    class GetF1VoltageReply: IonSourceGetReply {
        public GetF1VoltageReply(ushort voltage)
            : base(3, voltage) { }
        public GetF1VoltageReply()
            : base(3) { }
    }
    class GetF2VoltageReply: IonSourceGetReply {
        public GetF2VoltageReply(ushort voltage)
            : base(4, voltage) { }
        public GetF2VoltageReply()
            : base(4) { }
    }
    abstract class DetectorGetReply: ADCGetReply {
        protected DetectorGetReply(byte channel, ushort voltage)
            : base(channel, voltage) { }
        protected DetectorGetReply(byte channel)
            : base(channel, 0) { }
        public static DetectorGetReply Parse(IList<byte> data) {
            ushort voltage;
            byte channel;
            ADCGetReply.Parse(data, out channel, out voltage);
            switch (channel) {
                case 1:
                    return new GetD1VoltageReply(voltage);
                case 2:
                    return new GetD2VoltageReply(voltage);
                case 3:
                    return new GetD3VoltageReply(voltage);
                default:
                    return null;
            }
        }
        public override CommandCode Id {
            get { return CommandCode.SPI_DPS_GetVoltage; }
        }
    }
    class GetD1VoltageReply: DetectorGetReply {
        public GetD1VoltageReply(ushort voltage)
            : base(1, voltage) { }
        public GetD1VoltageReply()
            : base(1) { }
    }
    class GetD2VoltageReply: DetectorGetReply {
        public GetD2VoltageReply(ushort voltage)
            : base(2, voltage) { }
        public GetD2VoltageReply()
            : base(2) { }
    }
    class GetD3VoltageReply: DetectorGetReply {
        public GetD3VoltageReply(ushort voltage)
            : base(3, voltage) { }
        public GetD3VoltageReply()
            : base(3) { }
    }
    abstract class InletGetReply: ADCGetReply {
        protected InletGetReply(byte channel, ushort voltage)
            : base(channel, voltage) { }
        protected InletGetReply(byte channel)
            : base(channel, 0) { }
        public static InletGetReply Parse(IList<byte> data) {
            ushort voltage;
            byte channel;
            ADCGetReply.Parse(data, out channel, out voltage);
            switch (channel) {
                case 1:
                    return new GetInletVoltageReply(voltage);
                case 2:
                    return new GetHeaterVoltageReply(voltage);
                default:
                    return null;
            }
        }
        public override CommandCode Id {
            get { return CommandCode.SPI_PSInl_GetVoltage; }
        }
    }
    class GetInletVoltageReply: InletGetReply {
        public GetInletVoltageReply(ushort voltage)
            : base(1,voltage) { }
        public GetInletVoltageReply()
            : base(1) { }
    }
    class GetHeaterVoltageReply: InletGetReply {
        public GetHeaterVoltageReply(ushort voltage)
            : base(2, voltage) { }
        public GetHeaterVoltageReply()
            : base(2) { }
    }

    class SendMeasureReply: SyncReply {
        public SendMeasureReply(byte status) { }
        public SendMeasureReply() { }
        public override CommandCode Id {
            get { return CommandCode.RTC_StartMeasure; }
        }
    }

    class CountsReply: SyncReply, IUpdateDevice {
        readonly byte status;
        readonly uint COA, COB, COC;
        readonly double OverTime;
        const double sourceFrequency = 32.768;//кГц - опорная частота таймера
        public CountsReply(IList<byte> raw) {
            status = raw[0];
            if (status == 0) {
                COA = (uint)(raw[1] * 16777216 + raw[2] * 65536 + raw[3] * 256 + raw[4]);
                COB = (uint)(raw[5] * 16777216 + raw[6] * 65536 + raw[7] * 256 + raw[8]);
                COC = (uint)(raw[9] * 16777216 + raw[10] * 65536 + raw[11] * 256 + raw[12]);
                double prescaler_long;
                switch (raw[15]) {
                    case 1: prescaler_long = sourceFrequency; break;
                    case 2: prescaler_long = sourceFrequency / 2; break;
                    case 3: prescaler_long = sourceFrequency / 8; break;
                    case 4: prescaler_long = sourceFrequency / 16; break;
                    case 5: prescaler_long = sourceFrequency / 64; break;
                    case 6: prescaler_long = sourceFrequency / 256; break;
                    case 7: prescaler_long = sourceFrequency / 1024; break;
                    default: prescaler_long = 0; break;
                }
                OverTime = Math.Round((((raw[13] << 8) + raw[14]) / prescaler_long) * 1000) / 1000;
                //return true;
            }
            //return false;
        }
        public CountsReply() { }
        public override CommandCode Id {
            get { return CommandCode.RTC_ReceiveResults; }
        }
        #region IUpdateDevice Members
        public void UpdateDevice(IDevice device) {
            // temporarily changed order
            device.Detectors = new uint[] { COA, COC, COB };
            //device.Detectors = new uint[] { COA, COB, COC };
        }
        public void UpdateDevice() {
            throw new NotImplementedException();
        }
        #endregion
    }
}