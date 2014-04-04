﻿using CommandCode = Flavor.Common.Messaging.SevMorGeo.CommandCode;
using UserRequest = Flavor.Common.Messaging.ServicePacket<Flavor.Common.Messaging.SevMorGeo.CommandCode>.UserRequest;

namespace Flavor.Common.Messaging.SevMorGeo {
    internal class requestState: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.GetState; }
        }
        #region ISend Members
        public override byte[] Data {
            get { return ModBus.collectData((byte)Id); }
        }
        #endregion
    }

    internal class requestStatus: UserRequest, IStatusRequest {
        public override CommandCode Id {
            get { return CommandCode.GetStatus; }
        }
        #region ISend Members
        public override byte[] Data {
            get { return ModBus.collectData((byte)Id); }
        }
        #endregion
    }

    internal class sendShutdown: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.Shutdown; }
        }
        #region ISend Members
        public override byte[] Data {
            get { return ModBus.collectData((byte)Id); }
        }
        #endregion
    }

    internal class sendInit: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.Init; }
        }
        #region ISend Members
        public override byte[] Data {
            get { return ModBus.collectData((byte)Id); }
        }
        #endregion
    }

    internal class sendHCurrent: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.SetHeatCurrent; }
        }
        #region ISend Members
        public override byte[] Data {
            get { return ModBus.collectData((byte)Id, Config.CommonOptions.hCurrent); }
        }
        #endregion
    }

    internal class sendECurrent: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.SetEmissionCurrent; }
        }
        #region ISend Members
        public override byte[] Data {
            get { return ModBus.collectData((byte)Id, Config.CommonOptions.eCurrent); }
        }
        #endregion
    }

    internal class sendIVoltage: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.SetIonizationVoltage; }
        }
        #region ISend Members
        public override byte[] Data {
            get { return ModBus.collectData((byte)Id, Config.CommonOptions.iVoltage); }
        }
        #endregion
    }

    internal class sendF1Voltage: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.SetFocusVoltage1; }
        }
        #region ISend Members
        public override byte[] Data {
            get { return ModBus.collectData((byte)Id, Config.CommonOptions.fV1); }
        }
        #endregion
    }

    internal class sendF2Voltage: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.SetFocusVoltage2; }
        }
        #region ISend Members
        public override byte[] Data {
            get { return ModBus.collectData((byte)Id, Config.CommonOptions.fV2); }
        }
        #endregion
    }

    internal class sendSVoltage: UserRequest {
        private ushort SVoltage;

        internal sendSVoltage(ushort step) {
            SVoltage = CommonOptions.scanVoltage(step);
        }

        public override CommandCode Id {
            get { return CommandCode.SetScanVoltage; }
        }
        #region ISend Members
        public override byte[] Data {
            get { return ModBus.collectData((byte)Id, SVoltage); }
        }
        #endregion
    }

    internal class sendCapacitorVoltage: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.SetCapacitorVoltage; }
        }
        #region ISend Members
        public override byte[] Data {
            get { return ModBus.collectData((byte)Id, Config.CommonOptions.CP); }
        }
        #endregion
    }

    internal class sendMeasure: UserRequest {
        private ushort itime;
        private ushort etime;

        internal sendMeasure(ushort iT, ushort eT) {
            itime = iT;
            etime = eT;
        }

        public override CommandCode Id {
            get { return CommandCode.Measure; }
        }
        #region ISend Members
        public override byte[] Data {
            //!!!
            get {
                ConsoleWriter.WriteLine("Measure intervals: {0} + {1}", itime * 5, etime * 5);
                return ModBus.collectData((byte)Id, itime, etime);
            }
        }
        #endregion
    }

    internal class getCounts: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.GetCounts; }
        }
        #region ISend Members
        public override byte[] Data {
            get { return ModBus.collectData((byte)Id); }
        }
        #endregion
    }

    internal class enableHCurrent: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.heatCurrentEnable; }
        }
        #region ISend Members
        public override byte[] Data {
            get { return ModBus.collectData((byte)Id, 0); }
        }
        #endregion
    }

    internal class disableHCurrent: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.heatCurrentEnable; }
        }
        #region ISend Members
        public override byte[] Data {
            get { return ModBus.collectData((byte)Id, 1); }
        }
        #endregion
    }

    internal class enableHighVoltage: UserRequest {
        private byte HVenable;

        internal enableHighVoltage(bool enable) {
            HVenable = enable ? (byte)1 : (byte)0;
        }

        public override CommandCode Id {
            get { return CommandCode.EnableHighVoltage; }
        }
        #region ISend Members
        public override byte[] Data {
            get { return ModBus.collectData((byte)Id, HVenable); }
        }
        #endregion
    }

    internal class getTurboPumpStatus: UserRequest, IStatusRequest {
        public override CommandCode Id {
            get { return CommandCode.GetTurboPumpStatus; }
        }
        #region ISend Members
        public override byte[] Data {
            get { return ModBus.collectData((byte)Id); }
        }
        #endregion
    }

    internal class setForvacuumLevel: UserRequest {
        public override CommandCode Id {
            get { return CommandCode.SetForvacuumLevel; }
        }
        #region ISend Members
        public override byte[] Data {
            get { return ModBus.collectData((byte)Id); }
        }
        #endregion
    }
}