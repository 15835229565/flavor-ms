using System;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;

namespace Flavor.Common {
    static class Config {
        private static XmlDocument _conf;

        private static readonly string INITIAL_DIR = System.IO.Directory.GetCurrentDirectory();

        private const string CONFIG_NAME = "config.xml";
        private const string CRASH_LOG_NAME = "MScrash.log";

        private static string confName;
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
        private const string MONITOR_SPECTRUM_HEADER = "Monitor";
        internal const string PRECISE_OPTIONS_HEADER = "Precise options";
        private const string COMMON_OPTIONS_HEADER = "Common options";
        private const string MEASURED_SPECTRUM_HEADER = "Measure";
        private const string DIFF_SPECTRUM_HEADER = "Diff";
        #endregion
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
            confName = System.IO.Path.Combine(INITIAL_DIR, CONFIG_NAME);
            logName = System.IO.Path.Combine(INITIAL_DIR, CRASH_LOG_NAME);
        }

        private static void fillInnerText(string prefix, string nodeName) {
            fillInnerText(prefix, nodeName, "");
        }
        private static void fillInnerText(string prefix, string nodeName, object value) {
            string fullName = combine(prefix, nodeName);
            try {
                _conf.SelectSingleNode(fullName).InnerText = value.ToString();
            } catch (NullReferenceException) {
                _conf.SelectSingleNode(prefix).AppendChild(_conf.CreateElement(nodeName));
                _conf.SelectSingleNode(fullName).InnerText = value.ToString();
            }
        }
        private static string combine(params string[] args) {
            return string.Join("/", args);
        }

        internal static void loadConfig() {
            IMainConfig conf = TagHolder.getMainConfig(confName);
            _conf = conf.Config;
            conf.readConfig();
            /*string prefix;
            try {
                _conf.Load(confName);
            } catch (Exception Error) {
                throw new ConfigLoadException(Error.Message, "Ошибка чтения конфигурационного файла", confName);
            }
            try {
                prefix = combine(ROOT_CONFIG_TAG, CONNECT_CONFIG_TAG);
                SerialPort = (_conf.SelectSingleNode(combine(prefix, PORT_CONFIG_TAG)).InnerText);
                SerialBaudRate = ushort.Parse(_conf.SelectSingleNode(combine(prefix, BAUDRATE_CONFIG_TAG)).InnerText);
                sendTry = byte.Parse(_conf.SelectSingleNode(combine(prefix, TRY_NUMBER_CONFIG_TAG)).InnerText);
            } catch (NullReferenceException) {
                (new ConfigLoadException("Ошибка структуры конфигурационного файла", "Ошибка чтения конфигурационного файла", confName)).visualise();
                //use hard-coded defaults
            }
            try {
                prefix = combine(ROOT_CONFIG_TAG, OVERVIEW_CONFIG_TAG);
                sPoint = ushort.Parse(_conf.SelectSingleNode(combine(prefix, START_SCAN_CONFIG_TAG)).InnerText);
                ePoint = ushort.Parse(_conf.SelectSingleNode(combine(prefix, END_SCAN_CONFIG_TAG)).InnerText);
            } catch (NullReferenceException) {
                (new ConfigLoadException("Ошибка структуры конфигурационного файла", "Ошибка чтения конфигурационного файла", confName)).visualise();
                //use hard-coded defaults
            }
            try {
                loadMassCoeffs();
            } catch (ConfigLoadException) {
                //cle.visualise();
                //use hard-coded defaults
            }
            try {
                loadCommonOptions();
            } catch (ConfigLoadException cle) {
                cle.visualise();
                //use hard-coded defaults
            }
            try {
                LoadPreciseEditorData();
            } catch (ConfigLoadException cle) {
                cle.visualise();
                //use empty default ped
            }
            prefix = combine(ROOT_CONFIG_TAG, CHECK_CONFIG_TAG);
            try {
                ushort step = ushort.Parse(_conf.SelectSingleNode(combine(prefix, PEAK_NUMBER_CONFIG_TAG)).InnerText);
                byte collector = byte.Parse(_conf.SelectSingleNode(combine(prefix, PEAK_COL_NUMBER_CONFIG_TAG)).InnerText);
                ushort width = ushort.Parse(_conf.SelectSingleNode(combine(prefix, PEAK_WIDTH_CONFIG_TAG)).InnerText);
                reperPeak = new Utility.PreciseEditorData(false, 255, step, collector, 0, width, 0, "checker peak");
            } catch (NullReferenceException) {
                //use hard-coded defaults (null checker peak)
            } catch (FormatException) {
                // TODO: very bad..
                //use hard-coded defaults (null checker peak)
            }
            try {
                iterations = int.Parse(_conf.SelectSingleNode(combine(prefix, CHECK_ITER_NUMBER_CONFIG_TAG)).InnerText);
            } catch (NullReferenceException) {
                //use hard-coded defaults (infinite iterations)
            }
            try {
                timeLimit = int.Parse(_conf.SelectSingleNode(combine(prefix, CHECK_TIME_LIMIT_CONFIG_TAG)).InnerText);
            } catch (NullReferenceException) {
                //use hard-coded defaults (no time limit)
            }
            try {
                allowedShift = ushort.Parse(_conf.SelectSingleNode(combine(prefix, CHECK_MAX_SHIFT_CONFIG_TAG)).InnerText);
            } catch (NullReferenceException) {
                //use hard-coded defaults (zero shift allowed)
            }
            // BAD: really uses previous values!
            */
        }

        private static void saveScanOptions() {
            string prefix = combine(ROOT_CONFIG_TAG, OVERVIEW_CONFIG_TAG);
            fillInnerText(prefix, START_SCAN_CONFIG_TAG, sPoint);
            fillInnerText(prefix, END_SCAN_CONFIG_TAG, ePoint);
            _conf.Save(@confName);
        }
        internal static void saveScanOptions(ushort sPointReal, ushort ePointReal) {
            Config.sPoint = sPointReal;//!!!
            Config.ePoint = ePointReal;//!!!
            Config.saveScanOptions();
        }

        private static void saveDelaysOptions() {
            string prefix = combine(ROOT_CONFIG_TAG, COMMON_CONFIG_TAG);
            fillInnerText(prefix, DELAY_BEFORE_MEASURE_CONFIG_TAG, commonOpts.befTime);
            fillInnerText(prefix, EQUAL_DELAYS_CONFIG_TAG, commonOpts.ForwardTimeEqualsBeforeTime);
            fillInnerText(prefix, DELAY_FORWARD_MEASURE_CONFIG_TAG, commonOpts.fTime);
            fillInnerText(prefix, DELAY_BACKWARD_MEASURE_CONFIG_TAG, commonOpts.bTime);
            _conf.Save(@confName);
        }
        internal static void saveDelaysOptions(bool forwardAsBefore, ushort befTimeReal, ushort fTimeReal, ushort bTimeReal) {
            Config.commonOpts.befTimeReal = befTimeReal;
            Config.commonOpts.fTimeReal = fTimeReal;
            Config.commonOpts.bTimeReal = bTimeReal;
            Config.commonOpts.ForwardTimeEqualsBeforeTime = forwardAsBefore;
            Config.saveDelaysOptions();
        }

        private static void saveMassCoeffs() {
            fillInnerText(ROOT_CONFIG_TAG, INTERFACE_CONFIG_TAG);
            string prefix = combine(ROOT_CONFIG_TAG, INTERFACE_CONFIG_TAG);
            fillInnerText(prefix, C1_CONFIG_TAG, col1Coeff.ToString("R", CultureInfo.InvariantCulture));
            fillInnerText(prefix, C2_CONFIG_TAG, col2Coeff.ToString("R", CultureInfo.InvariantCulture));
            _conf.Save(@confName);
        }

        private static void saveConnectOptions() {
            string prefix = combine(ROOT_CONFIG_TAG, CONNECT_CONFIG_TAG);
            fillInnerText(prefix, PORT_CONFIG_TAG, Port);
            fillInnerText(prefix, BAUDRATE_CONFIG_TAG, BaudRate);
            fillInnerText(prefix, TRY_NUMBER_CONFIG_TAG, sendTry);
            _conf.Save(@confName);
        }
        internal static void saveConnectOptions(string port, ushort baudrate) {
            Config.Port = port;
            Config.BaudRate = baudrate;
            Config.saveConnectOptions();
        }

        private static void saveCheckOptions() {
            //checkpeak & iterations
            string prefix = combine(ROOT_CONFIG_TAG, CHECK_CONFIG_TAG);
            if (_conf.SelectSingleNode(prefix) == null) {
                XmlNode checkRegion = _conf.CreateElement(CHECK_CONFIG_TAG);
                _conf.SelectSingleNode(ROOT_CONFIG_TAG).AppendChild(checkRegion);
            }
            if (reperPeak != null) {
                fillInnerText(prefix, PEAK_NUMBER_CONFIG_TAG, reperPeak.Step);
                fillInnerText(prefix, PEAK_COL_NUMBER_CONFIG_TAG, reperPeak.Collector);
                fillInnerText(prefix, PEAK_WIDTH_CONFIG_TAG, reperPeak.Width);
            } else {
                fillInnerText(prefix, PEAK_NUMBER_CONFIG_TAG);
                fillInnerText(prefix, PEAK_COL_NUMBER_CONFIG_TAG);
                fillInnerText(prefix, PEAK_WIDTH_CONFIG_TAG);
            }
            fillInnerText(prefix, CHECK_ITER_NUMBER_CONFIG_TAG, iterations);
            fillInnerText(prefix, CHECK_TIME_LIMIT_CONFIG_TAG, timeLimit);
            fillInnerText(prefix, CHECK_MAX_SHIFT_CONFIG_TAG, allowedShift);
            _conf.Save(@confName);
        }
        internal static void saveCheckOptions(int iter, int timeLim, ushort shift, Utility.PreciseEditorData peak) {
            iterations = iter;
            timeLimit = timeLim;
            allowedShift = shift;
            reperPeak = peak;
            saveCheckOptions();
        }

        internal static void saveAll() {
            saveConnectOptions();
            saveScanOptions();
            saveCommonOptions();
            saveDelaysOptions();
            saveMassCoeffs();
            saveCheckOptions();
            SavePreciseOptions();
        }

        internal static bool openSpectrumFile(string filename, bool hint, out Graph graph) {
            ISpectrumReader reader = TagHolder.getSpectrumReader(filename, hint);
            return reader.readSpectrum(out graph);
        }

        private static string genAutoSaveFilename(string extension, DateTime now) {
            string dirname;
            dirname = System.IO.Path.Combine(INITIAL_DIR, string.Format("{0}-{1}-{2}", now.Year, now.Month, now.Day));
            if (!System.IO.Directory.Exists(@dirname)) {
                System.IO.Directory.CreateDirectory(@dirname);
            }
            return System.IO.Path.Combine(dirname, string.Format("{0}-{1}-{2}-{3}.", now.Hour, now.Minute, now.Second, now.Millisecond) + extension);
        }

        internal static XmlDocument SaveSpecterFile(string p, Graph graph) {
            XmlDocument sf = new XmlDocument();
            XmlNode scanNode = createRootStub(sf, "").AppendChild(sf.CreateElement(OVERVIEW_CONFIG_TAG));
            XmlNode temp = sf.SelectSingleNode(combine(ROOT_CONFIG_TAG, HEADER_CONFIG_TAG));
            switch (graph.DisplayingMode) {
                case Graph.Displaying.Loaded:
                    temp.InnerText = MEASURED_SPECTRUM_HEADER;
                    break;
                case Graph.Displaying.Measured:
                    temp.InnerText = MEASURED_SPECTRUM_HEADER;
                    XmlElement elem = sf.CreateElement(START_SCAN_CONFIG_TAG);
                    elem.InnerText = sPoint.ToString();
                    scanNode.AppendChild(elem);
                    elem = sf.CreateElement(END_SCAN_CONFIG_TAG);
                    elem.InnerText = ePoint.ToString();
                    scanNode.AppendChild(elem);
                    // In case of loaded (not auto) start/end points and measure parameters are not connected to spectrum data..
                    break;
                case Graph.Displaying.Diff:
                    temp.InnerText = DIFF_SPECTRUM_HEADER;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            scanNode.AppendChild(sf.CreateElement(COL1_CONFIG_TAG));
            scanNode.AppendChild(sf.CreateElement(COL2_CONFIG_TAG));
            foreach (ZedGraph.PointPair pp in graph.Displayed1Steps[0]) {
                temp = sf.CreateElement(POINT_CONFIG_TAG);
                temp.AppendChild(sf.CreateElement(POINT_STEP_CONFIG_TAG)).InnerText = pp.X.ToString();
                temp.AppendChild(sf.CreateElement(POINT_COUNT_CONFIG_TAG)).InnerText = ((long)(pp.Y)).ToString();
                scanNode.SelectSingleNode(COL1_CONFIG_TAG).AppendChild(temp);
            }
            foreach (ZedGraph.PointPair pp in graph.Displayed2Steps[0]) {
                temp = sf.CreateElement(POINT_CONFIG_TAG);
                temp.AppendChild(sf.CreateElement(POINT_STEP_CONFIG_TAG)).InnerText = pp.X.ToString();
                temp.AppendChild(sf.CreateElement(POINT_COUNT_CONFIG_TAG)).InnerText = ((long)(pp.Y)).ToString();
                scanNode.SelectSingleNode(COL2_CONFIG_TAG).AppendChild(temp);
            }
            sf.Save(@p);
            return sf;
        }
        internal static void AutoSaveSpecterFile() {
            DateTime dt = System.DateTime.Now;
            string filename = genAutoSaveFilename(SPECTRUM_EXT, dt);
            XmlDocument file = SaveSpecterFile(filename, Graph.Instance);

            XmlAttribute attr = file.CreateAttribute(TIME_SPECTRUM_ATTRIBUTE);
            attr.Value = dt.ToString("G", DateTimeFormatInfo.InvariantInfo);
            file.SelectSingleNode(combine(ROOT_CONFIG_TAG, HEADER_CONFIG_TAG)).Attributes.Append(attr);

            XmlNode commonNode = createCommonOptsStub(file, file.SelectSingleNode(ROOT_CONFIG_TAG));
            saveCommonOptions(commonNode);
            file.Save(filename);
        }

        internal static void DistractSpectra(string what, ushort step, Graph.pListScaled plsReference, Utility.PreciseEditorData pedReference, Graph graph) {
            ISpectrumReader reader = TagHolder.getSpectrumReader(what, !graph.isPreciseSpectrum);
            reader.distractSpectrum(step, plsReference, pedReference, graph);
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

        internal static XmlDocument SavePreciseSpecterFile(string filename, Graph graph) {
            string header;
            switch (graph.DisplayingMode) {
                case Graph.Displaying.Loaded:
                    header = MEASURED_SPECTRUM_HEADER;
                    break;
                case Graph.Displaying.Measured:
                    header = MEASURED_SPECTRUM_HEADER;
                    break;
                case Graph.Displaying.Diff:
                    header = DIFF_SPECTRUM_HEADER;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return SavePreciseOptions(graph.PreciseData, filename, header, true, false);
        }
        internal static DateTime AutoSavePreciseSpecterFile(short shift) {
            DateTime dt = System.DateTime.Now;
            string filename = genAutoSaveFilename(PRECISE_SPECTRUM_EXT, dt);
            XmlDocument file = SavePreciseSpecterFile(filename, Graph.Instance);

            writeSpectrumOptions(file, dt, shift);
            file.Save(filename);

            return dt;
        }
        internal static void autoSaveMonitorSpectrumFile(short shift) {
            DateTime dt = AutoSavePreciseSpecterFile(shift);// now both files are saved
            string filename = genAutoSaveFilename(MONITOR_SPECTRUM_EXT, dt);
            XmlDocument file = SavePreciseOptions(Config.PreciseData, filename, MONITOR_SPECTRUM_HEADER, false, true);

            writeSpectrumOptions(file, dt, shift);
            file.Save(filename);
        }
        private static void writeSpectrumOptions(XmlDocument file, DateTime dt, short shift) {
            XmlAttributeCollection headerNodeAttributes = file.SelectSingleNode(combine(ROOT_CONFIG_TAG, HEADER_CONFIG_TAG)).Attributes;
            XmlAttribute attr = file.CreateAttribute(TIME_SPECTRUM_ATTRIBUTE);
            attr.Value = dt.ToString("G", DateTimeFormatInfo.InvariantInfo);
            headerNodeAttributes.Append(attr);

            attr = file.CreateAttribute(SHIFT_SPECTRUM_ATTRIBUTE);
            attr.Value = shift.ToString();
            headerNodeAttributes.Append(attr);

            XmlNode commonNode = createCommonOptsStub(file, file.SelectSingleNode(ROOT_CONFIG_TAG));
            saveCommonOptions(commonNode);
        }
        
        private static void SavePreciseOptions() {
            SavePreciseOptions(Config.PreciseData, confName, PRECISE_OPTIONS_HEADER, false, false);
        }
        internal static void SavePreciseOptions(PreciseSpectrum peds) {
            preciseData = peds;
            SavePreciseOptions(peds, confName, PRECISE_OPTIONS_HEADER, false, false);
        }
        internal static XmlDocument SavePreciseOptions(List<Utility.PreciseEditorData> peds, string pedConfName, string header, bool savePoints, bool savePeakSum) {
            XmlDocument pedConf;

            if (newConfigFile(out pedConf, pedConfName)) {
                XmlNode rootNode = createRootStub(pedConf, header);
                createPEDStub(pedConf, rootNode);
            } else {
                for (int i = 1; i <= 20; ++i) {
                    string prefix = combine(ROOT_CONFIG_TAG, SENSE_CONFIG_TAG, string.Format(PEAK_TAGS_FORMAT, i));
                    fillInnerText(prefix, PEAK_NUMBER_CONFIG_TAG);
                    fillInnerText(prefix, PEAK_ITER_NUMBER_CONFIG_TAG);
                    fillInnerText(prefix, PEAK_WIDTH_CONFIG_TAG);
                    fillInnerText(prefix, PEAK_PRECISION_CONFIG_TAG);
                    fillInnerText(prefix, PEAK_COL_NUMBER_CONFIG_TAG);
                    fillInnerText(prefix, PEAK_COMMENT_CONFIG_TAG);
                    fillInnerText(prefix, PEAK_USE_CONFIG_TAG);
                }
            }

            foreach (Utility.PreciseEditorData ped in peds) {
                XmlNode regionNode = pedConf.SelectSingleNode(combine(ROOT_CONFIG_TAG, SENSE_CONFIG_TAG, string.Format(PEAK_TAGS_FORMAT, ped.pNumber + 1)));
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
                    regionNode.AppendChild(pedConf.CreateElement(PEAK_COUNT_SUM_CONFIG_TAG)).InnerText = ped.AssociatedPoints.PLSreference.PeakSum.ToString();
                }
                if (savePoints) {
                    XmlNode temp;
                    foreach (ZedGraph.PointPair pp in ped.AssociatedPoints) {
                        temp = pedConf.CreateElement(POINT_CONFIG_TAG);
                        temp.AppendChild(pedConf.CreateElement(POINT_STEP_CONFIG_TAG)).InnerText = pp.X.ToString();
                        temp.AppendChild(pedConf.CreateElement(POINT_COUNT_CONFIG_TAG)).InnerText = ((long)(pp.Y)).ToString();
                        regionNode.AppendChild(temp);
                    }
                }
            }
            pedConf.Save(@pedConfName);
            return pedConf;
        }

        internal static List<Utility.PreciseEditorData> LoadPreciseEditorData(string pedConfName) {
            List<Utility.PreciseEditorData> peds = new List<Utility.PreciseEditorData>();
            XmlDocument pedConf;
            string mainConfPrefix = "";

            if (!pedConfName.Equals(confName)) {
                pedConf = new XmlDocument();
                try {
                    pedConf.Load(pedConfName);
                } catch (Exception Error) {
                    throw new ConfigLoadException(Error.Message, "Ошибка чтения файла прецизионных точек", pedConfName);
                }
                if (pedConf.SelectSingleNode(combine(ROOT_CONFIG_TAG, SENSE_CONFIG_TAG)) != null)
                    mainConfPrefix = ROOT_CONFIG_TAG;
                else if (pedConf.SelectSingleNode(SENSE_CONFIG_TAG) == null) {
                    throw new structureErrorOnLoadPrecise(pedConfName);
                }
            } else {
                pedConf = _conf;
                mainConfPrefix = ROOT_CONFIG_TAG;
            }

            if (LoadPED(pedConf, pedConfName, peds, false, mainConfPrefix))
                return peds;
            return null;
        }
        private static void LoadPreciseEditorData() {
            List<Utility.PreciseEditorData> pedl = LoadPreciseEditorData(confName);
            if ((pedl != null) && (pedl.Count > 0)) {
                //BAD!!! cleaning previous points!!!
                preciseData.Clear();
                preciseData.AddRange(pedl);
            }
        }

        private static bool LoadPED(XmlDocument pedConf, string pedConfName, List<Utility.PreciseEditorData> peds, bool readSpectrum, string mainConfPrefix) {
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
                            if (readSpectrum)
                                throw new ConfigLoadException("Неверный формат данных", "Ошибка чтения файла прецизионного спектра", pedConfName);
                            else
                                throw new wrongFormatOnLoadPrecise(pedConfName);
                        }
                        if (readSpectrum) {
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
                                throw new ConfigLoadException("Неверный формат данных", "Ошибка чтения файла прецизионного спектра", pedConfName);
                            }
                            temp.AssociatedPoints = tempPntLst;
                        }
                    }
                } catch (NullReferenceException) {
                    if (readSpectrum)
                        throw new ConfigLoadException("Ошибка структуры файла", "Ошибка чтения файла прецизионного спектра", pedConfName);
                    else
                        throw new structureErrorOnLoadPrecise(pedConfName);
                }
                if (temp != null) peds.Add(temp);
            }
            peds.Sort(Utility.ComparePreciseEditorData);
            return true;
        }

        private static void saveCommonOptions() {
            saveCommonOptions(confName);
        }
        internal static void saveCommonOptions(ushort eT, ushort iT, double iV, double cp, double eC, double hC, double fv1, double fv2) {
            saveCommonOptions(confName, eT, iT, iV, cp, eC, hC, fv1, fv2);
        }
        internal static void saveCommonOptions(string filename, ushort eT, ushort iT, double iV, double cp, double eC, double hC, double fv1, double fv2) {
            Config.commonOpts.eTimeReal = eT;
            Config.commonOpts.iTimeReal = iT;
            Config.commonOpts.iVoltageReal = iV;
            Config.commonOpts.CPReal = cp;
            Config.commonOpts.eCurrentReal = eC;
            Config.commonOpts.hCurrentReal = hC;
            Config.commonOpts.fV1Real = fv1;
            Config.commonOpts.fV2Real = fv2;

            saveCommonOptions(filename);
        }
        private static void saveCommonOptions(string filename) {
            XmlDocument cdConf;
            XmlNode commonNode;

            if (newConfigFile(out cdConf, filename)) {
                XmlNode rootNode = createRootStub(cdConf, COMMON_OPTIONS_HEADER);
                commonNode = createCommonOptsStub(cdConf, rootNode);
            } else {
                commonNode = cdConf.SelectSingleNode(combine(ROOT_CONFIG_TAG, COMMON_CONFIG_TAG));
            }
            saveCommonOptions(commonNode);
            cdConf.Save(filename);
        }
        private static void saveCommonOptions(XmlNode commonNode) {
            commonNode.SelectSingleNode(EXPOSITURE_TIME_CONFIG_TAG).InnerText = Config.commonOpts.eTime.ToString();
            commonNode.SelectSingleNode(TRANSITION_TIME_CONFIG_TAG).InnerText = Config.commonOpts.iTime.ToString();
            commonNode.SelectSingleNode(IONIZATION_VOLTAGE_CONFIG_TAG).InnerText = Config.commonOpts.iVoltage.ToString();
            commonNode.SelectSingleNode(CAPACITOR_VOLTAGE_COEFF_CONFIG_TAG).InnerText = Config.commonOpts.CP.ToString();
            commonNode.SelectSingleNode(EMISSION_CURRENT_CONFIG_TAG).InnerText = Config.commonOpts.eCurrent.ToString();
            commonNode.SelectSingleNode(HEAT_CURRENT_CONFIG_TAG).InnerText = Config.commonOpts.hCurrent.ToString();
            commonNode.SelectSingleNode(FOCUS_VOLTAGE1_CONFIG_TAG).InnerText = Config.commonOpts.fV1.ToString();
            commonNode.SelectSingleNode(FOCUS_VOLTAGE2_CONFIG_TAG).InnerText = Config.commonOpts.fV2.ToString();

            commonNode.SelectSingleNode(DELAY_BEFORE_MEASURE_CONFIG_TAG).InnerText = Config.commonOpts.befTime.ToString();
            commonNode.SelectSingleNode(EQUAL_DELAYS_CONFIG_TAG).InnerText = Config.commonOpts.ForwardTimeEqualsBeforeTime.ToString();
            commonNode.SelectSingleNode(DELAY_FORWARD_MEASURE_CONFIG_TAG).InnerText = Config.commonOpts.fTime.ToString();
            commonNode.SelectSingleNode(DELAY_BACKWARD_MEASURE_CONFIG_TAG).InnerText = Config.commonOpts.bTime.ToString();
        }

        private static void newCommonOptionsFileOnLoad(out XmlDocument conf, string filename) {
            if (newConfigFile(out conf, filename)) {
                try {
                    conf.Load(filename);
                } catch (Exception Error) {
                    throw new ConfigLoadException(Error.Message, "Ошибка чтения файла общих настроек", filename);
                }
            }
        }
        private static void loadCommonOptions() {
            loadCommonOptions(confName, Config.commonOpts);
        }
        internal static void loadCommonOptions(string cdConfName) {
            loadCommonOptions(cdConfName, Config.commonOpts);
        }
        private static void loadCommonOptions(string cdConfName, CommonOptions commonOpts) {
            XmlDocument cdConf;
            newCommonOptionsFileOnLoad(out cdConf, cdConfName);

            loadCommonOptions(cdConfName, cdConf, commonOpts);
        }
        private static void loadCommonOptions(string cdConfName, XmlDocument cdConf, CommonOptions commonOpts) {
            string mainConfPrefix = "";

            if (cdConf.SelectSingleNode(combine(ROOT_CONFIG_TAG, COMMON_CONFIG_TAG)) != null)
                mainConfPrefix = ROOT_CONFIG_TAG;
            else if (cdConf.SelectSingleNode(COMMON_CONFIG_TAG) == null) {
                throw new structureErrorOnLoadCommonData(cdConfName);
            }
            XmlNode commonNode = cdConf.SelectSingleNode(combine(mainConfPrefix, COMMON_CONFIG_TAG));

            loadCommonOptions(cdConfName, commonNode, commonOpts);
        }
        private static void loadCommonOptions(string cdConfName, XmlNode commonNode, CommonOptions commonOpts) {
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

                //commonOpts = new CommonOptions();
                commonOpts.eTime = eT;
                commonOpts.iTime = iT;
                commonOpts.iVoltage = iV;
                commonOpts.CP = CP;
                commonOpts.eCurrent = eC;
                commonOpts.hCurrent = hC;
                commonOpts.fV1 = fV1;
                commonOpts.fV2 = fV2;
            } catch (NullReferenceException) {
                //commonOpts = null;
                throw new structureErrorOnLoadCommonData(cdConfName);
            }

            try {
                ushort befT, fT, bT;
                bool fAsbef;

                befT = ushort.Parse(commonNode.SelectSingleNode(DELAY_BEFORE_MEASURE_CONFIG_TAG).InnerText);
                fT = ushort.Parse(commonNode.SelectSingleNode(DELAY_FORWARD_MEASURE_CONFIG_TAG).InnerText);
                bT = ushort.Parse(commonNode.SelectSingleNode(DELAY_BACKWARD_MEASURE_CONFIG_TAG).InnerText);
                fAsbef = bool.Parse(commonNode.SelectSingleNode(EQUAL_DELAYS_CONFIG_TAG).InnerText);

                commonOpts.befTime = befT;
                commonOpts.ForwardTimeEqualsBeforeTime = fAsbef;
                commonOpts.fTime = fT;
                commonOpts.bTime = bT;
            } catch (NullReferenceException) {
                //Use hard-coded defaults
                return;
            }
        }
        #region Error messages on loading different configs
        internal class ConfigLoadException: System.Exception {
            internal ConfigLoadException(string message, string filestring, string confname)
                : base(message) {
                this.Data["FS"] = filestring;
                this.Data["CN"] = confname;
            }
            internal void visualise() {
                if (!(this.Data["CN"].Equals(confName)))
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
        private static void loadMassCoeffs() {
            loadMassCoeffs(confName);
        }
        private static void loadMassCoeffs(string confName) {
            XmlDocument conf;

            newCommonOptionsFileOnLoad(out conf, confName);
            XmlNode interfaceNode = conf.SelectSingleNode(combine(ROOT_CONFIG_TAG, INTERFACE_CONFIG_TAG));

            try {
                col1Coeff = double.Parse(interfaceNode.SelectSingleNode(C1_CONFIG_TAG).InnerText, CultureInfo.InvariantCulture);
                col2Coeff = double.Parse(interfaceNode.SelectSingleNode(C2_CONFIG_TAG).InnerText, CultureInfo.InvariantCulture);
            } catch (NullReferenceException) {
                //!!!
                throw new ConfigLoadException("", "", confName);
            }
        }
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
            saveMassCoeffs();
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
        private static bool newConfigFile(out XmlDocument conf, string filename) {
            if (!filename.Equals(confName)) {
                conf = new XmlDocument();
                return true;
            }
            conf = _conf;
            return false;
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
            commonNode.AppendChild(conf.CreateElement(HEADER_CONFIG_TAG));
            commonNode.AppendChild(conf.CreateElement(EXPOSITURE_TIME_CONFIG_TAG));
            commonNode.AppendChild(conf.CreateElement(TRANSITION_TIME_CONFIG_TAG));
            commonNode.AppendChild(conf.CreateElement(IONIZATION_VOLTAGE_CONFIG_TAG));
            commonNode.AppendChild(conf.CreateElement(CAPACITOR_VOLTAGE_COEFF_CONFIG_TAG));
            commonNode.AppendChild(conf.CreateElement(EMISSION_CURRENT_CONFIG_TAG));
            commonNode.AppendChild(conf.CreateElement(HEAT_CURRENT_CONFIG_TAG));
            commonNode.AppendChild(conf.CreateElement(FOCUS_VOLTAGE1_CONFIG_TAG));
            commonNode.AppendChild(conf.CreateElement(FOCUS_VOLTAGE2_CONFIG_TAG));

            commonNode.AppendChild(conf.CreateElement(DELAY_BEFORE_MEASURE_CONFIG_TAG));
            commonNode.AppendChild(conf.CreateElement(EQUAL_DELAYS_CONFIG_TAG));
            commonNode.AppendChild(conf.CreateElement(DELAY_FORWARD_MEASURE_CONFIG_TAG));
            commonNode.AppendChild(conf.CreateElement(DELAY_BACKWARD_MEASURE_CONFIG_TAG));
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

        private interface ISpectrumReader {
            bool readSpectrum(out Graph graph);
            void distractSpectrum(ushort step, Graph.pListScaled plsReference, Utility.PreciseEditorData pedReference, Graph graph);
        }
        private interface ISpectrumWriter {
            void writeSpectrum();
        }
        private interface IMainConfig {
            void readConfig();
            void writeConfig();
            XmlDocument Config {
                get;
            }
        }
        private abstract class TagHolder {
        protected const string TEST_TAG = "test";

            private readonly string filename;
            private readonly XmlDocument sf;
            public TagHolder(string filename, XmlDocument doc) {
                this.filename = filename;
                this.sf = doc;
            }
            private class LegacyMainConfig: TagHolder, IMainConfig {
                public LegacyMainConfig(string filename, XmlDocument doc): base(filename, doc) {
                }
                #region IMainConfig implementation
                public void readConfig ()
                {
                    string prefix;
                    try {
                        prefix = combine(ROOT_CONFIG_TAG, CONNECT_CONFIG_TAG);
                        SerialPort = (sf.SelectSingleNode(combine(prefix, PORT_CONFIG_TAG)).InnerText);
                        SerialBaudRate = ushort.Parse(sf.SelectSingleNode(combine(prefix, BAUDRATE_CONFIG_TAG)).InnerText);
                        sendTry = byte.Parse(sf.SelectSingleNode(combine(prefix, TRY_NUMBER_CONFIG_TAG)).InnerText);
                    } catch (NullReferenceException) {
                        (new ConfigLoadException("Ошибка структуры конфигурационного файла", "Ошибка чтения конфигурационного файла", confName)).visualise();
                        //use hard-coded defaults
                    }
                    try {
                        prefix = combine(ROOT_CONFIG_TAG, OVERVIEW_CONFIG_TAG);
                        sPoint = ushort.Parse(sf.SelectSingleNode(combine(prefix, START_SCAN_CONFIG_TAG)).InnerText);
                        ePoint = ushort.Parse(sf.SelectSingleNode(combine(prefix, END_SCAN_CONFIG_TAG)).InnerText);
                    } catch (NullReferenceException) {
                        (new ConfigLoadException("Ошибка структуры конфигурационного файла", "Ошибка чтения конфигурационного файла", confName)).visualise();
                        //use hard-coded defaults
                    }
                    try {
                        loadMassCoeffs();
                    } catch (ConfigLoadException) {
                        //cle.visualise();
                        //use hard-coded defaults
                    }
                    try {
                        loadCommonOptions();
                    } catch (ConfigLoadException cle) {
                        cle.visualise();
                        //use hard-coded defaults
                    }
                    try {
                        LoadPreciseEditorData();
                    } catch (ConfigLoadException cle) {
                        cle.visualise();
                        //use empty default ped
                    }
                    prefix = combine(ROOT_CONFIG_TAG, CHECK_CONFIG_TAG);
                    try {
                        ushort step = ushort.Parse(sf.SelectSingleNode(combine(prefix, PEAK_NUMBER_CONFIG_TAG)).InnerText);
                        byte collector = byte.Parse(sf.SelectSingleNode(combine(prefix, PEAK_COL_NUMBER_CONFIG_TAG)).InnerText);
                        ushort width = ushort.Parse(sf.SelectSingleNode(combine(prefix, PEAK_WIDTH_CONFIG_TAG)).InnerText);
                        reperPeak = new Utility.PreciseEditorData(false, 255, step, collector, 0, width, 0, "checker peak");
                    } catch (NullReferenceException) {
                        //use hard-coded defaults (null checker peak)
                    } catch (FormatException) {
                        // TODO: very bad..
                        //use hard-coded defaults (null checker peak)
                    }
                    try {
                        iterations = int.Parse(sf.SelectSingleNode(combine(prefix, CHECK_ITER_NUMBER_CONFIG_TAG)).InnerText);
                    } catch (NullReferenceException) {
                        //use hard-coded defaults (infinite iterations)
                    }
                    try {
                        timeLimit = int.Parse(sf.SelectSingleNode(combine(prefix, CHECK_TIME_LIMIT_CONFIG_TAG)).InnerText);
                    } catch (NullReferenceException) {
                        //use hard-coded defaults (no time limit)
                    }
                    try {
                        allowedShift = ushort.Parse(sf.SelectSingleNode(combine(prefix, CHECK_MAX_SHIFT_CONFIG_TAG)).InnerText);
                    } catch (NullReferenceException) {
                        //use hard-coded defaults (zero shift allowed)
                    }
                    // BAD: really uses previous values!
                }
                public void writeConfig ()
                {
                    sf.Save(@filename);
                }
                public XmlDocument Config {
                    get {
                        return sf;
                    }
                }
                #endregion
            }
            private class LegacySpectrumReader: TagHolder, ISpectrumReader {
                protected new const string TEST_TAG = "test2";
                private readonly bool hint;
                public LegacySpectrumReader(string filename, bool hint, XmlDocument doc): base(filename, doc) {
                    // TODO: continue loading here, modify hint?
                    this.hint = hint;
                }

                #region ISpectrumReader Members
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
                            return (!hint);
                    } catch (ConfigLoadException cle) {
                        resultException = (resultException == null) ? cle : resultException;
                    }
                    throw resultException;
                }
                public void distractSpectrum(ushort step, Graph.pListScaled plsReference, Utility.PreciseEditorData pedReference, Graph graph) {
                    // TODO: hint != graph.isPreciseSpectrum
                    if (graph.isPreciseSpectrum) {
                        PreciseSpectrum peds = new PreciseSpectrum();
                        if (OpenPreciseSpecterFile(peds)) {
                            List<Utility.PreciseEditorData> temp;
                            switch (graph.DisplayingMode) {
                                case Graph.Displaying.Loaded:
                                    temp = new List<Utility.PreciseEditorData>(graph.PreciseData);
                                    break;
                                case Graph.Displaying.Measured:
                                    temp = new List<Utility.PreciseEditorData>(preciseData);
                                    break;
                                case Graph.Displaying.Diff:
                                //diffs can't be distracted!
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            temp.Sort(Utility.ComparePreciseEditorData);
                            try {
                                temp = PreciseEditorDataListDiff(temp, peds, step, pedReference);
                                graph.updateGraphAfterPreciseDiff(temp);
                            } catch (System.ArgumentException) {
                                throw new ConfigLoadException("Несовпадение рядов данных", "Ошибка при вычитании спектров", filename);
                            }
                        }
                    } else {
                        PointPairListPlus pl12 = new PointPairListPlus();
                        PointPairListPlus pl22 = new PointPairListPlus();
                        CommonOptions commonOpts;
                        if (OpenSpecterFile(pl12, pl22, out commonOpts) == Graph.Displaying.Measured) {
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
                                throw new ConfigLoadException("Несовпадение рядов данных", "Ошибка при вычитании спектров", filename);
                            }
                        } else {
                            //diffs can't be distracted!
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                }
                #endregion
                private bool OpenSpecterFile(out Graph graph) {
                    PointPairListPlus pl1 = new PointPairListPlus(), pl2 = new PointPairListPlus();
                    CommonOptions commonOpts;
                    Graph.Displaying result = OpenSpecterFile(pl1, pl2, out commonOpts);

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
                private Graph.Displaying OpenSpecterFile(PointPairListPlus pl1, PointPairListPlus pl2, out CommonOptions commonOpts) {
                    XmlNode headerNode = null;
                    string prefix = "";

                    if (sf.SelectSingleNode(OVERVIEW_CONFIG_TAG) != null) {
                        headerNode = sf.SelectSingleNode(combine(OVERVIEW_CONFIG_TAG, HEADER_CONFIG_TAG));
                    } else if (sf.SelectSingleNode(combine(ROOT_CONFIG_TAG, OVERVIEW_CONFIG_TAG)) != null) {
                        prefix = ROOT_CONFIG_TAG;
                        headerNode = sf.SelectSingleNode(combine(ROOT_CONFIG_TAG, HEADER_CONFIG_TAG));
                    } else {
                        throw new ConfigLoadException("Ошибка структуры файла", "Ошибка чтения файла спектра", filename);
                    }

                    Graph.Displaying spectrumType = Graph.Displaying.Measured;
                    if (headerNode != null && headerNode.InnerText == DIFF_SPECTRUM_HEADER)
                        spectrumType = Graph.Displaying.Diff;

                    ushort X = 0;
                    long Y = 0;
                    try {
                        foreach (XmlNode pntNode in sf.SelectNodes(combine(prefix, OVERVIEW_CONFIG_TAG, COL1_CONFIG_TAG, POINT_CONFIG_TAG))) {
                            X = ushort.Parse(pntNode.SelectSingleNode(POINT_STEP_CONFIG_TAG).InnerText);
                            Y = long.Parse(pntNode.SelectSingleNode(POINT_COUNT_CONFIG_TAG).InnerText);
                            pl1.Add(X, Y);
                        }
                        foreach (XmlNode pntNode in sf.SelectNodes(combine(prefix, OVERVIEW_CONFIG_TAG, COL2_CONFIG_TAG, POINT_CONFIG_TAG))) {
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
                        // what version of function will be called here?
                        commonOpts = new CommonOptions();
                        loadCommonOptions(filename, sf, commonOpts);
                    } catch (structureErrorOnLoadCommonData) {
                        commonOpts = null;
                    }
                    //!!!!!!!!!!!!!!!!!!!!!!!!
                    pl1.Sort(ZedGraph.SortType.XValues);
                    pl2.Sort(ZedGraph.SortType.XValues);
                    return spectrumType;
                }
                private bool OpenPreciseSpecterFile(out Graph graph) {
                    PreciseSpectrum peds = new PreciseSpectrum();
                    bool result = OpenPreciseSpecterFile(peds);
                    if (result) {
                        graph = new Graph(peds.CommonOptions);
                        graph.updateGraphAfterPreciseLoad(peds);
                    } else {
                        //TODO: other solution!
                        graph = null;
                    }
                    return result;
                }
                private bool OpenPreciseSpecterFile(PreciseSpectrum peds) {
                    string prefix = "";
                    if (sf.SelectSingleNode(combine(ROOT_CONFIG_TAG, SENSE_CONFIG_TAG)) != null)
                        prefix = ROOT_CONFIG_TAG;
                    else if (sf.SelectSingleNode(SENSE_CONFIG_TAG) == null) {
                        throw new ConfigLoadException("Ошибка структуры файла", "Ошибка чтения файла прецизионного спектра", filename);
                    }

                    CommonOptions co = new CommonOptions();
                    try {
                        // what version of function will be called here?
                        loadCommonOptions(filename, sf, co);
                    } catch (structureErrorOnLoadCommonData) {
                        co = null;
                    }
                    peds.CommonOptions = co;

                    return LoadPED(sf, filename, peds, true, prefix);
                }
            }
            private class AlwaysCurrentSpectrumWriter: TagHolder, ISpectrumWriter {
                public AlwaysCurrentSpectrumWriter(string filename, XmlDocument doc): base(filename, doc) {
                }
                public void writeSpectrum() {
                    // TODO
                }
            }
            public static ISpectrumReader getSpectrumReader(string filename, bool hint) {
                XmlDocument sf = new XmlDocument();
                try {
                    sf.Load(filename);
                } catch (Exception Error) {
                    throw new ConfigLoadException(Error.Message, "Ошибка чтения файла спектра", filename);
                }
                XmlNode rootNode = sf.SelectSingleNode(ROOT_CONFIG_TAG);
                if (rootNode == null) {
                    return new LegacySpectrumReader(filename, hint, sf);
                }
                XmlNode version = rootNode.Attributes.GetNamedItem(VERSION_ATTRIBUTE);
                if (version == null) {
                    return new LegacySpectrumReader(filename, hint, sf);
                }
                string versionText = version.Value;
                switch (versionText) {
                    case "1.0":
                        //CONFIG_VERSION
                        // TODO: return version-specific reader here!
                        return new LegacySpectrumReader(filename, hint, sf);
                    default:
                        // try load anyway
                        return new LegacySpectrumReader(filename, hint, sf);
                }
            }
            public static ISpectrumWriter getSpectrumWriter(string filename) {
                // TODO: form document here
                return new AlwaysCurrentSpectrumWriter(filename, new XmlDocument());
            }
            public static IMainConfig getMainConfig(string confName) {
                XmlDocument sf = new XmlDocument();
                try {
                    sf.Load(confName);
                } catch (Exception Error) {
                    throw new ConfigLoadException(Error.Message, "Ошибка чтения конфигурационного файла", confName);
                }
                XmlNode rootNode = sf.SelectSingleNode(ROOT_CONFIG_TAG);
                if (rootNode == null) {
                    return new LegacyMainConfig(confName, sf);
                }
                XmlNode version = rootNode.Attributes.GetNamedItem(VERSION_ATTRIBUTE);
                if (version == null) {
                    return new LegacyMainConfig(confName, sf);
                }
                string versionText = version.Value;
                switch (versionText) {
                    case "1.0":
                        //CONFIG_VERSION
                        // TODO: return version-specific reader here!
                        return new LegacyMainConfig(confName, sf);
                    default:
                        // try load anyway
                        return new LegacyMainConfig(confName, sf);
                }
            }
            /*private static TYPE findCorrespondingVersion<TYPE>(string filename, string errorMessage, params object[] args) where TYPE: TagHolder, new() {
                // TODO: implement this unification
                XmlDocument sf = new XmlDocument();
                try {
                    sf.Load(filename);
                } catch (Exception Error) {
                    throw new ConfigLoadException(Error.Message, errorMessage, filename);
                }
                XmlNode rootNode = sf.SelectSingleNode(ROOT_CONFIG_TAG);
                if (rootNode == null) {
                    return new TYPE();
                }
                XmlNode version = rootNode.Attributes.GetNamedItem(VERSION_ATTRIBUTE);
                if (version == null) {
                    return new TYPE(args);
                }
                string versionText = version.Value;
                switch (versionText) {
                    case "1.0":
                        //CONFIG_VERSION
                        // TODO: return version-specific reader here!
                        return new TYPE(args);
                    default:
                        // try load anyway
                        return new TYPE(args);
                }
            }*/
        }
    }
}