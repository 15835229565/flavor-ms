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
    abstract class IonSourceGetReply: SyncReply, IChannel {
        readonly ushort voltage;
        protected IonSourceGetReply(ushort voltage) {
            this.voltage = voltage;
        }
        public static IonSourceGetReply Parse(params byte[] data) {
            try {
                ushort voltage = data[0];
                voltage &= 0xF;
                voltage <<= 8;
                voltage += data[1];
                byte channel = data[0];
                channel >>= 4;
                ++channel;
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
            catch {
                return null;
            }
        }
        public override CommandCode Id {
            get { return CommandCode.SPI_PSIS_GetVoltage; }
        }
        // what to do with request?
        public override bool Equals(object other) {
            // BAD: asymmetric
            if (base.Equals(other))
                return (other as IChannel).Channel == this.Channel;
            return false;
        }
        public override int GetHashCode() {
            return base.GetHashCode() + 19 * Channel.GetHashCode();
        }

        #region IChannel Members
        public abstract byte Channel { get; }
        #endregion
    }
    class GetEmissionCurrentReply: IonSourceGetReply {
        public GetEmissionCurrentReply(ushort voltage)
            : base(voltage) { }
        public override byte Channel {
            get { return 1; }
        }
    }
    class GetIonizationVoltageReply: IonSourceGetReply {
        public GetIonizationVoltageReply(ushort voltage)
            : base(voltage) { }
        public override byte Channel {
            get { return 2; }
        }
    }
    class GetF1VoltageReply: IonSourceGetReply {
        public GetF1VoltageReply(ushort voltage)
            : base(voltage) { }
        public override byte Channel {
            get { return 3; }
        }
    }
    class GetF2VoltageReply: IonSourceGetReply {
        public GetF2VoltageReply(ushort voltage)
            : base(voltage) { }
        public override byte Channel {
            get { return 4; }
        }
    }
}