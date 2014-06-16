using System;
using System.Text;

namespace Flavor.Common.Settings {
//    class CommonOptions: CommonData {
    class CommonOptions: AlmazovCommonData {
        public ushort befTime { get; set; }
        public ushort befTimeReal {
            get { return (ushort)(befTime * 5); }
            set { befTime = (ushort)(value / 5); }
        }
        public ushort fTime { get; set; }
        public ushort fTimeReal {
            get { return (ushort)(fTime * 5); }
            set { fTime = (ushort)(value / 5); }
        }
        public ushort bTime { get; set; }
        public ushort bTimeReal {
            get { return (ushort)(bTime * 5); }
            set { bTime = (ushort)(value / 5); }
        }
        public bool ForwardTimeEqualsBeforeTime { get; set; }
        public ushort eTime { get; set; }
        public ushort eTimeReal {
            get { return (ushort)(eTime * 5); }
            set { eTime = (ushort)(value / 5); }
        }
        public ushort iTime { get; set; }
        public ushort iTimeReal {
            get { return (ushort)(5 * iTime); }
            set { iTime = (ushort)(value / 5); }
        }
        public ushort CP { get; set; }
        public double CPReal {
            get { return CPConvert(CP); }
            set { CP = CPConvert(value); }
        }
        public static double CPConvert(ushort coeff) {
            return (10 / (double)coeff) * 4096;
        }
        public static ushort CPConvert(double coeff) {
            return genericConvert((ushort)((10 / coeff) * 4096));
        }

        public CommonOptions() {
            // defaults
            befTime = 100;
            fTime = 100;
            bTime = 400;
            ForwardTimeEqualsBeforeTime = false;
            eTime = 200;
            iTime = 10;
            CP = 3780;
        }
        // scan voltage modification law
        public static ushort scanVoltage(ushort step) {
            if (step > Config.MAX_STEP) step = Config.MAX_STEP;
            return (ushort)(4095 * Math.Pow(((double)527 / (double)528), 1056 - step));
        }
        public static double scanVoltageReal(ushort step) {
            return (double)(scanVoltage(step) * 5 * 600) / 4096;
        }

        const string DELIMITER = " ";
        const string START = "{";
        const string END = "}";
        public override string ToString() {
            return (new StringBuilder())
                .Append(START)
                .Append(eTime)
                .Append(DELIMITER)
                .Append(iTime)
                .Append(DELIMITER)
                .Append(iVoltage)
                .Append(DELIMITER)
                .Append(CP)
                .Append(DELIMITER)
                //.Append(hCurrent)
                .Append(DELIMITER)
                .Append(eCurrent)
                .Append(DELIMITER)
                .Append(fV1)
                .Append(DELIMITER)
                .Append(fV2)
                .Append(END).ToString();
        }
    }
}
