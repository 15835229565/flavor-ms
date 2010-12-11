using System;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;
using System.Text;

namespace Flavor.Common {
    static class Config {
        private static IMainConfig mainConfig;
        private static IMainConfigWriter mainConfigWriter;
        
        private static readonly string INITIAL_DIR = System.IO.Directory.GetCurrentDirectory();

        private const string CONFIG_NAME = "config.xml";
        private const string CRASH_LOG_NAME = "MScrash.log";

        private static string mainConfigName;
        private static string logName;

        #region Extensions
        internal const string SPECTRUM_EXT = "sdf";
        internal const string PRECISE_SPECTRUM_EXT = "psf";
        internal const string MONITOR_SPECTRUM_EXT = "mon";
        #endregion
        internal const string DIFF_FILE_SUFFIX = "~diff";
        #region Dialog filters
        internal static readonly string SPECTRUM_FILE_DIALOG_FILTER = string.Format("Specter data files (*.{0})|*.{0}", SPECTRUM_EXT);
        internal static readonly string PRECISE_SPECTRUM_FILE_DIALOG_FILTER = string.Format("Precise specter files (*.{0})|*.{0}", PRECISE_SPECTRUM_EXT);
        #endregion
        #region Spectra headers
        //TODO: move to TagHolder!
        private const string MONITOR_SPECTRUM_HEADER = "Monitor";
        private const string PRECISE_OPTIONS_HEADER = "Precise options";
        #endregion
        private static string SerialPort = "COM1";
        private static ushort SerialBaudRate = 38400;
        private static byte sendTry = 1;

        internal const ushort MIN_STEP = 0;
        internal const ushort MAX_STEP = 1056;
        private static ushort startPoint = MIN_STEP;
        private static ushort endPoint = MAX_STEP;

        private static readonly CommonOptions commonOpts = new CommonOptions();
        internal static CommonOptions CommonOptions {
            get { return commonOpts; }
        }

        private static PreciseSpectrum preciseData = new PreciseSpectrum();
        internal static PreciseSpectrum PreciseData {
            get { return preciseData; }
        }

        private static Utility.PreciseEditorData reperPeak = null;
        internal static Utility.PreciseEditorData CheckerPeak {
            get {
                if (reperPeak == null) {
                    return null;
                }
                ushort maxIteration = 0;
                foreach (Utility.PreciseEditorData ped in Config.PreciseData.FindAll(Utility.PeakIsUsed)) {
                    maxIteration = maxIteration < ped.Iterations ? ped.Iterations : maxIteration;
                }
                return new Utility.PreciseEditorData(false, 255, reperPeak.Step, reperPeak.Collector, maxIteration, reperPeak.Width, 0, "checker peak");
            }
        }
        internal static List<Utility.PreciseEditorData> PreciseDataWithChecker {
            get {
                ushort maxIteration = 0;
                List<Utility.PreciseEditorData> res = preciseData.FindAll(Utility.PeakIsUsed);
                if (res.Count == 0) {
                    return null;
                }
                foreach (Utility.PreciseEditorData ped in res) {
                    maxIteration = maxIteration < ped.Iterations ? ped.Iterations : maxIteration;
                }
                // mark checker peak with false flag
                if (reperPeak != null) {
                    res.Add(new Utility.PreciseEditorData(false, 255, reperPeak.Step, reperPeak.Collector, maxIteration, reperPeak.Width, 0, "checker peak"));
                }
                return res;
            }
        }
        private static int iterations = 0;
        internal static int Iterations {
            get { return iterations; }
        }
        private static int timeLimit = 0;
        internal static int TimeLimit {
            get { return timeLimit; }
        }
        private static ushort allowedShift = 0;
        internal static ushort AllowedShift {
            get { return allowedShift; }
        }

        internal static string Port {
            get { return SerialPort; }
            set { SerialPort = value; }
        }
        internal static ushort BaudRate {
            get { return SerialBaudRate; }
            set { SerialBaudRate = value; }
        }

        internal static byte Try {
            get { return sendTry; }
        }

        internal static ushort sPoint {
            get { return startPoint; }
            set { startPoint = value; }
        }
        internal static ushort ePoint {
            get { return endPoint; }
            set { endPoint = value; }
        }

        internal static void getInitialDirectory() {
            mainConfigName = System.IO.Path.Combine(INITIAL_DIR, CONFIG_NAME);
            logName = System.IO.Path.Combine(INITIAL_DIR, CRASH_LOG_NAME);
        }
        #region Global Config I/O
        internal static void loadGlobalConfig() {
            mainConfig = TagHolder.getMainConfig(mainConfigName);
            mainConfig.read();
            mainConfigWriter = TagHolder.getMainConfigWriter(mainConfigName, mainConfig.XML);
        }
        internal static void saveGlobalScanOptions(ushort sPointReal, ushort ePointReal) {
            Config.sPoint = sPointReal;//!!!
            Config.ePoint = ePointReal;//!!!
            mainConfigWriter.write();
        }
        internal static void saveGlobalDelaysOptions(bool forwardAsBefore, ushort befTimeReal, ushort fTimeReal, ushort bTimeReal) {
            Config.commonOpts.befTimeReal = befTimeReal;
            Config.commonOpts.fTimeReal = fTimeReal;
            Config.commonOpts.bTimeReal = bTimeReal;
            Config.commonOpts.ForwardTimeEqualsBeforeTime = forwardAsBefore;
            mainConfigWriter.write();
        }
        internal static void saveGlobalConnectOptions(string port, ushort baudrate) {
            Config.Port = port;
            Config.BaudRate = baudrate;
            mainConfigWriter.write();
        }
        internal static void saveGlobalCheckOptions(int iter, int timeLim, ushort shift, Utility.PreciseEditorData peak) {
            iterations = iter;
            timeLimit = timeLim;
            allowedShift = shift;
            reperPeak = peak;
            mainConfigWriter.write();
        }
        internal static void saveGlobalPreciseOptions(PreciseSpectrum peds) {
            preciseData = peds;
            mainConfigWriter.savePreciseData(peds, false);
            mainConfigWriter.write();
        }
        internal static void saveGlobalCommonOptions(ushort eT, ushort iT, double iV, double cp, double eC, double hC, double fv1, double fv2) {
            Config.commonOpts.eTimeReal = eT;
            Config.commonOpts.iTimeReal = iT;
            Config.commonOpts.iVoltageReal = iV;
            Config.commonOpts.CPReal = cp;
            Config.commonOpts.eCurrentReal = eC;
            Config.commonOpts.hCurrentReal = hC;
            Config.commonOpts.fV1Real = fv1;
            Config.commonOpts.fV2Real = fv2;
            mainConfigWriter.saveCommonOptions(Config.CommonOptions);
            mainConfigWriter.write();
        }
        internal static void saveGlobalConfig() {
            mainConfigWriter.write();
        }
        #endregion
        #region Spectra I/O
        #region Manual Actions
        internal static bool openSpectrumFile(string filename, bool hint, out Graph graph) {
            return TagHolder.getSpectrumReader(filename, hint).readSpectrum(out graph);
        }
        internal static void saveSpectrumFile(string filename, Graph graph) {
            // TODO: avoid losing time and shift information!
            TagHolder.getSpectrumWriter(filename, graph).write();
        }
        internal static void savePreciseSpectrumFile(string filename, Graph graph) {
            ISpectrumWriter writer = TagHolder.getSpectrumWriter(filename, graph);
            writer.savePreciseData(graph.PreciseData, false);
            writer.write();
        }
        #region Spectra Distraction
        internal static void distractSpectra(string what, ushort step, Graph.pListScaled plsReference, Utility.PreciseEditorData pedReference, Graph graph) {
            bool hint = !graph.isPreciseSpectrum;
            ISpectrumReader reader = TagHolder.getSpectrumReader(what, hint);
            if (reader.Hint != hint) {
                throw new ConfigLoadException("Несовпадение типов спектров", "Ошибка при вычитании спектров", what);
            }
            if (graph.DisplayingMode == Graph.Displaying.Diff) {
                //diffs can't be distracted!
                throw new ArgumentOutOfRangeException();
            }
            if (hint) {
                PointPairListPlus pl12 = new PointPairListPlus();
                PointPairListPlus pl22 = new PointPairListPlus();
                CommonOptions commonOpts;
                if (reader.openSpectrumFile(pl12, pl22, out commonOpts) == Graph.Displaying.Measured) {
                    // TODO: check commonOpts for equality?

                    // coeff counting
                    double coeff = 1.0;
                    if (plsReference != null) {
                        PointPairListPlus PL = plsReference.IsFirstCollector ? graph.Displayed1Steps[0] : graph.Displayed2Steps[0];
                        PointPairListPlus pl = plsReference.IsFirstCollector ? pl12 : pl22;
                        if (step != 0) {
                            for (int i = 0; i < PL.Count; ++i) {
                                if (step == PL[i].X) {
                                    if (step != pl[i].X)
                                        throw new System.ArgumentException();
                                    if ((pl[i].Y != 0) && (PL[i].Y != 0))
                                        coeff = PL[i].Y / pl[i].Y;
                                    break;
                                }
                            }
                        }
                    }
                    try {
                        PointPairListPlus diff1 = PointPairListDiff(graph.Displayed1Steps[0], pl12, coeff);
                        PointPairListPlus diff2 = PointPairListDiff(graph.Displayed2Steps[0], pl22, coeff);
                        graph.updateGraphAfterScanDiff(diff1, diff2);
                    } catch (System.ArgumentException) {
                        throw new ConfigLoadException("Несовпадение рядов данных", "Ошибка при вычитании спектров", what);
                    }
                } else {
                    //diffs can't be distracted!
                    throw new ArgumentOutOfRangeException();
                }
                return;
            }
            PreciseSpectrum peds = new PreciseSpectrum();
            if (reader.openPreciseSpectrumFile(peds)) {
                List<Utility.PreciseEditorData> temp = new List<Utility.PreciseEditorData>(graph.PreciseData);
                temp.Sort(Utility.ComparePreciseEditorData);
                try {
                    temp = PreciseEditorDataListDiff(temp, peds, step, pedReference);
                    graph.updateGraphAfterPreciseDiff(temp);
                } catch (System.ArgumentException) {
                    throw new ConfigLoadException("Несовпадение рядов данных", "Ошибка при вычитании спектров", what);
                }
            }
        }
        private static PointPairListPlus PointPairListDiff(PointPairListPlus from, PointPairListPlus what, double coeff) {
            if (from.Count != what.Count)
                throw new System.ArgumentOutOfRangeException();

            PointPairListPlus res = new PointPairListPlus(from, null, null);
            for (int i = 0; i < res.Count; ++i) {
                if (res[i].X != what[i].X)
                    throw new System.ArgumentException();
                res[i].Y -= what[i].Y * coeff;
            }
            return res;
        }
        private static Utility.PreciseEditorData PreciseEditorDataDiff(Utility.PreciseEditorData from, Utility.PreciseEditorData what, double coeff) {
            if (!from.Equals(what))
                throw new System.ArgumentException();
            if ((from.AssociatedPoints == null || from.AssociatedPoints.Count == 0) ^ (what.AssociatedPoints == null || what.AssociatedPoints.Count == 0))
                throw new System.ArgumentException();
            if (from.AssociatedPoints != null && what.AssociatedPoints != null && from.AssociatedPoints.Count != what.AssociatedPoints.Count)
                throw new System.ArgumentException();
            if ((from.AssociatedPoints == null || from.AssociatedPoints.Count == 0) && (what.AssociatedPoints == null || what.AssociatedPoints.Count == 0))
                return new Utility.PreciseEditorData(from);
            if (from.AssociatedPoints.Count != 2 * from.Width + 1)
                throw new System.ArgumentException();
            Utility.PreciseEditorData res = new Utility.PreciseEditorData(from);
            for (int i = 0; i < res.AssociatedPoints.Count; ++i) {
                if (res.AssociatedPoints[i].X != what.AssociatedPoints[i].X)
                    throw new System.ArgumentException();
                res.AssociatedPoints[i].Y -= what.AssociatedPoints[i].Y * coeff;
            }
            return res;
        }
        private static List<Utility.PreciseEditorData> PreciseEditorDataListDiff(List<Utility.PreciseEditorData> from, List<Utility.PreciseEditorData> what, ushort step, Utility.PreciseEditorData pedReference) {
            if (from.Count != what.Count)
                throw new System.ArgumentOutOfRangeException();

            // coeff counting
            double coeff = 1.0;
            if (pedReference != null) {
                int fromIndex = from.IndexOf(pedReference);
                int whatIndex = what.IndexOf(pedReference);
                if ((fromIndex == -1) || (whatIndex == -1))
                    throw new System.ArgumentException();
                if (System.Math.Abs(from[fromIndex].Step - step) > from[fromIndex].Width)
                    throw new System.ArgumentOutOfRangeException();
                if (from[fromIndex].AssociatedPoints.Count != what[whatIndex].AssociatedPoints.Count)
                    throw new System.ArgumentException();
                if (from[fromIndex].AssociatedPoints.Count != 2 * from[fromIndex].Width + 1)
                    throw new System.ArgumentException();

                if ((step != ushort.MaxValue)) {
                    //diff on point
                    if ((what[whatIndex].AssociatedPoints[step - what[whatIndex].Step + what[whatIndex].Width].Y != 0) &&
                        (from[fromIndex].AssociatedPoints[step - from[fromIndex].Step + from[fromIndex].Width].Y != 0))
                        coeff = from[fromIndex].AssociatedPoints[step - from[fromIndex].Step + from[fromIndex].Width].Y /
                                what[whatIndex].AssociatedPoints[step - what[whatIndex].Step + what[whatIndex].Width].Y;
                } else {
                    //diff on peak sum
                    double sumFrom = from[fromIndex].AssociatedPoints.PLSreference.PeakSum;
                    if (sumFrom != 0) {
                        double sumWhat = 0;
                        foreach (ZedGraph.PointPair pp in what[whatIndex].AssociatedPoints) {
                            sumWhat += pp.Y;
                        }
                        if (sumWhat != 0)
                            coeff = sumFrom / sumWhat;
                    }
                }
            }

            List<Utility.PreciseEditorData> res = new List<Utility.PreciseEditorData>(from);
            for (int i = 0; i < res.Count; ++i) {
                res[i] = PreciseEditorDataDiff(res[i], what[i], coeff);
            }
            return res;
        }
        #endregion
        #endregion
        #region Automatic Actions
        private static string genAutoSaveFilename(string extension, DateTime now) {
            string dirname;
            dirname = System.IO.Path.Combine(INITIAL_DIR, string.Format("{0}-{1}-{2}", now.Year, now.Month, now.Day));
            if (!System.IO.Directory.Exists(@dirname)) {
                System.IO.Directory.CreateDirectory(@dirname);
            }
            return System.IO.Path.Combine(dirname, string.Format("{0}-{1}-{2}-{3}.", now.Hour, now.Minute, now.Second, now.Millisecond) + extension);
        }
        internal static void autoSaveSpectrumFile() {
            DateTime dt = System.DateTime.Now;
            string filename = genAutoSaveFilename(SPECTRUM_EXT, dt);

            ISpectrumWriter writer = TagHolder.getSpectrumWriter(filename, Graph.Instance);
            writer.setTimeStamp(dt);
            writer.write();
        }
        internal static DateTime autoSavePreciseSpectrumFile(short shift) {
            DateTime dt = System.DateTime.Now;
            string filename = genAutoSaveFilename(PRECISE_SPECTRUM_EXT, dt);

            ISpectrumWriter writer = TagHolder.getSpectrumWriter(filename, Graph.Instance);
            writer.setTimeStamp(dt);
            writer.setShift(shift);
            writer.savePreciseData(Graph.Instance.PreciseData, false);
            writer.write();

            return dt;
        }
        internal static void autoSaveMonitorSpectrumFile(short shift) {
            DateTime dt = autoSavePreciseSpectrumFile(shift);// now both files are saved
            string filename = genAutoSaveFilename(MONITOR_SPECTRUM_EXT, dt);
            IPreciseDataWriter writer = TagHolder.getPreciseDataWriter(filename, MONITOR_SPECTRUM_HEADER);
            writer.setTimeStamp(dt);
            writer.setShift(shift);
            writer.savePreciseData(Graph.Instance.PreciseData, true);
            writer.write();
        }
        #endregion
        #endregion
        #region Config I/O
        internal static List<Utility.PreciseEditorData> loadPreciseOptions(string pedConfName) {
            return TagHolder.getPreciseDataReader(pedConfName).loadPreciseData();
        }
        internal static void savePreciseOptions(List<Utility.PreciseEditorData> peds, string pedConfName, bool savePeakSum) {
            IPreciseDataWriter writer = TagHolder.getPreciseDataWriter(pedConfName, PRECISE_OPTIONS_HEADER);
            writer.savePreciseData(peds, savePeakSum);
            writer.write();
        }

        internal static void loadCommonOptions(string cdConfName) {
            TagHolder.getCommonOptionsReader(cdConfName).loadCommonOptions(Config.commonOpts);
        }
        internal static void saveCommonOptions(string filename, ushort eT, ushort iT, double iV, double cp, double eC, double hC, double fv1, double fv2) {
            ICommonOptionsWriter writer = TagHolder.getCommonOptionsWriter(filename);
            writer.saveCommonOptions(eT, iT, iV, cp, eC, hC, fv1, fv2);
            writer.write();
        }
        #endregion
        #region Error messages on loading different configs
        internal class ConfigLoadException: System.Exception {
            internal ConfigLoadException(string message, string filestring, string confname)
                : base(message) {
                this.Data["FS"] = filestring;
                this.Data["CN"] = confname;
            }
            internal void visualise() {
                if (!(this.Data["CN"].Equals(mainConfigName)))
                    System.Windows.Forms.MessageBox.Show(this.Message, this.Data["FS"] as string);
                else
                    System.Windows.Forms.MessageBox.Show(this.Message, "Ошибка чтения конфигурационного файла");
            }
        }
        private class wrongFormatOnLoadPrecise: wrongFormatOnLoad {
            internal wrongFormatOnLoadPrecise(string configName)
                : base(configName, "Ошибка чтения файла прецизионных точек") { }
        }
        private class wrongFormatOnLoad: ConfigLoadException {
            internal wrongFormatOnLoad(string configName, string errorFile)
                : base("Неверный формат данных", errorFile, configName) { }
        }
        private class structureErrorOnLoad: ConfigLoadException {
            internal structureErrorOnLoad(string configName, string errorFile)
                : base("Ошибка структуры файла", errorFile, configName) { }
        }
        private class structureErrorOnLoadCommonData: structureErrorOnLoad {
            internal structureErrorOnLoadCommonData(string configName)
                : base(configName, "Ошибка чтения файла общих настроек") { }
        }
        private class structureErrorOnLoadPrecise: structureErrorOnLoad {
            internal structureErrorOnLoadPrecise(string configName)
                : base(configName, "Ошибка чтения файла прецизионных точек") { }
        }
        #endregion
        #region Graph scaling to mass coeffs
        private static double col1Coeff = 2770 * 28;
        private static double col2Coeff = 896.5 * 18;
        internal static void setScalingCoeff(byte col, ushort pnt, double mass) {
            double value = mass * Config.commonOpts.scanVoltageReal(pnt);
            if (col == 1) {
                if (value != col1Coeff) {
                    col1Coeff = value;
                    Graph.Instance.RecomputeMassRows(col);
                }
            } else {
                if (value != col2Coeff) {
                    col2Coeff = value;
                    Graph.Instance.RecomputeMassRows(col);
                }
            }
            mainConfigWriter.write();
        }
        internal static double pointToMass(ushort pnt, bool isFirstCollector) {
            double coeff;
            if (isFirstCollector)
                coeff = col1Coeff;
            else
                coeff = col2Coeff;
            return coeff / Config.commonOpts.scanVoltageReal(pnt);
        }
        #endregion
        #region Logging routines
        private static System.IO.StreamWriter openLog() {
            try {
                System.IO.StreamWriter errorLog;
                errorLog = new System.IO.StreamWriter(@logName, true);
                errorLog.AutoFlush = true;
                return errorLog;
            } catch (Exception e) {
                System.Windows.Forms.MessageBox.Show(e.Message, "Ошибка при открытии файла отказов");
                return null;
            }
        }
        private static string genMessage(string data, DateTime moment) {
            return string.Format("{0}-{1}-{2}|", moment.Year, moment.Month, moment.Day) +
                string.Format("{0}.{1}.{2}.{3}: ", moment.Hour, moment.Minute, moment.Second, moment.Millisecond) + data;
        }
        private static void log(System.IO.StreamWriter errorLog, string msg) {
            DateTime now = System.DateTime.Now;
            try {
                errorLog.WriteLine(genMessage(msg, now));
                //errorLog.Flush();
            } catch (Exception Error) {
                string message = "Error log write failure ";
                string cause = "(" + msg + ") -- " + Error.Message;
                ConsoleWriter.WriteLine(message + cause);
                System.Windows.Forms.MessageBox.Show(cause, message);
            } finally {
                errorLog.Close();
            }
        }
        internal static void logCrash(byte[] commandline) {
            string cmd = "";
            List<byte> pack = new List<byte>();
            ModBus.buildPackBody(pack, commandline);
            foreach (byte b in pack) {
                cmd += (char)b;
            }
            System.IO.StreamWriter errorLog;
            if ((errorLog = openLog()) == null)
                return;
            log(errorLog, cmd);
        }
        internal static void logTurboPumpAlert(string message) {
            System.IO.StreamWriter errorLog;
            if ((errorLog = openLog()) == null)
                return;
            log(errorLog, message);
        }
        #endregion
        #region XML Configs
        private interface ITimeStamp {
            void setTimeStamp(DateTime dt);
        }
        private interface IShift {
            void setShift(short shift);
        }
        private interface IAnyConfig { }
        private interface IAnyReader: IAnyConfig {}
        private interface ICommonOptionsReader: IAnyReader {
            void loadCommonOptions(CommonOptions opts);
        }
        private interface IPreciseDataReader: IAnyReader {
            List<Utility.PreciseEditorData> loadPreciseData();
        }
        private interface ISpectrumReader: ICommonOptionsReader, IPreciseDataReader {
            bool readSpectrum(out Graph graph);
            bool Hint {
                get;
                set;
            }
            Graph.Displaying openSpectrumFile(PointPairListPlus pl12, PointPairListPlus pl22, out CommonOptions commonOpts);
            bool openPreciseSpectrumFile(PreciseSpectrum peds);
        }
        private interface IMainConfig: ICommonOptionsReader, IPreciseDataReader {
            void read();
            XmlDocument XML {
                get;
            }
        }
        private interface IAnyWriter: IAnyConfig {
            void write();
        }
        private interface ICommonOptionsWriter: IAnyWriter {
            void saveCommonOptions(ushort eT, ushort iT, double iV, double cp, double eC, double hC, double fv1, double fv2);
            void saveCommonOptions(CommonOptions opts);
        }
        private interface IPreciseDataWriter: IAnyWriter, ITimeStamp, IShift { 
            void savePreciseData(List<Utility.PreciseEditorData> peds/*, bool savePoints*/, bool savePeakSum);
        }
        private interface ISpectrumWriter: ICommonOptionsWriter, IPreciseDataWriter {}
        private interface IMainConfigWriter: ICommonOptionsWriter, IPreciseDataWriter {}
        private abstract class TagHolder {
            #region Tags
            private const string ROOT_CONFIG_TAG = "control";
            private const string VERSION_ATTRIBUTE = "version";
            private const string CONFIG_VERSION = "1.0";

            private const string HEADER_CONFIG_TAG = "header";

            private const string CONNECT_CONFIG_TAG = "connect";
            private const string PORT_CONFIG_TAG = "port";
            private const string BAUDRATE_CONFIG_TAG = "baudrate";
            private const string TRY_NUMBER_CONFIG_TAG = "try";

            private const string COMMON_CONFIG_TAG = "common";
            private const string EXPOSITURE_TIME_CONFIG_TAG = "exptime";
            private const string TRANSITION_TIME_CONFIG_TAG = "meastime";
            private const string IONIZATION_VOLTAGE_CONFIG_TAG = "ivoltage";
            private const string CAPACITOR_VOLTAGE_COEFF_CONFIG_TAG = "cp";
            private const string EMISSION_CURRENT_CONFIG_TAG = "ecurrent";
            private const string HEAT_CURRENT_CONFIG_TAG = "hcurrent";
            private const string FOCUS_VOLTAGE1_CONFIG_TAG = "focus1";
            private const string FOCUS_VOLTAGE2_CONFIG_TAG = "focus2";

            private const string DELAY_BEFORE_MEASURE_CONFIG_TAG = "before";
            private const string EQUAL_DELAYS_CONFIG_TAG = "equal";
            private const string DELAY_FORWARD_MEASURE_CONFIG_TAG = "forward";
            private const string DELAY_BACKWARD_MEASURE_CONFIG_TAG = "back";

            private const string POINT_CONFIG_TAG = "p";
            private const string POINT_STEP_CONFIG_TAG = "s";
            private const string POINT_COUNT_CONFIG_TAG = "c";

            private const string OVERVIEW_CONFIG_TAG = "overview";
            private const string START_SCAN_CONFIG_TAG = "start";
            private const string END_SCAN_CONFIG_TAG = "end";
            private const string COL1_CONFIG_TAG = "collector1";
            private const string COL2_CONFIG_TAG = "collector2";

            private const string SENSE_CONFIG_TAG = "sense";
            private const string PEAK_TAGS_FORMAT = "region{0}";
            private const string PEAK_NUMBER_CONFIG_TAG = "peak";
            private const string PEAK_COL_NUMBER_CONFIG_TAG = "col";
            private const string PEAK_WIDTH_CONFIG_TAG = "width";
            private const string PEAK_ITER_NUMBER_CONFIG_TAG = "iteration";
            private const string PEAK_PRECISION_CONFIG_TAG = "error";
            private const string PEAK_COMMENT_CONFIG_TAG = "comment";
            private const string PEAK_USE_CONFIG_TAG = "use";

            private const string PEAK_COUNT_SUM_CONFIG_TAG = "sum";

            private const string CHECK_CONFIG_TAG = "check";
            private const string CHECK_ITER_NUMBER_CONFIG_TAG = "iterations";
            private const string CHECK_TIME_LIMIT_CONFIG_TAG = "limit";
            private const string CHECK_MAX_SHIFT_CONFIG_TAG = "allowed";

            private const string INTERFACE_CONFIG_TAG = "interface";
            private const string C1_CONFIG_TAG = "coeff1";
            private const string C2_CONFIG_TAG = "coeff2";

            private const string TIME_SPECTRUM_ATTRIBUTE = "time";
            private const string SHIFT_SPECTRUM_ATTRIBUTE = "shift";
            #endregion
            #region Spectra headers
            //private const string MONITOR_SPECTRUM_HEADER = "Monitor";
            //private const string PRECISE_OPTIONS_HEADER = "Precise options";
            private const string COMMON_OPTIONS_HEADER = "Common options";
            private const string MEASURED_SPECTRUM_HEADER = "Measure";
            private const string DIFF_SPECTRUM_HEADER = "Diff";
            #endregion
            private string filename;
            private XmlDocument xmlData;
            protected void initialize(string filename, XmlDocument doc) {
                this.filename = filename;
                this.xmlData = doc;
            }
            #region Legacy Readers
            private abstract class LegacyTagHolder: TagHolder {
            
            }
            private abstract class LegacyReader: LegacyTagHolder {
                public void loadCommonOptions(CommonOptions opts) {
                    string mainConfPrefix = "";

                    if (xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, COMMON_CONFIG_TAG)) != null)
                        mainConfPrefix = ROOT_CONFIG_TAG;
                    else if (xmlData.SelectSingleNode(COMMON_CONFIG_TAG) == null) {
                        throw new structureErrorOnLoadCommonData(filename);
                    }
                    XmlNode commonNode = xmlData.SelectSingleNode(combine(mainConfPrefix, COMMON_CONFIG_TAG));
                    try {
                        ushort eT, iT, iV, CP, eC, hC, fV1, fV2;

                        eT = ushort.Parse(commonNode.SelectSingleNode(EXPOSITURE_TIME_CONFIG_TAG).InnerText);
                        iT = ushort.Parse(commonNode.SelectSingleNode(TRANSITION_TIME_CONFIG_TAG).InnerText);
                        iV = ushort.Parse(commonNode.SelectSingleNode(IONIZATION_VOLTAGE_CONFIG_TAG).InnerText);
                        CP = ushort.Parse(commonNode.SelectSingleNode(CAPACITOR_VOLTAGE_COEFF_CONFIG_TAG).InnerText);
                        eC = ushort.Parse(commonNode.SelectSingleNode(EMISSION_CURRENT_CONFIG_TAG).InnerText);
                        hC = ushort.Parse(commonNode.SelectSingleNode(HEAT_CURRENT_CONFIG_TAG).InnerText);
                        fV1 = ushort.Parse(commonNode.SelectSingleNode(FOCUS_VOLTAGE1_CONFIG_TAG).InnerText);
                        fV2 = ushort.Parse(commonNode.SelectSingleNode(FOCUS_VOLTAGE2_CONFIG_TAG).InnerText);

                        opts.eTime = eT;
                        opts.iTime = iT;
                        opts.iVoltage = iV;
                        opts.CP = CP;
                        opts.eCurrent = eC;
                        opts.hCurrent = hC;
                        opts.fV1 = fV1;
                        opts.fV2 = fV2;
                    } catch (NullReferenceException) {
                        throw new structureErrorOnLoadCommonData(filename);
                    }

                    try {
                        ushort befT, fT, bT;
                        bool fAsbef;

                        befT = ushort.Parse(commonNode.SelectSingleNode(DELAY_BEFORE_MEASURE_CONFIG_TAG).InnerText);
                        fT = ushort.Parse(commonNode.SelectSingleNode(DELAY_FORWARD_MEASURE_CONFIG_TAG).InnerText);
                        bT = ushort.Parse(commonNode.SelectSingleNode(DELAY_BACKWARD_MEASURE_CONFIG_TAG).InnerText);
                        fAsbef = bool.Parse(commonNode.SelectSingleNode(EQUAL_DELAYS_CONFIG_TAG).InnerText);

                        opts.befTime = befT;
                        opts.ForwardTimeEqualsBeforeTime = fAsbef;
                        opts.fTime = fT;
                        opts.bTime = bT;
                    } catch (NullReferenceException) {
                        //Use hard-coded defaults
                        return;
                    }
                }
                public List<Utility.PreciseEditorData> loadPreciseData() {
                    List<Utility.PreciseEditorData> peds = new List<Utility.PreciseEditorData>();
                    string mainConfPrefix = "";
                    if (xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, SENSE_CONFIG_TAG)) != null)
                        mainConfPrefix = ROOT_CONFIG_TAG;
                    else if (xmlData.SelectSingleNode(SENSE_CONFIG_TAG) == null) {
                        throw new structureErrorOnLoadPrecise(filename);
                    }
                    if (LoadPED(xmlData, "", peds, mainConfPrefix))
                        return peds;
                    return null;
                }
                protected bool LoadPED(XmlDocument pedConf, string errorMessage, List<Utility.PreciseEditorData> peds, string mainConfPrefix) {
                    for (int i = 1; i <= 20; ++i) {
                        Utility.PreciseEditorData temp = null;
                        string peak, iter, width, col;
                        try {
                            XmlNode regionNode = pedConf.SelectSingleNode(combine(mainConfPrefix, SENSE_CONFIG_TAG, string.Format(PEAK_TAGS_FORMAT, i)));
                            peak = regionNode.SelectSingleNode(PEAK_NUMBER_CONFIG_TAG).InnerText;
                            col = regionNode.SelectSingleNode(PEAK_COL_NUMBER_CONFIG_TAG).InnerText;
                            iter = regionNode.SelectSingleNode(PEAK_ITER_NUMBER_CONFIG_TAG).InnerText;
                            width = regionNode.SelectSingleNode(PEAK_WIDTH_CONFIG_TAG).InnerText;
                            bool allFilled = ((peak != "") && (iter != "") && (width != "") && (col != ""));
                            if (allFilled) {
                                string comment = "";
                                try {
                                    comment = regionNode.SelectSingleNode(PEAK_COMMENT_CONFIG_TAG).InnerText;
                                } catch (NullReferenceException) { }
                                bool use = true;
                                try {
                                    use = bool.Parse(regionNode.SelectSingleNode(PEAK_USE_CONFIG_TAG).InnerText);
                                } catch (NullReferenceException) { } catch (FormatException) { }
                                try {
                                    temp = new Utility.PreciseEditorData(use, (byte)(i - 1), ushort.Parse(peak),
                                                                 byte.Parse(col), ushort.Parse(iter),
                                                                 ushort.Parse(width), (float)0, comment);
                                } catch (FormatException) {
                                    throw new ConfigLoadException("Неверный формат данных", errorMessage, filename);
                                }
                                temp.AssociatedPoints = readPeaks(regionNode);
                            }
                        } catch (NullReferenceException) {
                            throw new ConfigLoadException("Ошибка структуры файла", errorMessage, filename);
                        }
                        if (temp != null) peds.Add(temp);
                    }
                    peds.Sort(Utility.ComparePreciseEditorData);
                    return true;
                }
                protected virtual PointPairListPlus readPeaks(XmlNode regionNode) {
                    return null;
                }
            }
            private class LegacyCommonOptionsReader: LegacyReader, ICommonOptionsReader {}
            private class LegacyPreciseDataReader: LegacyReader, IPreciseDataReader {}
            private class LegacyMainConfig: LegacyReader, IMainConfig {
                #region IMainConfig implementation
                public void read()
                {
                    string prefix;
                    try {
                        prefix = combine(ROOT_CONFIG_TAG, CONNECT_CONFIG_TAG);
                        SerialPort = (xmlData.SelectSingleNode(combine(prefix, PORT_CONFIG_TAG)).InnerText);
                        SerialBaudRate = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, BAUDRATE_CONFIG_TAG)).InnerText);
                        sendTry = byte.Parse(xmlData.SelectSingleNode(combine(prefix, TRY_NUMBER_CONFIG_TAG)).InnerText);
                    } catch (NullReferenceException) {
                        (new ConfigLoadException("Ошибка структуры конфигурационного файла", "Ошибка чтения конфигурационного файла", filename)).visualise();
                        //use hard-coded defaults
                    }
                    try {
                        prefix = combine(ROOT_CONFIG_TAG, OVERVIEW_CONFIG_TAG);
                        sPoint = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, START_SCAN_CONFIG_TAG)).InnerText);
                        ePoint = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, END_SCAN_CONFIG_TAG)).InnerText);
                    } catch (NullReferenceException) {
                        (new ConfigLoadException("Ошибка структуры конфигурационного файла", "Ошибка чтения конфигурационного файла", filename)).visualise();
                        //use hard-coded defaults
                    }
                    try {
                        loadMassCoeffs();
                    } catch (ConfigLoadException) {
                        //cle.visualise();
                        //use hard-coded defaults
                    }
                    try {
                        loadCommonOptions(commonOpts);
                    } catch (ConfigLoadException cle) {
                        cle.visualise();
                        //use hard-coded defaults
                    }
                    try {
                        List<Utility.PreciseEditorData> pedl = loadPreciseData();
                        if ((pedl != null) && (pedl.Count > 0)) {
                            //BAD!!! cleaning previous points!!!
                            preciseData.Clear();
                            preciseData.AddRange(pedl);
                        }
                    } catch (ConfigLoadException cle) {
                        cle.visualise();
                        //use empty default ped
                    }
                    prefix = combine(ROOT_CONFIG_TAG, CHECK_CONFIG_TAG);
                    try {
                        ushort step = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, PEAK_NUMBER_CONFIG_TAG)).InnerText);
                        byte collector = byte.Parse(xmlData.SelectSingleNode(combine(prefix, PEAK_COL_NUMBER_CONFIG_TAG)).InnerText);
                        ushort width = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, PEAK_WIDTH_CONFIG_TAG)).InnerText);
                        reperPeak = new Utility.PreciseEditorData(false, 255, step, collector, 0, width, 0, "checker peak");
                    } catch (NullReferenceException) {
                        //use hard-coded defaults (null checker peak)
                    } catch (FormatException) {
                        // TODO: very bad..
                        //use hard-coded defaults (null checker peak)
                    }
                    try {
                        iterations = int.Parse(xmlData.SelectSingleNode(combine(prefix, CHECK_ITER_NUMBER_CONFIG_TAG)).InnerText);
                    } catch (NullReferenceException) {
                        //use hard-coded defaults (infinite iterations)
                    }
                    try {
                        timeLimit = int.Parse(xmlData.SelectSingleNode(combine(prefix, CHECK_TIME_LIMIT_CONFIG_TAG)).InnerText);
                    } catch (NullReferenceException) {
                        //use hard-coded defaults (no time limit)
                    }
                    try {
                        allowedShift = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, CHECK_MAX_SHIFT_CONFIG_TAG)).InnerText);
                    } catch (NullReferenceException) {
                        //use hard-coded defaults (zero shift allowed)
                    }
                    // BAD: really uses previous values!
                }
                public XmlDocument XML {
                    get {
                        return xmlData;
                    }
                }
                #endregion
                private void loadMassCoeffs() {
                    // parse from already loaded config
                    XmlNode interfaceNode = xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, INTERFACE_CONFIG_TAG));
                    if (interfaceNode == null)
                        throw new ConfigLoadException("", "", filename);
                    try {
                        col1Coeff = double.Parse(interfaceNode.SelectSingleNode(C1_CONFIG_TAG).InnerText, CultureInfo.InvariantCulture);
                        col2Coeff = double.Parse(interfaceNode.SelectSingleNode(C2_CONFIG_TAG).InnerText, CultureInfo.InvariantCulture);
                    } catch (FormatException) {
                        throw new ConfigLoadException("", "", filename);
                    }
                }
            }
            private class LegacySpectrumReader: LegacyReader, ISpectrumReader {
                private bool hint;
                #region ISpectrumReader Members
                public bool Hint {
                    get {
                        return hint;
                    }
                    set {
                        hint = value;
                    }
                }
                public bool readSpectrum(out Graph graph) {
                    bool result;
                    ConfigLoadException resultException = null;
                    try {
                        result = hint ? OpenSpecterFile(out graph) : OpenPreciseSpecterFile(out graph);
                        if (result)
                            return hint;
                    } catch (ConfigLoadException cle) {
                        resultException = cle;
                    }
                    try {
                        result = (!hint) ? OpenSpecterFile(out graph) : OpenPreciseSpecterFile(out graph);
                        if (result)
                            return (hint = !hint);
                    } catch (ConfigLoadException cle) {
                        resultException = (resultException == null) ? cle : resultException;
                    }
                    throw resultException;
                }
                public Graph.Displaying openSpectrumFile(PointPairListPlus pl1, PointPairListPlus pl2, out CommonOptions commonOpts) {
                    XmlNode headerNode = null;
                    string prefix = "";

                    if (xmlData.SelectSingleNode(OVERVIEW_CONFIG_TAG) != null) {
                        headerNode = xmlData.SelectSingleNode(combine(OVERVIEW_CONFIG_TAG, HEADER_CONFIG_TAG));
                    } else if (xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, OVERVIEW_CONFIG_TAG)) != null) {
                        prefix = ROOT_CONFIG_TAG;
                        headerNode = xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, HEADER_CONFIG_TAG));
                    } else {
                        throw new ConfigLoadException("Ошибка структуры файла", "Ошибка чтения файла спектра", filename);
                    }

                    Graph.Displaying spectrumType = Graph.Displaying.Measured;
                    if (headerNode != null && headerNode.InnerText == DIFF_SPECTRUM_HEADER)
                        spectrumType = Graph.Displaying.Diff;

                    ushort X = 0;
                    long Y = 0;
                    try {
                        foreach (XmlNode pntNode in xmlData.SelectNodes(combine(prefix, OVERVIEW_CONFIG_TAG, COL1_CONFIG_TAG, POINT_CONFIG_TAG))) {
                            X = ushort.Parse(pntNode.SelectSingleNode(POINT_STEP_CONFIG_TAG).InnerText);
                            Y = long.Parse(pntNode.SelectSingleNode(POINT_COUNT_CONFIG_TAG).InnerText);
                            pl1.Add(X, Y);
                        }
                        foreach (XmlNode pntNode in xmlData.SelectNodes(combine(prefix, OVERVIEW_CONFIG_TAG, COL2_CONFIG_TAG, POINT_CONFIG_TAG))) {
                            X = ushort.Parse(pntNode.SelectSingleNode(POINT_STEP_CONFIG_TAG).InnerText);
                            Y = long.Parse(pntNode.SelectSingleNode(POINT_COUNT_CONFIG_TAG).InnerText);
                            pl2.Add(X, Y);
                        }
                    } catch (NullReferenceException) {
                        throw new ConfigLoadException("Ошибка структуры файла", "Ошибка чтения файла спектра", filename);
                    }
                    try {
                        commonOpts = new CommonOptions();
                        loadCommonOptions(commonOpts);
                    } catch (structureErrorOnLoadCommonData) {
                        commonOpts = null;
                    }
                    pl1.Sort(ZedGraph.SortType.XValues);
                    pl2.Sort(ZedGraph.SortType.XValues);
                    return spectrumType;
                }
                public bool openPreciseSpectrumFile(PreciseSpectrum peds) {
                    string prefix = "";
                    if (xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, SENSE_CONFIG_TAG)) != null)
                        prefix = ROOT_CONFIG_TAG;
                    else if (xmlData.SelectSingleNode(SENSE_CONFIG_TAG) == null) {
                        throw new ConfigLoadException("Ошибка структуры файла", "Ошибка чтения файла прецизионного спектра", filename);
                    }

                    CommonOptions co = new CommonOptions();
                    try {
                        loadCommonOptions(co);
                    } catch (structureErrorOnLoadCommonData) {
                        co = null;
                    }
                    peds.CommonOptions = co;

                    return LoadPED(xmlData, "Ошибка чтения файла прецизионного спектра", peds, prefix);
                }
                #endregion
                private bool OpenSpecterFile(out Graph graph) {
                    PointPairListPlus pl1 = new PointPairListPlus(), pl2 = new PointPairListPlus();
                    CommonOptions commonOpts;
                    Graph.Displaying result = openSpectrumFile(pl1, pl2, out commonOpts);

                    graph = new Graph(commonOpts);
                    switch (result) {
                        case Graph.Displaying.Measured:
                            graph.updateGraphAfterScanLoad(pl1, pl2);
                            return true;
                        case Graph.Displaying.Diff:
                            graph.updateGraphAfterScanDiff(pl1, pl2);
                            return true;
                        default:
                            return false;
                    }
                }
                private bool OpenPreciseSpecterFile(out Graph graph) {
                    PreciseSpectrum peds = new PreciseSpectrum();
                    bool result = openPreciseSpectrumFile(peds);
                    if (result) {
                        graph = new Graph(peds.CommonOptions);
                        graph.updateGraphAfterPreciseLoad(peds);
                    } else {
                        //TODO: other solution!
                        graph = null;
                    }
                    return result;
                }
                protected sealed override PointPairListPlus readPeaks(XmlNode regionNode) {
                    ushort X;
                    long Y;
                    PointPairListPlus tempPntLst = new PointPairListPlus();
                    try {
                        foreach (XmlNode pntNode in regionNode.SelectNodes(POINT_CONFIG_TAG)) {
                            X = ushort.Parse(pntNode.SelectSingleNode(POINT_STEP_CONFIG_TAG).InnerText);
                            Y = long.Parse(pntNode.SelectSingleNode(POINT_COUNT_CONFIG_TAG).InnerText);
                            tempPntLst.Add(X, Y);
                        }
                    } catch (FormatException) {
                        throw new ConfigLoadException("Неверный формат данных", "Ошибка чтения файла прецизионного спектра", filename);
                    }
                    return tempPntLst;
                }
            }
            #endregion
            private abstract class CurrentTagHolder: TagHolder {
                protected const char COUNTS_SEPARATOR = ' ';
            }
            #region Current Readers
            private abstract class CurrentReader: CurrentTagHolder {
                public void loadCommonOptions(CommonOptions opts) {
                    XmlNode commonNode = xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, COMMON_CONFIG_TAG));
                    try {
                        ushort eT, iT, iV, CP, eC, hC, fV1, fV2;

                        eT = ushort.Parse(commonNode.SelectSingleNode(EXPOSITURE_TIME_CONFIG_TAG).InnerText);
                        iT = ushort.Parse(commonNode.SelectSingleNode(TRANSITION_TIME_CONFIG_TAG).InnerText);
                        iV = ushort.Parse(commonNode.SelectSingleNode(IONIZATION_VOLTAGE_CONFIG_TAG).InnerText);
                        CP = ushort.Parse(commonNode.SelectSingleNode(CAPACITOR_VOLTAGE_COEFF_CONFIG_TAG).InnerText);
                        eC = ushort.Parse(commonNode.SelectSingleNode(EMISSION_CURRENT_CONFIG_TAG).InnerText);
                        hC = ushort.Parse(commonNode.SelectSingleNode(HEAT_CURRENT_CONFIG_TAG).InnerText);
                        fV1 = ushort.Parse(commonNode.SelectSingleNode(FOCUS_VOLTAGE1_CONFIG_TAG).InnerText);
                        fV2 = ushort.Parse(commonNode.SelectSingleNode(FOCUS_VOLTAGE2_CONFIG_TAG).InnerText);

                        opts.eTime = eT;
                        opts.iTime = iT;
                        opts.iVoltage = iV;
                        opts.CP = CP;
                        opts.eCurrent = eC;
                        opts.hCurrent = hC;
                        opts.fV1 = fV1;
                        opts.fV2 = fV2;
                    } catch (NullReferenceException) {
                        throw new structureErrorOnLoadCommonData(filename);
                    }
                    loadDelays(commonNode, opts);
                }
                public List<Utility.PreciseEditorData> loadPreciseData() {
                    List<Utility.PreciseEditorData> peds = new List<Utility.PreciseEditorData>();
                    if (LoadPED(xmlData, "", peds))
                        return peds;
                    return null;
                }
                protected bool LoadPED(XmlDocument pedConf, string errorMessage, List<Utility.PreciseEditorData> peds) {
                    string prefix = combine(ROOT_CONFIG_TAG, SENSE_CONFIG_TAG);
                    for (int i = 1; i <= 20; ++i) {
                        string peak, iter, width, col;
                        try {
                            XmlNode regionNode = pedConf.SelectSingleNode(combine(prefix, string.Format(PEAK_TAGS_FORMAT, i)));
                            peak = regionNode.SelectSingleNode(PEAK_NUMBER_CONFIG_TAG).InnerText;
                            col = regionNode.SelectSingleNode(PEAK_COL_NUMBER_CONFIG_TAG).InnerText;
                            iter = regionNode.SelectSingleNode(PEAK_ITER_NUMBER_CONFIG_TAG).InnerText;
                            width = regionNode.SelectSingleNode(PEAK_WIDTH_CONFIG_TAG).InnerText;
                            bool allFilled = ((peak != "") && (iter != "") && (width != "") && (col != ""));
                            if (allFilled) {
                                string comment;
                                try {
                                    comment = regionNode.SelectSingleNode(PEAK_COMMENT_CONFIG_TAG).InnerText;
                                } catch (NullReferenceException) {
                                    comment = "";
                                }
                                try {
                                    bool use = bool.Parse(regionNode.SelectSingleNode(PEAK_USE_CONFIG_TAG).InnerText);
                                    ushort peakStep = ushort.Parse(peak);
                                    ushort peakWidth = ushort.Parse(width);
                                    Utility.PreciseEditorData temp = new Utility.PreciseEditorData(use, (byte)(i - 1), peakStep,
                                                                        byte.Parse(col), ushort.Parse(iter),
                                                                        ushort.Parse(width), (float)0, comment);
                                    temp.AssociatedPoints = readPeaks(regionNode, peakStep, peakWidth);
                                    peds.Add(temp);
                                } catch (FormatException) {
                                    throw new ConfigLoadException("Неверный формат данных", errorMessage, filename);
                                }
                            }
                        } catch (NullReferenceException) {
                            throw new ConfigLoadException("Ошибка структуры файла", errorMessage, filename);
                        }
                    }
                    peds.Sort(Utility.ComparePreciseEditorData);
                    return true;
                }
                protected virtual PointPairListPlus readPeaks(XmlNode regionNode, ushort peakStep, ushort peakWidth) { return null; }
                protected virtual void loadDelays(XmlNode commonNode, CommonOptions opts) {}
            }
            private class CurrentCommonOptionsReader: CurrentReader, ICommonOptionsReader {}
            private class CurrentPreciseDataReader: CurrentReader, IPreciseDataReader {}
            private class CurrentMainConfig: CurrentReader, IMainConfig {
                #region IMainConfig implementation
                public void read() {
                    string prefix;
                    try {
                        prefix = combine(ROOT_CONFIG_TAG, CONNECT_CONFIG_TAG);
                        SerialPort = (xmlData.SelectSingleNode(combine(prefix, PORT_CONFIG_TAG)).InnerText);
                        SerialBaudRate = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, BAUDRATE_CONFIG_TAG)).InnerText);
                        sendTry = byte.Parse(xmlData.SelectSingleNode(combine(prefix, TRY_NUMBER_CONFIG_TAG)).InnerText);
                    } catch (NullReferenceException) {
                        (new ConfigLoadException("Ошибка структуры конфигурационного файла", "Ошибка чтения конфигурационного файла", filename)).visualise();
                        //use hard-coded defaults
                    }
                    try {
                        prefix = combine(ROOT_CONFIG_TAG, OVERVIEW_CONFIG_TAG);
                        sPoint = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, START_SCAN_CONFIG_TAG)).InnerText);
                        ePoint = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, END_SCAN_CONFIG_TAG)).InnerText);
                    } catch (NullReferenceException) {
                        (new ConfigLoadException("Ошибка структуры конфигурационного файла", "Ошибка чтения конфигурационного файла", filename)).visualise();
                        //use hard-coded defaults
                    }
                    try {
                        loadMassCoeffs();
                    } catch (ConfigLoadException) {
                        //cle.visualise();
                        //use hard-coded defaults
                    }
                    try {
                        loadCommonOptions(commonOpts);
                    } catch (ConfigLoadException cle) {
                        cle.visualise();
                        //use hard-coded defaults
                    }
                    try {
                        List<Utility.PreciseEditorData> pedl = loadPreciseData();
                        if ((pedl != null) && (pedl.Count > 0)) {
                            //BAD!!! cleaning previous points!!!
                            preciseData.Clear();
                            preciseData.AddRange(pedl);
                        }
                    } catch (ConfigLoadException cle) {
                        cle.visualise();
                        //use empty default ped
                    }
                    prefix = combine(ROOT_CONFIG_TAG, CHECK_CONFIG_TAG);
                    try {
                        ushort step = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, PEAK_NUMBER_CONFIG_TAG)).InnerText);
                        byte collector = byte.Parse(xmlData.SelectSingleNode(combine(prefix, PEAK_COL_NUMBER_CONFIG_TAG)).InnerText);
                        ushort width = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, PEAK_WIDTH_CONFIG_TAG)).InnerText);
                        reperPeak = new Utility.PreciseEditorData(false, 255, step, collector, 0, width, 0, "checker peak");
                    } catch (NullReferenceException) {
                        //use hard-coded defaults (null checker peak)
                    } catch (FormatException) {
                        // TODO: very bad..
                        //use hard-coded defaults (null checker peak)
                    }
                    try {
                        iterations = int.Parse(xmlData.SelectSingleNode(combine(prefix, CHECK_ITER_NUMBER_CONFIG_TAG)).InnerText);
                    } catch (NullReferenceException) {
                        //use hard-coded defaults (infinite iterations)
                    }
                    try {
                        timeLimit = int.Parse(xmlData.SelectSingleNode(combine(prefix, CHECK_TIME_LIMIT_CONFIG_TAG)).InnerText);
                    } catch (NullReferenceException) {
                        //use hard-coded defaults (no time limit)
                    }
                    try {
                        allowedShift = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, CHECK_MAX_SHIFT_CONFIG_TAG)).InnerText);
                    } catch (NullReferenceException) {
                        //use hard-coded defaults (zero shift allowed)
                    }
                    // BAD: really uses previous values!
                }
                public XmlDocument XML {
                    get {
                        return xmlData;
                    }
                }
                #endregion
                private void loadMassCoeffs() {
                    try {
                        XmlNode interfaceNode = xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, INTERFACE_CONFIG_TAG));
                        col1Coeff = double.Parse(interfaceNode.SelectSingleNode(C1_CONFIG_TAG).InnerText, CultureInfo.InvariantCulture);
                        col2Coeff = double.Parse(interfaceNode.SelectSingleNode(C2_CONFIG_TAG).InnerText, CultureInfo.InvariantCulture);
                    } catch (NullReferenceException) {
                        throw new ConfigLoadException("Ошибка структуры конфигурационного файла", "Ошибка чтения конфигурационного файла", filename);
                    } catch (FormatException) {
                        throw new ConfigLoadException("Неверный формат данных", "Ошибка чтения конфигурационного файла", filename);
                    }
                }
                protected override void loadDelays(XmlNode commonNode, CommonOptions opts) {
                    try {
                        ushort befT, fT, bT;
                        bool fAsbef;

                        befT = ushort.Parse(commonNode.SelectSingleNode(DELAY_BEFORE_MEASURE_CONFIG_TAG).InnerText);
                        fT = ushort.Parse(commonNode.SelectSingleNode(DELAY_FORWARD_MEASURE_CONFIG_TAG).InnerText);
                        bT = ushort.Parse(commonNode.SelectSingleNode(DELAY_BACKWARD_MEASURE_CONFIG_TAG).InnerText);
                        fAsbef = bool.Parse(commonNode.SelectSingleNode(EQUAL_DELAYS_CONFIG_TAG).InnerText);

                        opts.befTime = befT;
                        opts.ForwardTimeEqualsBeforeTime = fAsbef;
                        opts.fTime = fT;
                        opts.bTime = bT;
                    } catch (NullReferenceException) {
                        //Use hard-coded defaults
                        return;
                    }
                }
            }
            private class CurrentSpectrumReader: CurrentReader, ISpectrumReader {
                private bool hint;
                #region ISpectrumReader Members
                public bool Hint {
                    get {
                        return hint;
                    }
                    set {
                        hint = value;
                    }
                }
                public bool readSpectrum(out Graph graph) {
                    bool result;
                    ConfigLoadException resultException = null;
                    try {
                        result = hint ? OpenSpecterFile(out graph) : OpenPreciseSpecterFile(out graph);
                        if (result)
                            return hint;
                    } catch (ConfigLoadException cle) {
                        resultException = cle;
                    }
                    try {
                        result = (!hint) ? OpenSpecterFile(out graph) : OpenPreciseSpecterFile(out graph);
                        if (result)
                            return (hint = !hint);
                    } catch (ConfigLoadException cle) {
                        resultException = (resultException == null) ? cle : resultException;
                    }
                    throw resultException;
                }
                public Graph.Displaying openSpectrumFile(PointPairListPlus pl1, PointPairListPlus pl2, out CommonOptions commonOpts) {
                    XmlNode headerNode = xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, HEADER_CONFIG_TAG));
                    string prefix = combine(ROOT_CONFIG_TAG, OVERVIEW_CONFIG_TAG);

                    Graph.Displaying spectrumType = Graph.Displaying.Measured;
                    if (headerNode != null && headerNode.InnerText == DIFF_SPECTRUM_HEADER)
                        spectrumType = Graph.Displaying.Diff;

                    ushort X = 0;
                    long Y = 0;
                    try {
                        foreach (XmlNode pntNode in xmlData.SelectNodes(combine(prefix, COL1_CONFIG_TAG, POINT_CONFIG_TAG))) {
                            X = ushort.Parse(pntNode.SelectSingleNode(POINT_STEP_CONFIG_TAG).InnerText);
                            Y = long.Parse(pntNode.SelectSingleNode(POINT_COUNT_CONFIG_TAG).InnerText);
                            pl1.Add(X, Y);
                        }
                        foreach (XmlNode pntNode in xmlData.SelectNodes(combine(prefix, COL2_CONFIG_TAG, POINT_CONFIG_TAG))) {
                            X = ushort.Parse(pntNode.SelectSingleNode(POINT_STEP_CONFIG_TAG).InnerText);
                            Y = long.Parse(pntNode.SelectSingleNode(POINT_COUNT_CONFIG_TAG).InnerText);
                            pl2.Add(X, Y);
                        }
                    } catch (NullReferenceException) {
                        throw new ConfigLoadException("Ошибка структуры файла", "Ошибка чтения файла спектра", filename);
                    }
                    //the whole logic of displaying spectra must be modified
                    //!!!!!!!!!!!!!!!!!!!!!!!!
                    try {
                        commonOpts = new CommonOptions();
                        loadCommonOptions(commonOpts);
                    } catch (structureErrorOnLoadCommonData) {
                        commonOpts = null;
                    }
                    //!!!!!!!!!!!!!!!!!!!!!!!!
                    pl1.Sort(ZedGraph.SortType.XValues);
                    pl2.Sort(ZedGraph.SortType.XValues);
                    return spectrumType;
                }
                public bool openPreciseSpectrumFile(PreciseSpectrum peds) {
                    CommonOptions co = new CommonOptions();
                    try {
                        loadCommonOptions(co);
                    } catch (structureErrorOnLoadCommonData) {
                        co = null;
                    }
                    peds.CommonOptions = co;

                    return LoadPED(xmlData, "Ошибка чтения файла прецизионного спектра", peds);
                }
                #endregion
                private bool OpenSpecterFile(out Graph graph) {
                    PointPairListPlus pl1 = new PointPairListPlus(), pl2 = new PointPairListPlus();
                    CommonOptions commonOpts;
                    Graph.Displaying result = openSpectrumFile(pl1, pl2, out commonOpts);

                    graph = new Graph(commonOpts);
                    switch (result) {
                        case Graph.Displaying.Measured:
                            graph.updateGraphAfterScanLoad(pl1, pl2);
                            return true;
                        case Graph.Displaying.Diff:
                            graph.updateGraphAfterScanDiff(pl1, pl2);
                            return true;
                        default:
                            return false;
                    }
                }
                private bool OpenPreciseSpecterFile(out Graph graph) {
                    PreciseSpectrum peds = new PreciseSpectrum();
                    bool result = openPreciseSpectrumFile(peds);
                    if (result) {
                        graph = new Graph(peds.CommonOptions);
                        graph.updateGraphAfterPreciseLoad(peds);
                    } else {
                        //TODO: other solution!
                        graph = null;
                    }
                    return result;
                }
                protected sealed override PointPairListPlus readPeaks(XmlNode regionNode, ushort peakStep, ushort peakWidth) {
                    peakStep -= peakWidth;
                    peakWidth += peakWidth;
                    peakWidth += peakStep;
                    long Y;
                    PointPairListPlus tempPntLst = new PointPairListPlus();
                    try {
                        foreach (string str in regionNode.SelectSingleNode(POINT_CONFIG_TAG).InnerText.Split(COUNTS_SEPARATOR)) {
                            // locale?
                            Y = long.Parse(str);
                            tempPntLst.Add(peakStep, Y);
                            ++peakStep;
                        }
                    } catch (FormatException) {
                        throw new ConfigLoadException("Неверный формат данных", "Ошибка чтения файла прецизионного спектра", filename);
                    }
                    if (--peakStep != peakWidth)
                        throw new ConfigLoadException("Несовпадение рядов данных", "Ошибка чтения файла прецизионного спектра", filename);
                    
                    return tempPntLst;
                }
            }
            #endregion
            #region Current Writers
            private abstract class CurrentWriter: CurrentTagHolder {
                public void setTimeStamp(DateTime dt) {
                    XmlAttribute attr = xmlData.CreateAttribute(TIME_SPECTRUM_ATTRIBUTE);
                    attr.Value = dt.ToString("G", DateTimeFormatInfo.InvariantInfo);
                    xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, HEADER_CONFIG_TAG)).Attributes.Append(attr);
                }
                public void setShift(short shift) {
                    XmlAttribute attr = xmlData.CreateAttribute(SHIFT_SPECTRUM_ATTRIBUTE);
                    attr.Value = shift.ToString();
                    xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, HEADER_CONFIG_TAG)).Attributes.Append(attr);
                }
                public virtual void write() {
                    xmlData.Save(filename);
                }
                public void saveCommonOptions(CommonOptions opts) {
                    XmlNode commonNode = xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, COMMON_CONFIG_TAG));
                    saveCommonOptions(commonNode, opts);
                }
                public void saveCommonOptions(ushort eT, ushort iT, double iV, double cp, double eC, double hC, double fv1, double fv2) {
                    CommonOptions opts = new CommonOptions();
                    opts.eTimeReal = eT;
                    opts.iTimeReal = iT;
                    opts.iVoltageReal = iV;
                    opts.CPReal = cp;
                    opts.eCurrentReal = eC;
                    opts.hCurrentReal = hC;
                    opts.fV1Real = fv1;
                    opts.fV2Real = fv2;
                    saveCommonOptions(opts);
                }
                private void saveCommonOptions(XmlNode commonNode, CommonOptions opts) {
                    commonNode.SelectSingleNode(EXPOSITURE_TIME_CONFIG_TAG).InnerText = opts.eTime.ToString();
                    commonNode.SelectSingleNode(TRANSITION_TIME_CONFIG_TAG).InnerText = opts.iTime.ToString();
                    commonNode.SelectSingleNode(IONIZATION_VOLTAGE_CONFIG_TAG).InnerText = opts.iVoltage.ToString();
                    commonNode.SelectSingleNode(CAPACITOR_VOLTAGE_COEFF_CONFIG_TAG).InnerText = opts.CP.ToString();
                    commonNode.SelectSingleNode(EMISSION_CURRENT_CONFIG_TAG).InnerText = opts.eCurrent.ToString();
                    commonNode.SelectSingleNode(HEAT_CURRENT_CONFIG_TAG).InnerText = opts.hCurrent.ToString();
                    commonNode.SelectSingleNode(FOCUS_VOLTAGE1_CONFIG_TAG).InnerText = opts.fV1.ToString();
                    commonNode.SelectSingleNode(FOCUS_VOLTAGE2_CONFIG_TAG).InnerText = opts.fV2.ToString();
                    /*commonNode.SelectSingleNode(DELAY_BEFORE_MEASURE_CONFIG_TAG).InnerText = Config.commonOpts.befTime.ToString();
                    commonNode.SelectSingleNode(EQUAL_DELAYS_CONFIG_TAG).InnerText = Config.commonOpts.ForwardTimeEqualsBeforeTime.ToString();
                    commonNode.SelectSingleNode(DELAY_FORWARD_MEASURE_CONFIG_TAG).InnerText = Config.commonOpts.fTime.ToString();
                    commonNode.SelectSingleNode(DELAY_BACKWARD_MEASURE_CONFIG_TAG).InnerText = Config.commonOpts.bTime.ToString();*/
                }
                protected void saveScanOptions() {
                    string prefix = combine(ROOT_CONFIG_TAG, OVERVIEW_CONFIG_TAG);
                    fillInnerText(prefix, START_SCAN_CONFIG_TAG, sPoint);
                    fillInnerText(prefix, END_SCAN_CONFIG_TAG, ePoint);
                }
                public void savePreciseData(List<Utility.PreciseEditorData> peds, bool savePeakSum) {
                    clearOldValues();
                    foreach (Utility.PreciseEditorData ped in peds) {
                        XmlNode regionNode = xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, SENSE_CONFIG_TAG, string.Format(PEAK_TAGS_FORMAT, ped.pNumber + 1)));
                        regionNode.SelectSingleNode(PEAK_NUMBER_CONFIG_TAG).InnerText = ped.Step.ToString();
                        regionNode.SelectSingleNode(PEAK_ITER_NUMBER_CONFIG_TAG).InnerText = ped.Iterations.ToString();
                        regionNode.SelectSingleNode(PEAK_WIDTH_CONFIG_TAG).InnerText = ped.Width.ToString();
                        regionNode.SelectSingleNode(PEAK_PRECISION_CONFIG_TAG).InnerText = ped.Precision.ToString();
                        regionNode.SelectSingleNode(PEAK_COL_NUMBER_CONFIG_TAG).InnerText = ped.Collector.ToString();
                        regionNode.SelectSingleNode(PEAK_COMMENT_CONFIG_TAG).InnerText = ped.Comment;
                        regionNode.SelectSingleNode(PEAK_USE_CONFIG_TAG).InnerText = ped.Use.ToString();

                        if (ped.AssociatedPoints == null) {
                            continue;
                        }

                        if (savePeakSum) {
                            regionNode.AppendChild(xmlData.CreateElement(PEAK_COUNT_SUM_CONFIG_TAG)).InnerText = ped.AssociatedPoints.PLSreference.PeakSum.ToString();
                        }
                        savePointRows(ped.AssociatedPoints, regionNode);
                    }
                }
                protected virtual void clearOldValues() {}
                protected virtual void savePointRows(PointPairListPlus row, XmlNode node) {}
            }
            private class CurrentCommonOptionsWriter: CurrentWriter, ICommonOptionsWriter {}
            private class CurrentPreciseDataWriter: CurrentWriter, IPreciseDataWriter {}
            private class CurrentSpectrumWriter: CurrentWriter, ISpectrumWriter {
                protected sealed override void savePointRows(PointPairListPlus row, XmlNode node) {
                    StringBuilder sb = new StringBuilder();
                    foreach (ZedGraph.PointPair pp in row) {
                        sb.Append(pp.Y);
                        sb.Append(COUNTS_SEPARATOR);
                    }
                    XmlNode temp = xmlData.CreateElement(POINT_CONFIG_TAG); ; 
                    temp.InnerText = sb.ToString(0, sb.Length - 1);
                    node.AppendChild(temp);
                }
            }
            private class CurrentMainConfigWriter: CurrentWriter, IMainConfigWriter {
                #region IAnyWriter Members
                public override void write() {
                    saveConnectOptions();
                    saveScanOptions();
                    saveCommonOptions();
                    saveDelaysOptions();
                    saveMassCoeffs();
                    saveCheckOptions();
                    SavePreciseOptions();
                    base.write();
                }
                #endregion
                private void saveConnectOptions() {
                    string prefix = combine(ROOT_CONFIG_TAG, CONNECT_CONFIG_TAG);
                    fillInnerText(prefix, PORT_CONFIG_TAG, Port);
                    fillInnerText(prefix, BAUDRATE_CONFIG_TAG, BaudRate);
                    fillInnerText(prefix, TRY_NUMBER_CONFIG_TAG, sendTry);
                }
                private void saveCommonOptions() {
                    saveCommonOptions(commonOpts);
                }
                private void saveDelaysOptions() {
                    string prefix = combine(ROOT_CONFIG_TAG, COMMON_CONFIG_TAG);
                    fillInnerText(prefix, DELAY_BEFORE_MEASURE_CONFIG_TAG, commonOpts.befTime);
                    fillInnerText(prefix, EQUAL_DELAYS_CONFIG_TAG, commonOpts.ForwardTimeEqualsBeforeTime);
                    fillInnerText(prefix, DELAY_FORWARD_MEASURE_CONFIG_TAG, commonOpts.fTime);
                    fillInnerText(prefix, DELAY_BACKWARD_MEASURE_CONFIG_TAG, commonOpts.bTime);
                }
                private void saveMassCoeffs() {
                    clearInnerText(ROOT_CONFIG_TAG, INTERFACE_CONFIG_TAG);
                    string prefix = combine(ROOT_CONFIG_TAG, INTERFACE_CONFIG_TAG);
                    fillInnerText(prefix, C1_CONFIG_TAG, col1Coeff.ToString("R", CultureInfo.InvariantCulture));
                    fillInnerText(prefix, C2_CONFIG_TAG, col2Coeff.ToString("R", CultureInfo.InvariantCulture));
                }
                private void saveCheckOptions() {
                    //checkpeak & iterations
                    string prefix = combine(ROOT_CONFIG_TAG, CHECK_CONFIG_TAG);
                    if (xmlData.SelectSingleNode(prefix) == null) {
                        XmlNode checkRegion = xmlData.CreateElement(CHECK_CONFIG_TAG);
                        xmlData.SelectSingleNode(ROOT_CONFIG_TAG).AppendChild(checkRegion);
                    }
                    if (reperPeak != null) {
                        fillInnerText(prefix, PEAK_NUMBER_CONFIG_TAG, reperPeak.Step);
                        fillInnerText(prefix, PEAK_COL_NUMBER_CONFIG_TAG, reperPeak.Collector);
                        fillInnerText(prefix, PEAK_WIDTH_CONFIG_TAG, reperPeak.Width);
                    } else {
                        clearInnerText(prefix, PEAK_NUMBER_CONFIG_TAG);
                        clearInnerText(prefix, PEAK_COL_NUMBER_CONFIG_TAG);
                        clearInnerText(prefix, PEAK_WIDTH_CONFIG_TAG);
                    }
                    fillInnerText(prefix, CHECK_ITER_NUMBER_CONFIG_TAG, iterations);
                    fillInnerText(prefix, CHECK_TIME_LIMIT_CONFIG_TAG, timeLimit);
                    fillInnerText(prefix, CHECK_MAX_SHIFT_CONFIG_TAG, allowedShift);
                }
                private void SavePreciseOptions() {
                    savePreciseData(PreciseData, false);
                }
                protected override void clearOldValues() {
                    for (int i = 1; i <= 20; ++i) {
                        string prefix = combine(ROOT_CONFIG_TAG, SENSE_CONFIG_TAG, string.Format(PEAK_TAGS_FORMAT, i));
                        clearInnerText(prefix, PEAK_NUMBER_CONFIG_TAG);
                        clearInnerText(prefix, PEAK_ITER_NUMBER_CONFIG_TAG);
                        clearInnerText(prefix, PEAK_WIDTH_CONFIG_TAG);
                        clearInnerText(prefix, PEAK_PRECISION_CONFIG_TAG);
                        clearInnerText(prefix, PEAK_COL_NUMBER_CONFIG_TAG);
                        clearInnerText(prefix, PEAK_COMMENT_CONFIG_TAG);
                        clearInnerText(prefix, PEAK_USE_CONFIG_TAG);
                    }
                }
            }
            #endregion
            #region Static Getters
            public static ISpectrumReader getSpectrumReader(string confName, bool hint) {
                ISpectrumReader reader = findCorrespondingReaderVersion<ISpectrumReader, LegacySpectrumReader, CurrentSpectrumReader>(confName, "Ошибка чтения файла спектра");
                reader.Hint = hint;
                return reader;
            }
            public static ISpectrumWriter getSpectrumWriter(string confName, Graph graph) {
                XmlDocument doc = new XmlDocument();
                string header = graph.DisplayingMode == Graph.Displaying.Diff ? DIFF_SPECTRUM_HEADER : MEASURED_SPECTRUM_HEADER;
                XmlNode rootNode = createRootStub(doc, header);

                if (graph.isPreciseSpectrum)
                    createPEDStub(doc, rootNode);
                else {
                    XmlNode scanNode = rootNode.AppendChild(doc.CreateElement(OVERVIEW_CONFIG_TAG));
                    XmlElement temp;
                    if (graph.DisplayingMode == Graph.Displaying.Measured) {
                        temp = doc.CreateElement(START_SCAN_CONFIG_TAG);
                        temp.InnerText = sPoint.ToString();
                        scanNode.AppendChild(temp);
                        temp = doc.CreateElement(END_SCAN_CONFIG_TAG);
                        temp.InnerText = ePoint.ToString();
                        scanNode.AppendChild(temp);
                        // In case of loaded (not auto) start/end points and measure parameters are not connected to spectrum data..
                    }
                    scanNode.AppendChild(doc.CreateElement(COL1_CONFIG_TAG));
                    scanNode.AppendChild(doc.CreateElement(COL2_CONFIG_TAG));
                    foreach (ZedGraph.PointPair pp in graph.Displayed1Steps[0]) {
                        temp = doc.CreateElement(POINT_CONFIG_TAG);
                        temp.AppendChild(doc.CreateElement(POINT_STEP_CONFIG_TAG)).InnerText = pp.X.ToString();
                        temp.AppendChild(doc.CreateElement(POINT_COUNT_CONFIG_TAG)).InnerText = ((long)(pp.Y)).ToString();
                        scanNode.SelectSingleNode(COL1_CONFIG_TAG).AppendChild(temp);
                    }
                    foreach (ZedGraph.PointPair pp in graph.Displayed2Steps[0]) {
                        temp = doc.CreateElement(POINT_CONFIG_TAG);
                        temp.AppendChild(doc.CreateElement(POINT_STEP_CONFIG_TAG)).InnerText = pp.X.ToString();
                        temp.AppendChild(doc.CreateElement(POINT_COUNT_CONFIG_TAG)).InnerText = ((long)(pp.Y)).ToString();
                        scanNode.SelectSingleNode(COL2_CONFIG_TAG).AppendChild(temp);
                    }
                }
                ISpectrumWriter writer = getInitializedConfig<ISpectrumWriter, CurrentSpectrumWriter>(confName, doc);
                if (graph.CommonOptions != null) {
                    // TODO: do not allow saving! data lack for properly save in new formats
                    createCommonOptsStub(doc, rootNode);
                    writer.saveCommonOptions(graph.CommonOptions);
                }
                return writer;
            }
            public static ICommonOptionsReader getCommonOptionsReader(string confName) {
                return findCorrespondingReaderVersion<ICommonOptionsReader, LegacyCommonOptionsReader, CurrentCommonOptionsReader>(confName, "Ошибка чтения файла общих настроек");
            }
            public static IPreciseDataReader getPreciseDataReader(string confName) {
                return findCorrespondingReaderVersion<IPreciseDataReader, LegacyPreciseDataReader, CurrentPreciseDataReader>(confName, "Ошибка чтения файла прецизионных точек");
            }
            public static IMainConfig getMainConfig(string confName) {
                return findCorrespondingReaderVersion<IMainConfig, LegacyMainConfig, CurrentMainConfig>(confName, "Ошибка чтения конфигурационного файла");
            }
            public static ICommonOptionsWriter getCommonOptionsWriter(string confName) {
                XmlDocument doc = new XmlDocument();
                createCommonOptsStub(doc, createRootStub(doc, COMMON_OPTIONS_HEADER));
                return getInitializedConfig<ICommonOptionsWriter, CurrentCommonOptionsWriter>(confName, doc);
            }
            public static IPreciseDataWriter getPreciseDataWriter(string confName, string header) {
                XmlDocument doc = new XmlDocument();
                createPEDStub(doc, createRootStub(doc, header));
                return getInitializedConfig<IPreciseDataWriter, CurrentPreciseDataWriter>(confName, doc);
            }
            public static IMainConfigWriter getMainConfigWriter(string confName, XmlDocument doc) {
                return getInitializedConfig<IMainConfigWriter, CurrentMainConfigWriter>(confName, doc);
            }
            #endregion
            #region Private Service Methods
            private static RETURN_INTERFACE findCorrespondingReaderVersion<RETURN_INTERFACE, LEGACY_TYPE, TYPE0>(string filename, string errorMessage)
                where RETURN_INTERFACE: IAnyReader
                where LEGACY_TYPE: TagHolder, RETURN_INTERFACE, new()
                where TYPE0: TagHolder, RETURN_INTERFACE, new() {
                XmlDocument doc = new XmlDocument();
                try {
                    doc.Load(filename);
                } catch (Exception Error) {
                    throw new ConfigLoadException(Error.Message, errorMessage, filename);
                }
                XmlNode rootNode = doc.SelectSingleNode(ROOT_CONFIG_TAG);
                if (rootNode == null) {
                    return getInitializedConfig<RETURN_INTERFACE, LEGACY_TYPE>(filename, doc);
                }
                XmlNode version = rootNode.Attributes.GetNamedItem(VERSION_ATTRIBUTE);
                if (version == null) {
                    return getInitializedConfig<RETURN_INTERFACE, LEGACY_TYPE>(filename, doc);
                }
                string versionText = version.Value;
                switch (versionText) {
                    case CONFIG_VERSION:
                        return getInitializedConfig<RETURN_INTERFACE, TYPE0>(filename, doc);
                    default:
                        // try load anyway
                        return getInitializedConfig<RETURN_INTERFACE, LEGACY_TYPE>(filename, doc);
                }
            }
            private static RETURN_INTERFACE getInitializedConfig<RETURN_INTERFACE, TYPE>(string filename, XmlDocument doc)                
                where RETURN_INTERFACE: IAnyConfig
                where TYPE: TagHolder, RETURN_INTERFACE, new() {
                RETURN_INTERFACE config;
                config = new TYPE();
                (config as TagHolder).initialize(filename, doc);
                return config;
            }
            private static string combine(params string[] args) {
                return string.Join("/", args);
            }
            private void clearInnerText(string prefix, string nodeName) {
                fillInnerText(prefix, nodeName, "");
            }
            private void fillInnerText(string prefix, string nodeName, object value) {
                string fullName = combine(prefix, nodeName);
                try {
                    xmlData.SelectSingleNode(fullName).InnerText = value.ToString();
                } catch (NullReferenceException) {
                    xmlData.SelectSingleNode(prefix).AppendChild(xmlData.CreateElement(nodeName));
                    xmlData.SelectSingleNode(fullName).InnerText = value.ToString();
                }
            }
            private static XmlNode createRootStub(XmlDocument conf, string header) {
                conf.AppendChild(conf.CreateXmlDeclaration("1.0", "utf-8", ""));
                XmlElement rootNode = conf.CreateElement(ROOT_CONFIG_TAG);
                conf.AppendChild(rootNode);

                XmlAttribute attr = conf.CreateAttribute(VERSION_ATTRIBUTE);
                attr.Value = CONFIG_VERSION;
                rootNode.Attributes.Append(attr);

                XmlElement headerNode = conf.CreateElement(HEADER_CONFIG_TAG);
                headerNode.InnerText = header;
                rootNode.AppendChild(headerNode);

                return rootNode;
            }
            private static XmlNode createCommonOptsStub(XmlDocument conf, XmlNode mountPoint) {
                XmlNode commonNode = conf.CreateElement(COMMON_CONFIG_TAG);
                //commonNode.AppendChild(conf.CreateElement(HEADER_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(EXPOSITURE_TIME_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(TRANSITION_TIME_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(IONIZATION_VOLTAGE_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(CAPACITOR_VOLTAGE_COEFF_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(EMISSION_CURRENT_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(HEAT_CURRENT_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(FOCUS_VOLTAGE1_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(FOCUS_VOLTAGE2_CONFIG_TAG));

                /*commonNode.AppendChild(conf.CreateElement(DELAY_BEFORE_MEASURE_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(EQUAL_DELAYS_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(DELAY_FORWARD_MEASURE_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(DELAY_BACKWARD_MEASURE_CONFIG_TAG));*/
                mountPoint.AppendChild(commonNode);
                return commonNode;
            }
            private static XmlNode createPEDStub(XmlDocument pedConf, XmlNode mountPoint) {
                XmlNode senseNode = pedConf.CreateElement(SENSE_CONFIG_TAG);

                for (int i = 1; i <= 20; ++i) {
                    XmlNode tempRegion = pedConf.CreateElement(string.Format(PEAK_TAGS_FORMAT, i));
                    tempRegion.AppendChild(pedConf.CreateElement(PEAK_NUMBER_CONFIG_TAG));
                    tempRegion.AppendChild(pedConf.CreateElement(PEAK_COL_NUMBER_CONFIG_TAG));
                    tempRegion.AppendChild(pedConf.CreateElement(PEAK_ITER_NUMBER_CONFIG_TAG));
                    tempRegion.AppendChild(pedConf.CreateElement(PEAK_WIDTH_CONFIG_TAG));
                    tempRegion.AppendChild(pedConf.CreateElement(PEAK_PRECISION_CONFIG_TAG));
                    tempRegion.AppendChild(pedConf.CreateElement(PEAK_COMMENT_CONFIG_TAG));
                    tempRegion.AppendChild(pedConf.CreateElement(PEAK_USE_CONFIG_TAG));
                    senseNode.AppendChild(tempRegion);
                }
                mountPoint.AppendChild(senseNode);
                return senseNode;
            }
            #endregion
        }
        #endregion
    }
}