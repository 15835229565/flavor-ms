using System.Collections.Generic;

namespace Flavor.Common.Messaging.SevMorGeo {
    using AsyncErrorReply = AsyncError<CommandCode>;

    class logInternalError: AsyncErrorReply {
        public override string Message {
            get { return "Internal error " + internalError; }
        }
        readonly byte internalError;
        public logInternalError(byte error) {
            internalError = error;
        }
        public logInternalError()
            : this(0) { }
    }

    class logInvalidSystemState: AsyncErrorReply {
        public override string Message {
            get { return "Wrong system state"; }
        }
        public logInvalidSystemState() { }
    }

    class logVacuumCrash: AsyncErrorReply {
        public override string Message {
            get { return "Vacuum crash state " + vacState; }
        }
        public logVacuumCrash()
            : this(0) { }
        readonly byte vacState;
        public logVacuumCrash(byte state) {
            vacState = state;
        }
    }

    class logTurboPumpFailure: AsyncErrorReply, IUpdateDevice {
        ushort turboSpeed;
        ushort turboCurrent;
        ushort pwm;
        ushort pumpTemp;
        ushort driveTemp;
        ushort operationTime;
        byte v1;
        byte v2;
        byte v3;
        public override string Message {
            get { return "Turbopump failure"; }
        }
        public logTurboPumpFailure(IList<byte> commandline) {
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
        public logTurboPumpFailure() { }
        #region IUpdateDevice Members
        public void UpdateDevice(IDevice device) {
            throw new System.NotImplementedException();
        }
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

    class logPowerFail: AsyncErrorReply {
        public override string Message {
            get { return "Device power fail"; }
        }
        public logPowerFail() { }
    }

    class logInvalidVacuumState: AsyncErrorReply {
        public override string Message {
            get { return "Wrong vacuum state"; }
        }
        public logInvalidVacuumState() { }
    }

    class logAdcPlaceIonSrc: AsyncErrorReply {
        public override string Message {
            get { return "AdcPlaceIonSrc"; }
        }
        public logAdcPlaceIonSrc(IList<byte> commandline) { }
        public logAdcPlaceIonSrc() { }
    }

    class logAdcPlaceScanv: AsyncErrorReply {
        public override string Message {
            get { return "AdcPlaceScanv"; }
        }
        public logAdcPlaceScanv(IList<byte> commandline) { }
        public logAdcPlaceScanv() { }
    }

    class logAdcPlaceControlm: AsyncErrorReply {
        public override string Message {
            get { return "AdcPlaceControlm"; }
        }
        public logAdcPlaceControlm(IList<byte> commandline) { }
        public logAdcPlaceControlm() { }
    }
}
