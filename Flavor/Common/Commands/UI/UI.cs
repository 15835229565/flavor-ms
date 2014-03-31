using Protocol = Flavor.Common.Messaging.ModBusNew;
using CommandCode = Flavor.Common.Messaging.ModBusNew.CommandCode;

namespace Flavor.Common.Messaging.Commands {
    internal abstract class UserRequest: ServicePacket.Sync, ISend {
        #region ISend Members
        public virtual byte[] Data {
            get { return Protocol.collectData((byte)Id); }
        }
        #endregion

        internal class requestState: UserRequest {
            internal override CommandCode Id {
                get { return CommandCode.GetState; }
            }
        }

        internal class requestStatus: UserRequest {
            internal override CommandCode Id {
                get { return CommandCode.GetStatus; }
            }
        }

        internal class sendShutdown: UserRequest {
            internal override CommandCode Id {
                get { return CommandCode.Shutdown; }
            }
        }

        internal class sendInit: UserRequest {
            internal override CommandCode Id {
                get { return CommandCode.Init; }
            }
        }

        internal class sendHCurrent: UserRequest {
            internal override CommandCode Id {
                get { return CommandCode.SetHeatCurrent; }
            }
            #region ISend Members
            public override byte[] Data {
                get { return Protocol.collectData((byte)Id, Config.CommonOptions.hCurrent); }
            }
            #endregion
        }

        internal class sendECurrent: UserRequest {
            internal override CommandCode Id {
                get { return CommandCode.SetEmissionCurrent; }
            }
            #region ISend Members
            public override byte[] Data {
                get { return Protocol.collectData((byte)Id, Config.CommonOptions.eCurrent); }
            }
            #endregion
        }

        internal class sendIVoltage: UserRequest {
            internal override CommandCode Id {
                get { return CommandCode.SetIonizationVoltage; }
            }
            #region ISend Members
            public override byte[] Data {
                get { return Protocol.collectData((byte)Id, Config.CommonOptions.iVoltage); }
            }
            #endregion
        }

        internal class sendF1Voltage: UserRequest {
            internal override CommandCode Id {
                get { return CommandCode.SetFocusVoltage1; }
            }
            #region ISend Members
            public override byte[] Data {
                get { return Protocol.collectData((byte)Id, Config.CommonOptions.fV1); }
            }
            #endregion
        }

        internal class sendF2Voltage: UserRequest {
            internal override CommandCode Id {
                get { return CommandCode.SetFocusVoltage2; }
            }
            #region ISend Members
            public override byte[] Data {
                get { return Protocol.collectData((byte)Id, Config.CommonOptions.fV2); }
            }
            #endregion
        }

        internal class sendSVoltage: UserRequest {
            private ushort SVoltage;

            internal sendSVoltage(ushort step) {
                SVoltage = CommonOptions.scanVoltage(step);
            }

            internal override CommandCode Id {
                get { return CommandCode.SetScanVoltage; }
            }
            #region ISend Members
            public override byte[] Data {
                get { return Protocol.collectData((byte)Id, SVoltage); }
            }
            #endregion
        }

        internal class sendCapacitorVoltage: UserRequest {
            internal override CommandCode Id {
                get { return CommandCode.SetCapacitorVoltage; }
            }
            #region ISend Members
            public override byte[] Data {
                get { return Protocol.collectData((byte)Id, Config.CommonOptions.CP); }
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

            internal override CommandCode Id {
                get { return CommandCode.Measure; }
            }
            #region ISend Members
            public override byte[] Data {
                //!!!
                get {
                    ConsoleWriter.WriteLine("Measure intervals: {0} + {1}", itime * 5, etime * 5);
                    return Protocol.collectData((byte)Id, itime, etime);
                }
            }
            #endregion
        }

        internal class getCounts: UserRequest {
            internal override CommandCode Id {
                get { return CommandCode.GetCounts; }
            }
        }

        internal class enableHCurrent: UserRequest {
            internal override CommandCode Id {
                get { return CommandCode.heatCurrentEnable; }
            }
            #region ISend Members
            public override byte[] Data {
                get { return Protocol.collectData((byte)Id, 0); }
            }
            #endregion
        }

        internal class disableHCurrent: UserRequest {
            internal override CommandCode Id {
                get { return CommandCode.heatCurrentEnable; }
            }
            #region ISend Members
            public override byte[] Data {
                get { return Protocol.collectData((byte)Id, 1); }
            }
            #endregion
        }

        internal class enableHighVoltage: UserRequest {
            private byte HVenable;

            internal enableHighVoltage(bool enable) {
                HVenable = enable ? (byte)1 : (byte)0;
            }

            internal override CommandCode Id {
                get { return CommandCode.EnableHighVoltage; }
            }
            #region ISend Members
            public override byte[] Data {
                get { return Protocol.collectData((byte)Id, HVenable); }
            }
            #endregion
        }

        internal class getTurboPumpStatus: UserRequest {
            internal override CommandCode Id {
                get { return CommandCode.GetTurboPumpStatus; }
            }
        }

        internal class setForvacuumLevel: UserRequest {
            internal override CommandCode Id {
                get { return CommandCode.SetForvacuumLevel; }
            }
        }
    }
}