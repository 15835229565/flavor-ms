using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor
{
    class UserRequest: SyncServicePacket, ISend
    {
        #region ISend Members
        public virtual void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)Id)));
        }
        #endregion
    }

    class requestState : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.GetState; }
        }
    }
    
    class requestStatus : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.GetStatus; }
        }
    }
    
    class sendShutdown : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.Shutdown; }
        }
    }
    
    class sendInit : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.Init; }
        }
    }
    
    class sendHCurrent : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.SetHeatCurrent; }
        }
        #region ISend Members

        public override void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.SetHeatCurrent, Config.hCurrent)));
        }

        #endregion
    }
    
    class sendECurrent : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.SetEmissionCurrent; }
        }
        #region ISend Members
        public override void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.SetEmissionCurrent, Config.eCurrent)));
        }
        #endregion
    }
    
    class sendIVoltage : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.SetIonizationVoltage; }
        }
        #region ISend Members
        public override void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.SetIonizationVoltage, Config.iVoltage)));
        }
        #endregion
    }
    
    class sendF1Voltage : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.SetFocusVoltage1; }
        }
        #region ISend Members
        public override void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.SetFocusVoltage1, Config.fV1)));
        }
        #endregion
    }
    
    class sendF2Voltage : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.SetFocusVoltage2; }
        }

        #region ISend Members
        public override void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.SetFocusVoltage2, Config.fV2)));
        }
        #endregion
    }
    
    class sendSVoltage : UserRequest
    {
        private ushort SVoltage;

        public sendSVoltage(ushort step)
        {
            SVoltage = Config.scanVoltage(step);
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
    
    class sendCapacitorVoltage : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.SetCapacitorVoltage; }
        }

        #region ISend Members
        public override void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.SetCapacitorVoltage, Config.CP)));
        }
        #endregion
    }
    
    class sendMeasure : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.Measure; }
        }

        #region ISend Members
        public override void Send()
        {
            Console.WriteLine("Измеряем в течение {0}+{1}", Config.iTimeReal, Config.eTimeReal);
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)ModBus.CommandCode.Measure, Config.iTime, Config.eTime)));
        }
        #endregion
    }

    class getCounts : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.GetCounts; }
        }
    }

    class enableHCurrent : UserRequest
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
    
    class disableHCurrent : UserRequest
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
    class enableHighVoltage : UserRequest
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
    
    class getTurboPumpStatus : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.GetTurboPumpStatus; }
        }
    }
    
    class setForvacuumLevel : UserRequest
    {
        public override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.SetForvacuumLevel; }
        }
    }
}