namespace Flavor.Common.Settings {
    abstract class CommonData {
        public ushort hCurrent { get; set; }
        public double hCurrentReal {
            get { return hCurrentConvert(hCurrent); }
            set { hCurrent = hCurrentConvert(value); }
        }
        public static double hCurrentConvert(ushort current) {
            return (double)current / 4096;
        }
        public static ushort hCurrentConvert(double current) {
            return genericConvert((ushort)(current * 4096));
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
            return 150 * (double)voltage / 4096;
        }
        public static ushort iVoltageConvert(double voltage) {
            return genericConvert((ushort)((voltage / 150) * 4096));
        }

        public ushort fV1 { get; set; }
        public double fV1Real {
            get { return fV1Convert(fV1); }
            set { fV1 = fV1Convert(value); }
        }
        public static double fV1Convert(ushort voltage) {
            return 150 * (double)voltage / 4096;
        }
        public static ushort fV1Convert(double voltage) {
            return genericConvert((ushort)((voltage / 150) * 4096));
        }

        public ushort fV2 { get; set; }
        public double fV2Real {
            get { return fV2Convert(fV2); }
            set { fV2 = fV2Convert(value); }
        }
        public static double fV2Convert(ushort voltage) {
            return 150 * (double)voltage / 4096;
        }
        public static ushort fV2Convert(double voltage) {
            return genericConvert((ushort)((voltage / 150) * 4096));
        }

        protected static ushort genericConvert(ushort x) {
            return x < 4096 ? x : (ushort)4095;
        }
    }
}
