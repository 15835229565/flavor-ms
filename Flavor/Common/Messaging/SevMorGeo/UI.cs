using System.Collections.Generic;
using CommandCode = Flavor.Common.Messaging.SevMorGeo.CommandCode;
using UserRequest = Flavor.Common.Messaging.UserRequest<Flavor.Common.Messaging.SevMorGeo.CommandCode>;

namespace Flavor.Common.Messaging.SevMorGeo {
    class requestState: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.GetState; }
        }
        #region ISend Members
        public override IList<byte> Data {
            get { return ModBus.collectData((byte)Id); }
        }
        #endregion
    }

    class requestStatus: UserRequest, IStatusRequest {
        public override CommandCode Id {
            get { return CommandCode.GetStatus; }
        }
        #region ISend Members
        public override IList<byte> Data {
            get { return ModBus.collectData((byte)Id); }
        }
        #endregion
    }

    class sendShutdown: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.Shutdown; }
        }
        #region ISend Members
        public override IList<byte> Data {
            get { return ModBus.collectData((byte)Id); }
        }
        #endregion
    }

    class sendInit: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.Init; }
        }
        #region ISend Members
        public override IList<byte> Data {
            get { return ModBus.collectData((byte)Id); }
        }
        #endregion
    }

    class sendHCurrent: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.SetHeatCurrent; }
        }
        #region ISend Members
        public override IList<byte> Data {
            get { return ModBus.collectData((byte)Id, Config.CommonOptions.hCurrent); }
        }
        #endregion
    }

    class sendECurrent: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.SetEmissionCurrent; }
        }
        #region ISend Members
        public override IList<byte> Data {
            get { return ModBus.collectData((byte)Id, Config.CommonOptions.eCurrent); }
        }
        #endregion
    }

    class sendIVoltage: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.SetIonizationVoltage; }
        }
        #region ISend Members
        public override IList<byte> Data {
            get { return ModBus.collectData((byte)Id, Config.CommonOptions.iVoltage); }
        }
        #endregion
    }

    class sendF1Voltage: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.SetFocusVoltage1; }
        }
        #region ISend Members
        public override IList<byte> Data {
            get { return ModBus.collectData((byte)Id, Config.CommonOptions.fV1); }
        }
        #endregion
    }

    class sendF2Voltage: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.SetFocusVoltage2; }
        }
        #region ISend Members
        public override IList<byte> Data {
            get { return ModBus.collectData((byte)Id, Config.CommonOptions.fV2); }
        }
        #endregion
    }

    class sendSVoltage: UserRequest {
        ushort SVoltage;

        public sendSVoltage(ushort step) {
            SVoltage = CommonOptions.scanVoltage(step);
        }

        public override CommandCode Id {
            get { return CommandCode.SetScanVoltage; }
        }
        #region ISend Members
        public override IList<byte> Data {
            get { return ModBus.collectData((byte)Id, SVoltage); }
        }
        #endregion
    }

    class sendCapacitorVoltage: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.SetCapacitorVoltage; }
        }
        #region ISend Members
        public override IList<byte> Data {
            get { return ModBus.collectData((byte)Id, Config.CommonOptions.CP); }
        }
        #endregion
    }

    class sendMeasure: UserRequest {
        ushort itime;
        ushort etime;

        public sendMeasure(ushort iT, ushort eT) {
            itime = iT;
            etime = eT;
        }

        public override CommandCode Id {
            get { return CommandCode.Measure; }
        }
        #region ISend Members
        public override IList<byte> Data {
            //!!!
            get {
                ConsoleWriter.WriteLine("Measure intervals: {0} + {1}", itime * 5, etime * 5);
                return ModBus.collectData((byte)Id, itime, etime);
            }
        }
        #endregion
    }

    class getCounts: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.GetCounts; }
        }
        #region ISend Members
        public override IList<byte> Data {
            get { return ModBus.collectData((byte)Id); }
        }
        #endregion
    }

    class enableHCurrent: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.heatCurrentEnable; }
        }
        #region ISend Members
        public override IList<byte> Data {
            get { return ModBus.collectData((byte)Id, 0); }
        }
        #endregion
    }

    class disableHCurrent: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.heatCurrentEnable; }
        }
        #region ISend Members
        public override IList<byte> Data {
            get { return ModBus.collectData((byte)Id, 1); }
        }
        #endregion
    }

    class enableHighVoltage: UserRequest {
        byte HVenable;

        public enableHighVoltage(bool enable) {
            HVenable = enable ? (byte)1 : (byte)0;
        }

        public override CommandCode Id {
            get { return CommandCode.EnableHighVoltage; }
        }
        #region ISend Members
        public override IList<byte> Data {
            get { return ModBus.collectData((byte)Id, HVenable); }
        }
        #endregion
    }

    class getTurboPumpStatus: UserRequest, IStatusRequest {
        public override CommandCode Id {
            get { return CommandCode.GetTurboPumpStatus; }
        }
        #region ISend Members
        public override IList<byte> Data {
            get { return ModBus.collectData((byte)Id); }
        }
        #endregion
    }

    internal class setForvacuumLevel: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.SetForvacuumLevel; }
        }
        #region ISend Members
        public override IList<byte> Data {
            get { return ModBus.collectData((byte)Id); }
        }
        #endregion
    }
}