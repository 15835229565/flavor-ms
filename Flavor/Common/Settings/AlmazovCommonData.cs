﻿namespace Flavor.Common.Settings {
    class AlmazovCommonData {
        public ushort d1V { get; set; }
        public double d1VReal {
            get { return dVConvert(d1V); }
            set { d1V = dVConvert(value); }
        }
        public ushort d2V { get; set; }
        public double d2VReal {
            get { return dVConvert(d2V); }
            set { d2V = dVConvert(value); }
        }
        public ushort d3V { get; set; }
        public double d3VReal {
            get { return dVConvert(d3V); }
            set { d3V = dVConvert(value); }
        }
        public static double dVConvert(ushort voltage) {
            return 5 * 600 * (double)voltage / 4096;
        }
        public static ushort dVConvert(double voltage) {
            return genericConvert((ushort)((voltage / 3000) * 4096));
        }

        public ushort eCurrent { get; set; }
        public double eCurrentReal {
            get { return eCurrentConvert(eCurrent); }
            set { eCurrent = eCurrentConvert(value); }
        }
        public static double eCurrentConvert(ushort current) {
            return 50 * (double)current / 4096;
        }
        public static ushort eCurrentConvert(double current) {
            return genericConvert((ushort)((current / 50) * 4096));
        }

        public ushort iVoltage { get; set; }
        public double iVoltageReal {
            get { return iVoltageConvert(iVoltage); }
            set { iVoltage = iVoltageConvert(value); }
        }
        public static double iVoltageConvert(ushort voltage) {
            return 100 * (double)voltage / 4096;
        }
        public static ushort iVoltageConvert(double voltage) {
            return genericConvert((ushort)((voltage / 100) * 4096));
        }

        public ushort fV1 { get; set; }
        public double fV1Real {
            get { return fV1Convert(fV1); }
            set { fV1 = fV1Convert(value); }
        }
        public static double fV1Convert(ushort voltage) {
            return 100 * (double)voltage / 4096;
        }
        public static ushort fV1Convert(double voltage) {
            return genericConvert((ushort)((voltage / 100) * 4096));
        }

        public ushort fV2 { get; set; }
        public double fV2Real {
            get { return fV2Convert(fV2); }
            set { fV2 = fV2Convert(value); }
        }
        public static double fV2Convert(ushort voltage) {
            return 100 * (double)voltage / 4096;
        }
        public static ushort fV2Convert(double voltage) {
            return genericConvert((ushort)((voltage / 100) * 4096));
        }

        protected static ushort genericConvert(ushort x) {
            return x < 4096 ? x : (ushort)4095;
        }
    }
}
