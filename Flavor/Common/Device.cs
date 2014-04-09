using System;

namespace Flavor.Common {
    delegate void DeviceEventHandler();
    delegate void TurboPumpAlertEventHandler(bool isFault, byte bits);

    static class Device {
        public static event DeviceEventHandler OnDeviceStateChanged;
        public static event DeviceEventHandler OnDeviceStatusChanged;
        public static event DeviceEventHandler OnVacuumStateChanged;
        public static event DeviceEventHandler OnTurboPumpStatusChanged;
        public static event TurboPumpAlertEventHandler OnTurboPumpAlert;

        public enum DeviceStates: byte {
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

        public enum VacuumStates: byte {
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

        static DeviceStates systemState;
        static VacuumStates vacuumState;

        static bool forPumpOn;
        static bool pValve;
        static bool hvValve;

        static bool turboPumpOn;
        static bool trFault;

        static bool hvOn;

        static bool heatCurrentEnable;

        static ushort forVacuumValue;
        static ushort hVacuumValue;

        static int Detector1Value;
        static int Detector2Value;

        public static DeviceStates sysState {
            get { return systemState; }
            set {
                if (systemState != value) {
                    systemState = value;
                    OnDeviceStateChanged();
                    if (systemState == DeviceStates.TurboPumpFailure) {
                        // log!
                    }
                    if (systemState == DeviceStates.VacuumCrash) {
                        // log!
                    }
                };
            }
        }

        public static VacuumStates vacState {
            get { return vacuumState; }
            set {
                if (vacuumState != value) {
                    vacuumState = value;
                    OnVacuumStateChanged();
                }
            }
        }

        public static bool fPumpOn {
            get { return forPumpOn; }
            private set { forPumpOn = value; }
        }

        public static bool probeValve {
            get { return pValve; }
            private set { pValve = value; }
        }

        public static bool highVacuumValve {
            get { return hvValve; }
            private set { hvValve = value; }
        }

        public static bool tPumpOn {
            get { return turboPumpOn; }
            private set { turboPumpOn = value; }
        }

        public static bool turboReplyFault {
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

        public static bool highVoltageOn {
            get { return hvOn; }
            private set { hvOn = value; }
        }

        public static bool hCurrentEnable {
            get { return heatCurrentEnable; }
            private set { heatCurrentEnable = value; }
        }

        public static ushort fVacuum {
            get { return forVacuumValue; }
            set { forVacuumValue = value; }
        }
        public static double fVacuumReal {
            get { return 2 * 5 * (double)fVacuum / 4096; }
        }

        public static ushort hVacuum {
            get { return hVacuumValue; }
            set { hVacuumValue = value; }
        }
        public static double hVacuumReal {
            get { return 2 * 5 * (double)hVacuum / 4096; }
        }

        public static EventHandler<EventArgs<int[]>> CountsUpdated;
        static void OnCountsUpdated() {
            CountsUpdated.Raise(null, new EventArgs<int[]>(Detectors));
        }
        // TODO: configurable
        static int[] detectors = new int[2];
        public static int[] Detectors {
            get { return (int[])detectors.Clone(); }
            set {
                if (value == null || value.Length != detectors.Length)
                    return;
                value.CopyTo(detectors, 0);
            }
        }

        static DevCommonData deviceCommonData = new DevCommonData();
        public static DevCommonData DeviceCommonData {
            get { return deviceCommonData; }
        }
        public class DevCommonData: CommonData {
            private ushort condVoltagePlus;
            private ushort condVoltageMin;

            private ushort detectorVoltage;
            private ushort scanVoltage;

            internal ushort cVPlus {
                set { condVoltagePlus = value; }
            }
            internal double cVPlusReal {
                get { return 120 * 5 * (double)condVoltagePlus / 4096; }
            }
            internal ushort cVMin {
                set { condVoltageMin = value; }
            }
            internal double cVMinReal {
                get { return 100 * 5 * (double)condVoltageMin / 4096; }
            }

            internal ushort dVoltage {
                set { detectorVoltage = value; }
            }
            internal double dVoltageReal {
                get { return 5 * (double)detectorVoltage / (4096 * 0.001); }
            }

            internal ushort sVoltage {
                set { scanVoltage = value; }
            }
            internal double sVoltageReal {
                get { return 5 * (double)scanVoltage / (4096 * 0.0008); }
            }
        }

        public struct TurboPump {
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

        public static void relaysState(byte value) {
            fPumpOn = Convert.ToBoolean(value & 1);
            tPumpOn = Convert.ToBoolean(value & 1 << 1);
            probeValve = Convert.ToBoolean(value & 1 << 2);
            highVacuumValve = Convert.ToBoolean(value & 1 << 3);
            highVoltageOn = Convert.ToBoolean(value & 1 << 4);
            hCurrentEnable = Convert.ToBoolean(value & 1 << 5);
            turboReplyFault = Convert.ToBoolean(value & 1 << 6);

            OnDeviceStatusChanged();
        }

        public static void Init() {
            Device.sysState = DeviceStates.Start;
            Device.vacState = VacuumStates.Idle;
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
