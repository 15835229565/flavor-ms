using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Common.Commands.Interfaces;

namespace Flavor.Common.Commands.Async
{
    public abstract class AsyncErrorReply : ServicePacket
    {
        private byte[] cmdln;
        public AsyncErrorReply(byte[] commandline)
        {
            cmdln = commandline;
            Config.logCrash(cmdln);
        }
        public virtual string errorMessage
        {
            get{return "Async error reply";}
        }
    }

    public class logInternalError : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "Internal error " + internalError.ToString(); }
        }
        
        private byte internalError;

        public logInternalError(byte[] commandline): base(commandline)
        {
            internalError = commandline[1];
        }
    }

    public class logInvalidSystemState : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "Wrong system state"; }
        }
        public logInvalidSystemState(byte[] commandline)
            : base(commandline)
        {
        }
    }

    public class logVacuumCrash : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "Vacuum crash state " + vacState.ToString(); }
        }
        
        byte vacState;
        public logVacuumCrash(byte[] commandline)
            : base(commandline)
        {
            vacState = commandline[1];
        }
    }

    public class logTurboPumpFailure : AsyncErrorReply, IUpdateDevice
    {
        private ushort turboSpeed;
        private ushort turboCurrent;
        private ushort pwm;
        private ushort pumpTemp;
        private ushort driveTemp;
        private ushort operationTime;
        private byte v1;
        private byte v2;
        private byte v3;
        
        public override string errorMessage
        {
            get { return "Turbopump failure"; }
        }
        public logTurboPumpFailure(byte[] commandline)
            : base(commandline)
        {
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

        public void UpdateDevice()
        {
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

    public class logPowerFail : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "Device power fail"; }
        }
        public logPowerFail(byte[] commandline)
            : base(commandline)
        {
        }
    }

    public class logInvalidVacuumState : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "Wrong vacuum state"; }
        }
        public logInvalidVacuumState(byte[] commandline)
            : base(commandline)
        {
        }
    }

    public class logAdcPlaceIonSrc : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "AdcPlaceIonSrc"; }
        }
        public logAdcPlaceIonSrc(byte[] commandline)
            : base(commandline)
        {
        }
    }

    public class logAdcPlaceScanv : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "AdcPlaceScanv"; }
        }
        public logAdcPlaceScanv(byte[] commandline)
            : base(commandline)
        {
        }
    }

    public class logAdcPlaceControlm : AsyncErrorReply
    {
        public override string errorMessage
        {
            get { return "AdcPlaceControlm"; }
        }
        public logAdcPlaceControlm(byte[] commandline)
            : base(commandline)
        {
        }
    }
}
