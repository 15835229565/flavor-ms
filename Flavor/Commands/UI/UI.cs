using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Common.Commands.Interfaces;

namespace Flavor.Common.Commands.UI
{
    public class UserRequest: SyncServicePacket, ISend
    {
        #region ISend Members
        public virtual void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)Id)));
        }
        #endregion
    }

    public class requestState : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.GetState; }
        }
    }

    public class requestStatus : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.GetStatus; }
        }
    }

    public class sendShutdown : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.Shutdown; }
        }
    }

    public class sendInit : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.Init; }
        }
    }

    public class sendHCurrent : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.SetHeatCurrent; }
        }
        #region ISend Members

        public override void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.SetHeatCurrent, Config.CommonOptions.hCurrent)));
        }

        #endregion
    }

    public class sendECurrent : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.SetEmissionCurrent; }
        }
        #region ISend Members
        public override void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.SetEmissionCurrent, Config.CommonOptions.eCurrent)));
        }
        #endregion
    }

    public class sendIVoltage : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.SetIonizationVoltage; }
        }
        #region ISend Members
        public override void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.SetIonizationVoltage, Config.CommonOptions.iVoltage)));
        }
        #endregion
    }

    public class sendF1Voltage : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.SetFocusVoltage1; }
        }
        #region ISend Members
        public override void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.SetFocusVoltage1, Config.CommonOptions.fV1)));
        }
        #endregion
    }

    public class sendF2Voltage : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.SetFocusVoltage2; }
        }

        #region ISend Members
        public override void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.SetFocusVoltage2, Config.CommonOptions.fV2)));
        }
        #endregion
    }

    public class sendSVoltage : UserRequest
    {
        private ushort SVoltage;

        public sendSVoltage(ushort step)
        {
            SVoltage = Config.CommonOptions.scanVoltage(step);
        }
        
        /*
        public sendSVoltage(ushort step, bool isSenseMeasure)
        {
            if (isSenseMeasure)
                if (step > 4095) SVoltage = 4095;
                else SVoltage = step;
            else SVoltage = Config.scanVoltage(step);
        }
        */
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.SetScanVoltage; }
        }
        #region ISend Members
        public override void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.SetScanVoltage, SVoltage)));
        }
        #endregion
    }

    public class sendCapacitorVoltage : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.SetCapacitorVoltage; }
        }

        #region ISend Members
        public override void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.SetCapacitorVoltage, Config.CommonOptions.CP)));
        }
        #endregion
    }
    
    public class sendMeasure : UserRequest
    {
        private ushort itime;
        private ushort etime;

        public sendMeasure()
        {
            itime = Config.CommonOptions.iTime;
            etime = Config.CommonOptions.eTime;
        }
        public sendMeasure(ushort iT, ushort eT)
        {
            itime = iT;
            etime = eT;
        }

        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.Measure; }
        }

        #region ISend Members
        public override void Send()
        {
            Console.WriteLine("Measure intervals: {0} + {1}", itime * 5, etime * 5);
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.Measure, itime, etime)));
            //ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.Measure, Config.iTime, Config.eTime)));
        }
        #endregion
    }

    public class getCounts : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.GetCounts; }
        }
    }

    public class enableHCurrent : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.heatCurrentEnable; }
        }

        #region ISend Members
        public override void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.heatCurrentEnable, 0)));
        }
        #endregion
    }
    
    public class disableHCurrent : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.heatCurrentEnable; }
        }

        #region ISend Members
        public override void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.heatCurrentEnable, 1)));
        }
        #endregion
    }
/*    
    class enableECurrent : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.emissionCurrentEnable; }
        }

        #region ISend Members
        public override void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.emissionCurrentEnable, 0)));
        }
        #endregion
    }
    
    class disableECurrent : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.emissionCurrentEnable; }
        }

        #region ISend Members
        public override void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.emissionCurrentEnable, 1)));
        }
        #endregion
    }
*/    
    public class enableHighVoltage : UserRequest
    {
        private byte HVenable;
        
        public enableHighVoltage(bool enable)
        {
            HVenable = 0;
            if (enable) HVenable = 1;
        }
        
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.EnableHighVoltage; }
        }
        
        #region ISend Members
        public override void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.EnableHighVoltage, HVenable)));
        }
        #endregion
    }
    
    public class getTurboPumpStatus : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.GetTurboPumpStatus; }
        }
    }
    
    public class setForvacuumLevel : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.SetForvacuumLevel; }
        }
    }
}