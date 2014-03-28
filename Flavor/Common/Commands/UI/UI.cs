namespace Flavor.Common.Messaging.Commands {
    internal abstract class UserRequest: ServicePacket.Sync, ISend {
        #region ISend Members
        public void Send() {
            ModBus.Send(Data());
        }
        protected virtual byte[] Data() {
            return ModBus.collectData((byte)Id);
        }
        #endregion

        internal class requestState: UserRequest {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.GetState; }
            }
        }

        internal class requestStatus: UserRequest {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.GetStatus; }
            }
        }

        internal class sendShutdown: UserRequest {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.Shutdown; }
            }
        }

        internal class sendInit: UserRequest {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.Init; }
            }
        }

        internal class sendHCurrent: UserRequest {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.SetHeatCurrent; }
            }
            #region ISend Members
            protected override byte[] Data() {
                return ModBus.collectData((byte)ModBus.CommandCode.SetHeatCurrent, Config.CommonOptions.hCurrent);
            }
            #endregion
        }

        internal class sendECurrent: UserRequest {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.SetEmissionCurrent; }
            }
            #region ISend Members
            protected override byte[] Data() {
                return ModBus.collectData((byte)ModBus.CommandCode.SetEmissionCurrent, Config.CommonOptions.eCurrent);
            }
            #endregion
        }

        internal class sendIVoltage: UserRequest {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.SetIonizationVoltage; }
            }
            #region ISend Members
            protected override byte[] Data() {
                return ModBus.collectData((byte)ModBus.CommandCode.SetIonizationVoltage, Config.CommonOptions.iVoltage);
            }
            #endregion
        }

        internal class sendF1Voltage: UserRequest {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.SetFocusVoltage1; }
            }
            #region ISend Members
            protected override byte[] Data() {
                return ModBus.collectData((byte)ModBus.CommandCode.SetFocusVoltage1, Config.CommonOptions.fV1);
            }
            #endregion
        }

        internal class sendF2Voltage: UserRequest {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.SetFocusVoltage2; }
            }
            #region ISend Members
            protected override byte[] Data() {
                return ModBus.collectData((byte)ModBus.CommandCode.SetFocusVoltage2, Config.CommonOptions.fV2);
            }
            #endregion
        }

        internal class sendSVoltage: UserRequest {
            private ushort SVoltage;

            internal sendSVoltage(ushort step) {
                SVoltage = CommonOptions.scanVoltage(step);
            }

            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.SetScanVoltage; }
            }
            #region ISend Members
            protected override byte[] Data() {
                return ModBus.collectData((byte)ModBus.CommandCode.SetScanVoltage, SVoltage);
            }
            #endregion
        }

        internal class sendCapacitorVoltage: UserRequest {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.SetCapacitorVoltage; }
            }
            #region ISend Members
            protected override byte[] Data() {
                return ModBus.collectData((byte)ModBus.CommandCode.SetCapacitorVoltage, Config.CommonOptions.CP);
            }
            #endregion
        }

        internal class sendMeasure: UserRequest {
            private ushort itime;
            private ushort etime;

            /*internal sendMeasure() {
                itime = Config.CommonOptions.iTime;
                etime = Config.CommonOptions.eTime;
            }*/
            internal sendMeasure(ushort iT, ushort eT) {
                itime = iT;
                etime = eT;
            }

            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.Measure; }
            }
            #region ISend Members
            protected override byte[] Data() {
                //!!!
                ConsoleWriter.WriteLine("Measure intervals: {0} + {1}", itime * 5, etime * 5);
                return ModBus.collectData((byte)ModBus.CommandCode.Measure, itime, etime);
            }
            #endregion
        }

        internal class getCounts: UserRequest {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.GetCounts; }
            }
        }

        internal class enableHCurrent: UserRequest {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.heatCurrentEnable; }
            }
            #region ISend Members
            protected override byte[] Data() {
                return ModBus.collectData((byte)ModBus.CommandCode.heatCurrentEnable, 0);
            }
            #endregion
        }

        internal class disableHCurrent: UserRequest {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.heatCurrentEnable; }
            }
            #region ISend Members
            protected override byte[] Data() {
                return ModBus.collectData((byte)ModBus.CommandCode.heatCurrentEnable, 1);
            }
            #endregion
        }

        internal class enableHighVoltage: UserRequest {
            private byte HVenable;

            internal enableHighVoltage(bool enable) {
                HVenable = enable ? (byte)1 : (byte)0;
            }

            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.EnableHighVoltage; }
            }
            #region ISend Members
            protected override byte[] Data() {
                return ModBus.collectData((byte)ModBus.CommandCode.EnableHighVoltage, HVenable);
            }
            #endregion
        }

        internal class getTurboPumpStatus: UserRequest {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.GetTurboPumpStatus; }
            }
        }

        internal class setForvacuumLevel: UserRequest {
            internal override ModBus.CommandCode Id {
                get { return ModBus.CommandCode.SetForvacuumLevel; }
            }
        }
    }
}