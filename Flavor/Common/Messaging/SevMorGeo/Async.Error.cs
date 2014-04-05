using System.Collections.Generic;
using AsyncErrorReply = Flavor.Common.Messaging.AsyncError<Flavor.Common.Messaging.SevMorGeo.CommandCode>;

namespace Flavor.Common.Messaging.SevMorGeo {
    internal class logInternalError: AsyncErrorReply {
        public override string Message {
            get { return "Internal error " + internalError; }
        }
        private byte internalError;
        internal logInternalError(byte error) {
            internalError = error;
        }
    }

    internal class logInvalidSystemState: AsyncErrorReply {
        public override string Message {
            get { return "Wrong system state"; }
        }
        internal logInvalidSystemState() { }
    }

    internal class logVacuumCrash: AsyncErrorReply {
        public override string Message {
            get { return "Vacuum crash state " + vacState; }
        }
        byte vacState;
        internal logVacuumCrash(byte state) {
            vacState = state;
        }
    }

    internal class logTurboPumpFailure: AsyncErrorReply, IUpdateDevice {
        private ushort turboSpeed;
        private ushort turboCurrent;
        private ushort pwm;
        private ushort pumpTemp;
        private ushort driveTemp;
        private ushort operationTime;
        private byte v1;
        private byte v2;
        private byte v3;
        public override string Message {
            get { return "Turbopump failure"; }
        }
        internal logTurboPumpFailure(IList<byte> commandline) {
            turboSpeed = (ushort)((ushort)commandline[1] + ((ushort)commandline[2] << 8));
            turboCurrent = (ushort)((ushort)commandline[3] + ((ushort)commandline[4] << 8));
            pwm = (ushort)((ushort)commandline[5] + ((ushort)commandline[6] << 8));
            pumpTemp = (ushort)((ushort)commandline[7] + ((ushort)commandline[8] << 8));
            driveTemp = (ushort)((ushort)commandline[9] + ((ushort)commandline[10] << 8));
            operationTime = (ushort)((ushort)commandline[11] + ((ushort)commandline[12] << 8));
            v1 = commandline[13];
            v2 = commandline[14];
            v3 = commandline[15];
        }
        #region IUpdateDevice Members

        public void UpdateDevice() {
            Device.TurboPump.Speed = turboSpeed;
            Device.TurboPump.Current = turboCurrent;
            Device.TurboPump.pwm = pwm;
            Device.TurboPump.PumpTemperature = pumpTemp;
            Device.TurboPump.DriveTemperature = driveTemp;
            Device.TurboPump.OperationTime = operationTime;
            Device.TurboPump.relaysState(v1, v2, v3);
        }

        #endregion
    }

    internal class logPowerFail: AsyncErrorReply {
        public override string Message {
            get { return "Device power fail"; }
        }
        internal logPowerFail() { }
    }

    internal class logInvalidVacuumState: AsyncErrorReply {
        public override string Message {
            get { return "Wrong vacuum state"; }
        }
        internal logInvalidVacuumState() { }
    }

    internal class logAdcPlaceIonSrc: AsyncErrorReply {
        public override string Message {
            get { return "AdcPlaceIonSrc"; }
        }
        internal logAdcPlaceIonSrc(IList<byte> commandline) { }
    }

    internal class logAdcPlaceScanv: AsyncErrorReply {
        public override string Message {
            get { return "AdcPlaceScanv"; }
        }
        internal logAdcPlaceScanv(IList<byte> commandline) { }
    }

    internal class logAdcPlaceControlm: AsyncErrorReply {
        public override string Message {
            get { return "AdcPlaceControlm"; }
        }
        internal logAdcPlaceControlm(IList<byte> commandline) { }
    }
}
