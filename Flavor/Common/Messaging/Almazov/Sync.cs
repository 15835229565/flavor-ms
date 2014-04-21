using System;
using System.Collections.Generic;
using CommandCode = Flavor.Common.Messaging.Almazov.CommandCode;
using SyncReply = Flavor.Common.Messaging.SyncReply<Flavor.Common.Messaging.Almazov.CommandCode>;

namespace Flavor.Common.Messaging.Almazov {
    class CPUStatusReply: SyncReply {
        readonly byte incTIC, incMS;
        public CPUStatusReply(byte incTIC, byte incMS) {
            this.incTIC = incTIC;
            this.incMS = incMS;
        }
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
        public override CommandCode Id {
            get { return CommandCode.HVE; }
        }
        #region IUpdateDevice Members
        public void UpdateDevice(IDevice device) {
            //TODO: implement
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
        public override CommandCode Id {
            get { return CommandCode.PRGE; }
        }
        #region IUpdateDevice Members
        public void UpdateDevice(IDevice device) {
            //TODO: implement
        }
        public void UpdateDevice() {
            throw new NotImplementedException();
        }
        #endregion
    }
    class TICStatusReply: SyncReply, IUpdateDevice {
        public readonly string Request = "?V902\r";
        readonly bool turbo, relay1, relay2, relay3;
        readonly int alert;
        public TICStatusReply(bool turbo, bool relay1, bool relay2, bool relay3, int alert) {
            this.turbo = turbo;
            this.relay1 = relay1;
            this.relay2 = relay2;
            this.relay3 = relay3;
            this.alert = alert;
        }
        public override CommandCode Id {
            get { return CommandCode.TIC_Retransmit; }
        }
        public void UpdateDevice(IDevice device) {
            throw new NotImplementedException();
        }
        public void UpdateDevice() {
            throw new NotImplementedException();
        }
        public override bool Equals(object other) {
            // BAD: asymmetric
            if (base.Equals(other))
                return (other as TICStatusRequest).Request.Equals(this.Request);
            return false;
        }
        public override int GetHashCode() {
            return base.GetHashCode() + 17 * Request.GetHashCode();
        }
    }
}