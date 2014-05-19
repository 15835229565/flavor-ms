using System;
using System.Collections.Generic;
using CommandCode = Flavor.Common.Messaging.Almazov.CommandCode;
using SyncReply = Flavor.Common.Messaging.SyncReply<Flavor.Common.Messaging.Almazov.CommandCode>;

namespace Flavor.Common.Messaging.Almazov.Commands {
    class CPUStatusReply: SyncReply {
        readonly byte incTIC, incMS;
        public CPUStatusReply(byte incTIC, byte incMS) {
            this.incTIC = incTIC;
            this.incMS = incMS;
        }
        public CPUStatusReply() : this(0, 0) { }
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
    class OperationBlockReply: SyncReply, IUpdateDevice {
        readonly bool? on;
        public OperationBlockReply(bool? on) {
            this.on = on;
        }
        public OperationBlockReply(): this(null) { }
        public override CommandCode Id {
            get { return CommandCode.PRGE; }
        }
        #region IUpdateDevice Members
        public void UpdateDevice(IDevice device) {
            if (on.HasValue)
                device.OperationBlock(on.Value);
        }
        public void UpdateDevice() {
            throw new NotImplementedException();
        }
        #endregion
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
        public TICStatusReply(): this(false, false, false, false, 0) { }
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
    class Valve1Reply: SyncReply, IUpdateDevice {
        readonly bool? on;
        public Valve1Reply(bool? on) {
            this.on = on;
        }
        public Valve1Reply(): this(null) { }
        public override CommandCode Id {
            get { return CommandCode.SEMV1; }
        }
        #region IUpdateDevice Members
        public void UpdateDevice(IDevice device) {
            // TODO: check Valve1 is turned on/off according to Relay1
            //if (on.HasValue)
            //    device.UpdateStatus();
        }
        public void UpdateDevice() {
            throw new NotImplementedException();
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
    abstract class ADCGetReply: SyncReply, IChannel {
        readonly ushort voltage;
        protected ADCGetReply(ushort voltage) {
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
        public abstract byte Channel { get; }
        #endregion
    }
    abstract class IonSourceGetReply: ADCGetReply {
        protected IonSourceGetReply(ushort voltage)
            : base(voltage) { }
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
            : base(voltage) { }
        public GetEmissionCurrentReply()
            : base(0) { }
        public override byte Channel {
            get { return 1; }
        }
    }
    class GetIonizationVoltageReply: IonSourceGetReply {
        public GetIonizationVoltageReply(ushort voltage)
            : base(voltage) { }
        public GetIonizationVoltageReply()
            : base(0) { }
        public override byte Channel {
            get { return 2; }
        }
    }
    class GetF1VoltageReply: IonSourceGetReply {
        public GetF1VoltageReply(ushort voltage)
            : base(voltage) { }
        public GetF1VoltageReply()
            : base(0) { }
        public override byte Channel {
            get { return 3; }
        }
    }
    class GetF2VoltageReply: IonSourceGetReply {
        public GetF2VoltageReply(ushort voltage)
            : base(voltage) { }
        public GetF2VoltageReply()
            : base(0) { }
        public override byte Channel {
            get { return 4; }
        }
    }
    abstract class DetectorGetReply: ADCGetReply {
        protected DetectorGetReply(ushort voltage)
            : base(voltage) { }
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
            : base(voltage) { }
        public GetD1VoltageReply()
            : base(0) { }
        public override byte Channel {
            get { return 1; }
        }
    }
    class GetD2VoltageReply: DetectorGetReply {
        public GetD2VoltageReply(ushort voltage)
            : base(voltage) { }
        public GetD2VoltageReply()
            : base(0) { }
        public override byte Channel {
            get { return 2; }
        }
    }
    class GetD3VoltageReply: DetectorGetReply {
        public GetD3VoltageReply(ushort voltage)
            : base(voltage) { }
        public GetD3VoltageReply()
            : base(0) { }
        public override byte Channel {
            get { return 3; }
        }
    }
    abstract class InletGetReply: ADCGetReply {
        protected InletGetReply(ushort voltage)
            : base(voltage) { }
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
            : base(voltage) { }
        public GetInletVoltageReply()
            : base(0) { }
        public override byte Channel {
            get { return 1; }
        }
    }
    class GetHeaterVoltageReply: InletGetReply {
        public GetHeaterVoltageReply(ushort voltage)
            : base(voltage) { }
        public GetHeaterVoltageReply()
            : base(0) { }
        public override byte Channel {
            get { return 2; }
        }
    }

    class CountsReply: SyncReply {
        public CountsReply(IList<byte> raw) {
            // TODO:
            throw new NotImplementedException();
        }
        public CountsReply() { }
        public override CommandCode Id {
            get { return CommandCode.RTC_ReceiveResults; }
        }
    }
}