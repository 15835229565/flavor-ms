using System;

namespace Flavor.Common {
    delegate void DeviceEventHandler();
    delegate void TurboPumpAlertEventHandler(bool isFault, byte bits);

    static class Device {
        internal static event DeviceEventHandler OnDeviceStateChanged;
        internal static event DeviceEventHandler OnDeviceStatusChanged;
        internal static event DeviceEventHandler OnVacuumStateChanged;
        internal static event DeviceEventHandler OnTurboPumpStatusChanged;
        internal static event TurboPumpAlertEventHandler OnTurboPumpAlert;

        internal enum DeviceStates: byte {
            Start = 0,
            Init,
            VacuumInit,
            WaitHighVoltage,
            Ready,
            Measuring,
            Measured,
            ShutdownInit,
            Shutdowning,
            Shutdowned,
            TurboPumpFailure = 16,
            VacuumCrash = 17,
            ConstantsWrite = 32
        }

        internal enum VacuumStates: byte {
            Idle = 0x00,
            Init = 0x01,
            StartingForvacuumPump = 0x02,
            PumpingForvacuum = 0x03,
            DelayPumpingHighVacuumByForvac = 0x04,
            PumpingHighVacuumByForvac = 0x05,
            PumpingHighVacuumByTurbo = 0x06,
            Ready = 0x07,

            ShutdownInit = 0x10,
            ShutdownDelay = 0x11,
            ShutdownPumpProbe = 0x12,
            Shutdowned = 0x13,
            ShutdownStartingTurboPump = 0x14,

            BadHighVacuum = 0x20,
            BadForvacuum = 0x21,
            ForvacuumFailure = 0x22,
            LargeLeak = 0x23,
            SmallLeak = 0x24,
            ThermoCoupleFailure = 0x25,
            TurboPumpFailure = 0x26,

            VacuumShutdownProbeLeak = 0x28
        }

        private static byte systemState;
        private static byte vacuumState;

        private static bool forPumpOn;
        private static bool pValve;
        private static bool hvValve;

        private static bool turboPumpOn;
        private static bool trFault;

        private static bool hvOn;

        private static bool heatCurrentEnable;
        //private static bool emissionCurrentEnable;

        private static ushort forVacuumValue;
        private static ushort hVacuumValue;

        private static int Detector1Value;
        private static int Detector2Value;

        internal static byte sysState {
            get { return systemState; }
            set {
                if (systemState != value) {
                    systemState = value;
                    OnDeviceStateChanged();
                    if (systemState == (byte)DeviceStates.TurboPumpFailure) {
                        // log!
                    }
                    if (systemState == (byte)DeviceStates.VacuumCrash) {
                        // log!
                    }
                };
            }
        }

        internal static byte vacState {
            get { return vacuumState; }
            set {
                if (vacuumState != value) {
                    vacuumState = value;
                    OnVacuumStateChanged();
                }
            }
        }

        internal static bool fPumpOn {
            get { return forPumpOn; }
            private set { forPumpOn = value; }
        }

        internal static bool probeValve {
            get { return pValve; }
            private set { pValve = value; }
        }

        internal static bool highVacuumValve {
            get { return hvValve; }
            private set { hvValve = value; }
        }

        internal static bool tPumpOn {
            get { return turboPumpOn; }
            private set { turboPumpOn = value; }
        }

        internal static bool turboReplyFault {
            get { return trFault; }
            private set {
                if (value != trFault) {
                    trFault = value;
                    if (trFault != false) {
                        OnTurboPumpAlert(true, 0);
                    }
                }
            }
        }

        internal static bool highVoltageOn {
            get { return hvOn; }
            private set { hvOn = value; }
        }

        internal static bool hCurrentEnable {
            get { return heatCurrentEnable; }
            private set { heatCurrentEnable = value; }
        }

        internal static ushort fVacuum {
            get { return forVacuumValue; }
            set { forVacuumValue = value; }
        }
        internal static double fVacuumReal {
            get { return 2 * 5 * (double)fVacuum / 4096; }
        }

        internal static ushort hVacuum {
            get { return hVacuumValue; }
            set { hVacuumValue = value; }
        }
        internal static double hVacuumReal {
            get { return 2 * 5 * (double)hVacuum / 4096; }
        }

        internal static int Detector1 {
            get { return Detector1Value; }
            set { Detector1Value = value; }
        }
        internal static int Detector2 {
            get { return Detector2Value; }
            set { Detector2Value = value; }
        }

        internal struct DeviceCommonData {
            private static ushort heatCurrent;
            private static ushort emissionCurrent;

            private static ushort ionizatinVoltage;

            private static ushort focusVoltage1;
            private static ushort focusVoltage2;

            private static ushort scanVoltage;

            private static ushort condVoltagePlus;
            private static ushort condVoltageMin;

            private static ushort CPValue;

            private static ushort detectorVoltage;

            internal static ushort hCurrent {
                get { return heatCurrent; }
                set { heatCurrent = value; }
            }
            internal static double hCurrentReal {
                get { return (double)hCurrent / 4096; }
            }

            internal static ushort eCurrent {
                get { return emissionCurrent; }
                set { emissionCurrent = value; }
            }
            internal static double eCurrentReal {
                get { return 50 * (double)eCurrent / 4096; }
            }

            internal static ushort iVoltage {
                get { return ionizatinVoltage; }
                set { ionizatinVoltage = value; }
            }
            internal static double iVoltageReal {
                get { return 150 * (double)iVoltage / 4096; }
            }

            internal static ushort fV1 {
                get { return focusVoltage1; }
                set { focusVoltage1 = value; }
            }
            internal static double fV1Real {
                get { return 150 * (double)fV1 / 4096; }
            }
            internal static ushort fV2 {
                get { return focusVoltage2; }
                set { focusVoltage2 = value; }
            }
            internal static double fV2Real {
                get { return 150 * (double)fV2 / 4096; }
            }

            internal static ushort sVoltage {
                get { return scanVoltage; }
                set { scanVoltage = value; }
            }
            internal static double sVoltageReal {
                get { return 5 * (double)sVoltage / (4096 * 0.0008); }
            }
            internal static ushort cVPlus {
                get { return condVoltagePlus; }
                set { condVoltagePlus = value; }
            }
            internal static double cVPlusReal {
                get { return 120 * 5 * (double)cVPlus / 4096; }
            }
            internal static ushort cVMin {
                get { return condVoltageMin; }
                set { condVoltageMin = value; }
            }
            internal static double cVMinReal {
                get { return 100 * 5 * (double)cVMin / 4096; }
            }

            internal static ushort CP {
                get { return CPValue; }
                set { CPValue = value; }
            }

            internal static ushort dVoltage {
                get { return detectorVoltage; }
                set { detectorVoltage = value; }
            }
            internal static double dVoltageReal {
                get { return 5 * (double)dVoltage / (4096 * 0.001); }
            }
        }

        internal struct TurboPump {
            private static ushort tpSpeed;
            internal static ushort Speed {
                get { return tpSpeed; }
                set { tpSpeed = value; }
            }
            private static ushort tpCurrent;
            internal static ushort Current {
                get { return tpCurrent; }
                set { tpCurrent = value; }
            }
            private static ushort pwm_;
            internal static ushort pwm {
                get { return pwm_; }
                set { pwm_ = value; }
            }
            private static ushort tpTemp;
            internal static ushort PumpTemperature {
                get { return tpTemp; }
                set { tpTemp = value; }
            }
            private static ushort drTemp;
            internal static ushort DriveTemperature {
                get { return drTemp; }
                set { drTemp = value; }
            }
            private static ushort opTime;
            internal static ushort OperationTime {
                get { return opTime; }
                set { opTime = value; }
            }

            private static byte statusBits, alertBits, faultBits;
            internal static byte StatusBits {
                get { return statusBits; }
                private set { statusBits = value; }
            }
            internal static byte AlertBits {
                get { return alertBits; }
                private set {
                    if (value != alertBits) {
                        alertBits = value;
                        if (alertBits != 0) {
                            OnTurboPumpAlert(false, alertBits);
                        }
                    }
                }
            }
            internal static byte FaultBits {
                get { return faultBits; }
                private set {
                    if (value != faultBits) {
                        faultBits = value;
                        if (faultBits != 0) {
                            OnTurboPumpAlert(true, faultBits);
                        }
                    }
                }
            }
            internal static void relaysState(byte value, byte value2, byte value3) {
                StatusBits = value;
                AlertBits = value2;
                FaultBits = value3;
                OnTurboPumpStatusChanged();
            }

            internal static void Init() {
                tpSpeed = 0;
                tpCurrent = 0;
                pwm_ = 0;
                tpTemp = 0;
                drTemp = 0;
                opTime = 0;
                relaysState(0, 0, 0);
            }
        }

        internal static void relaysState(byte value) {
            fPumpOn = Convert.ToBoolean(value & 1);
            tPumpOn = Convert.ToBoolean(value & 1 << 1);
            probeValve = Convert.ToBoolean(value & 1 << 2);
            highVacuumValve = Convert.ToBoolean(value & 1 << 3);
            highVoltageOn = Convert.ToBoolean(value & 1 << 4);
            hCurrentEnable = Convert.ToBoolean(value & 1 << 5);
            turboReplyFault = Convert.ToBoolean(value & 1 << 6);

            Commander.hBlock = !highVoltageOn;//!!!
            OnDeviceStatusChanged();
        }

        internal static void Init() {
            Device.sysState = 255;
            Device.vacState = 255;
            Device.fVacuum = 0;
            Device.hVacuum = 0;
            Device.DeviceCommonData.hCurrent = 0;
            Device.DeviceCommonData.eCurrent = 0;
            Device.DeviceCommonData.iVoltage = 0;
            Device.DeviceCommonData.fV1 = 0;
            Device.DeviceCommonData.fV2 = 0;
            Device.DeviceCommonData.sVoltage = 0;
            Device.DeviceCommonData.cVPlus = 0;
            Device.DeviceCommonData.cVMin = 0;
            Device.DeviceCommonData.dVoltage = 0;
            Device.relaysState(0);
            Device.TurboPump.Init();
        }
    }
}
