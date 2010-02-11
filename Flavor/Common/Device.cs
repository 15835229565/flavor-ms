using System;
using System.Collections.Generic;
using System.Text;

namespace Flavor.Common
{
    delegate void DeviceEventHandler();
    //delegate void DeviceStatusChangedEventHandler();
    //delegate void VacuumStateChangedEventHandler();
    delegate void TurboPumpAlertEventHandler(bool isFault, byte bits);

    static class Device
    {
        public static event DeviceEventHandler OnDeviceStateChanged;
        public static event DeviceEventHandler OnDeviceStatusChanged;
        public static event DeviceEventHandler OnVacuumStateChanged;
        public static event DeviceEventHandler OnTurboPumpStatusChanged;
        public static event TurboPumpAlertEventHandler OnTurboPumpAlert;

        public enum DeviceStates : byte
        {
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

        public enum VacuumStates : byte
        {
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

        public const float fVoltageC = 0.048828f;
        public const float iVoltageC = 0.024414f;
        public const float eCurrentC = 0.012207f;
        public const float hCurrentC = 0.000488f;
        public const float VacuumC = 0.009869f;
        public const float CCPNC = 0.047778f;
        public const float sVoltageC = 0.894043f; // онтроль сканирующего, на запись другой
        public const float dVoltageC = 1.220703f;

        public static byte sysState
        {
            get { return systemState; }
            set 
            {
                if (systemState != value)
                {
                    systemState = value;
                    OnDeviceStateChanged();
                    if (systemState == (byte)DeviceStates.TurboPumpFailure)
                    {
                        // log!
                    }
                    if (systemState == (byte)DeviceStates.VacuumCrash)
                    {
                        // log!
                    }
                };
            }
        }

        public static byte vacState
        {
            get { return vacuumState; }
            set 
            {
                if (vacuumState != value)
                {
                    vacuumState = value;
                    OnVacuumStateChanged();
                }
            }
        }

        public static bool fPumpOn
        {
            get { return forPumpOn; }
            private set { forPumpOn = value; }
        }

        public static bool probeValve
        {
            get { return pValve; }
            private set { pValve = value; }
        }

        public static bool highVacuumValve
        {
            get { return hvValve; }
            private set { hvValve = value; }
        }

        public static bool tPumpOn
        {
            get { return turboPumpOn; }
            private set { turboPumpOn = value; }
        }

        public static bool turboReplyFault
        {
            get { return trFault; }
            private set
            {
                if (value != trFault)
                {
                    trFault = value;
                    if (trFault != false)
                    {
                        OnTurboPumpAlert(true, 0);
                    }
                }
            }
        }

        public static bool highVoltageOn
        {
            get { return hvOn; }
            private set { hvOn = value; }
        }
        /*public static bool eCurrentEnable
        {
            get { return emissionCurrentEnable; }
            set { emissionCurrentEnable = value; }
        }*/

        public static bool hCurrentEnable
        {
            get { return heatCurrentEnable; }
            private set { heatCurrentEnable = value; }
        }
        
        public static ushort fVacuum
        {
            get { return forVacuumValue; }
            set { forVacuumValue = value; }
        }
        public static double fVacuumReal
        {
            get { return 2 * 5 * (double)fVacuum / 4096; }
        }
        
        public static ushort hVacuum
        {
            get { return hVacuumValue; }
            set { hVacuumValue = value; }
        }
        public static double hVacuumReal
        {
            get { return 2 * 5 * (double)hVacuum / 4096; }
        }

        public static int Detector1
        {
            get { return Detector1Value; }
            set { Detector1Value = value; }
        }
        public static int Detector2
        {
            get { return Detector2Value; }
            set { Detector2Value = value; }
        }

        public struct DeviceCommonData
        {
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

            public static ushort hCurrent
            {
                get { return heatCurrent; }
                set { heatCurrent = value; }
            }
            public static double hCurrentReal
            {
                get { return (double)hCurrent / 4096; }
            }

            public static ushort eCurrent
            {
                get { return emissionCurrent; }
                set { emissionCurrent = value; }
            }
            public static double eCurrentReal
            {
                get { return 50 * (double)eCurrent / 4096; }
            }

            public static ushort iVoltage
            {
                get { return ionizatinVoltage; }
                set { ionizatinVoltage = value; }
            }
            public static double iVoltageReal
            {
                get { return 150 * (double)iVoltage / 4096; }
            }

            public static ushort fV1
            {
                get { return focusVoltage1; }
                set { focusVoltage1 = value; }
            }
            public static double fV1Real
            {
                get { return 150 * (double)fV1 / 4096; }
            }
            public static ushort fV2
            {
                get { return focusVoltage2; }
                set { focusVoltage2 = value; }
            }
            public static double fV2Real
            {
                get { return 150 * (double)fV2 / 4096; }
            }

            public static ushort sVoltage
            {
                get { return scanVoltage; }
                set { scanVoltage = value; }
            }
            public static double sVoltageReal
            {
                get { return 5 * (double)sVoltage / (4096 * 0.0008); }
            }
            public static ushort cVPlus
            {
                get { return condVoltagePlus; }
                set { condVoltagePlus = value; }
            }
            public static double cVPlusReal
            {
                get { return 120 * 5 * (double)cVPlus / 4096; }
            }
            public static ushort cVMin
            {
                get { return condVoltageMin; }
                set { condVoltageMin = value; }
            }
            public static double cVMinReal
            {
                get { return 100 * 5 * (double)cVMin / 4096; }
            }

            public static ushort CP
            {
                get { return CPValue; }
                set { CPValue = value; }
            }

            public static ushort dVoltage
            {
                get { return detectorVoltage; }
                set { detectorVoltage = value; }
            }
            public static double dVoltageReal
            {
                get { return 5 * (double)dVoltage / (4096 * 0.001); }
            }
        }

        public struct TurboPump 
        {
            private static ushort tpSpeed;
            public static ushort Speed
            {
                get { return tpSpeed; }
                set { tpSpeed = value; }
            }
            private static ushort tpCurrent;
            public static ushort Current
            {
                get { return tpCurrent; }
                set { tpCurrent = value; }
            }
            private static ushort pwm_;
            public static ushort pwm
            {
                get { return pwm_; }
                set { pwm_ = value; }
            }
            private static ushort tpTemp;
            public static ushort PumpTemperature
            {
                get { return tpTemp; }
                set { tpTemp = value; }
            }
            private static ushort drTemp;
            public static ushort DriveTemperature
            {
                get { return drTemp; }
                set { drTemp = value; }
            }
            private static ushort opTime;
            public static ushort OperationTime
            {
                get { return opTime; }
                set { opTime = value; }
            }

            private static byte statusBits, alertBits, faultBits;
            public static byte StatusBits
            {
                get { return statusBits; }
                private set { statusBits = value; }
            }
            public static byte AlertBits
            {
                get { return alertBits; }
                private set 
                {
                    if (value != alertBits)
                    {
                        alertBits = value;
                        if (alertBits != 0)
                        {
                            OnTurboPumpAlert(false, alertBits);
                        }
                    }
                }
            }
            public static byte FaultBits
            {
                get { return faultBits; }
                private set
                {
                    if (value != faultBits)
                    {
                        faultBits = value;
                        if (faultBits != 0)
                        {
                            OnTurboPumpAlert(true, faultBits);
                        }
                    }
                }
            }
            public static void relaysState(byte value, byte value2, byte value3)
            {
                StatusBits = value;
                AlertBits = value2;
                FaultBits = value3;
                OnTurboPumpStatusChanged();
            }

            public static void Init()
            {
                tpSpeed = 0;
                tpCurrent = 0;
                pwm_ = 0;
                tpTemp = 0;
                drTemp = 0;
                opTime = 0;
                relaysState(0, 0, 0);
            }
        }
 
        public static void relaysState(byte value/*, byte value2*/)
        {
            fPumpOn = Convert.ToBoolean(value & 1);
            tPumpOn = Convert.ToBoolean(value & 1 << 1);
            probeValve = Convert.ToBoolean(value & 1 << 2);
            highVacuumValve = Convert.ToBoolean(value & 1 << 3);
            highVoltageOn = Convert.ToBoolean(value & 1 << 4);
            hCurrentEnable = Convert.ToBoolean(value & 1 << 5);
            turboReplyFault = Convert.ToBoolean(value & 1 << 6);

            Commander.hBlock = !highVoltageOn;//!!!
            //eCurrentEnable = Convert.ToBoolean(value2 & 1 << 4);
            OnDeviceStatusChanged();
        }

        internal static void Init()
        {
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
            Device.relaysState(0/*, 0*/);
            Device.TurboPump.Init();
        }
    }
}
