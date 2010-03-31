using System;
using System.Collections.Generic;
using System.Text;
using Flavor.Common.Commands.Interfaces;

namespace Flavor.Common.Commands.UI
{
    internal class UserRequest: SyncServicePacket, ISend
    {
        #region ISend Members
        public virtual void Send()
        {
            ModBus.Send(ModBus.buildPack(ModBus.collectData((byte)Id)));
        }
        #endregion
    }

    internal class requestState : UserRequest
    {
        internal override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.GetState; }
        }
    }

    internal class requestStatus : UserRequest
    {
        internal override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.GetStatus; }
        }
    }

    internal class sendShutdown : UserRequest
    {
        internal override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.Shutdown; }
        }
    }

    internal class sendInit : UserRequest
    {
        internal override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.Init; }
        }
    }

    internal class sendHCurrent : UserRequest
    {
        internal override ModBus.CommandCode Id
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

    internal class sendECurrent : UserRequest
    {
        internal override ModBus.CommandCode Id
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

    internal class sendIVoltage : UserRequest
    {
        internal override ModBus.CommandCode Id
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

    internal class sendF1Voltage : UserRequest
    {
        internal override ModBus.CommandCode Id
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

    internal class sendF2Voltage : UserRequest
    {
        internal override ModBus.CommandCode Id
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

    internal class sendSVoltage : UserRequest
    {
        private ushort SVoltage;

        internal sendSVoltage(ushort step)
        {
            SVoltage = Config.CommonOptions.scanVoltage(step);
        }
        
        /*
        internal sendSVoltage(ushort step, bool isSenseMeasure)
        {
            if (isSenseMeasure)
                if (step > 4095) SVoltage = 4095;
                else SVoltage = step;
            else SVoltage = Config.scanVoltage(step);
        }
        */
        internal override ModBus.CommandCode Id
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

    internal class sendCapacitorVoltage : UserRequest
    {
        internal override ModBus.CommandCode Id
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
    
    internal class sendMeasure : UserRequest
    {
        private ushort itime;
        private ushort etime;

        internal sendMeasure()
        {
            itime = Config.CommonOptions.iTime;
            etime = Config.CommonOptions.eTime;
        }
        internal sendMeasure(ushort iT, ushort eT)
        {
            itime = iT;
            etime = eT;
        }

        internal override ModBus.CommandCode Id
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

    internal class getCounts : UserRequest
    {
        internal override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.GetCounts; }
        }
    }

    internal class enableHCurrent : UserRequest
    {
        internal override ModBus.CommandCode Id
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
    
    internal class disableHCurrent : UserRequest
    {
        internal override ModBus.CommandCode Id
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
        internal override ModBus.CommandCode Id
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
        internal override ModBus.CommandCode Id
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
    internal class enableHighVoltage : UserRequest
    {
        private byte HVenable;
        
        internal enableHighVoltage(bool enable)
        {
            HVenable = enable? (byte)1: (byte)0;
        }
        
        internal override ModBus.CommandCode Id
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
    
    internal class getTurboPumpStatus : UserRequest
    {
        internal override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.GetTurboPumpStatus; }
        }
    }
    
    internal class setForvacuumLevel : UserRequest
    {
        internal override ModBus.CommandCode Id
        {
            get { return ModBus.CommandCode.SetForvacuumLevel; }
        }
    }
}