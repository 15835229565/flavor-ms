using System;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Flavor.Common.Data.Measure;

namespace Flavor.Common.Settings {
    static class Config {
        static IMainConfig mainConfig;
        static IMainConfigWriter mainConfigWriter;
        
        static readonly string INITIAL_DIR = System.IO.Directory.GetCurrentDirectory();

        const string CONFIG_NAME = "config.xml";
        const string CRASH_LOG_NAME = "MScrash.log";
        const string LIBRARY_NAME = "library.xml";

        static string mainConfigName;
        static string logName;
        static string libraryName;

        #region File extensions
        public const string SPECTRUM_EXT = "sdf";
        public const string PRECISE_SPECTRUM_EXT = "psf";
        public const string MONITOR_SPECTRUM_EXT = "mon";
        #endregion
        public const string DIFF_FILE_SUFFIX = "~diff";
        #region Dialog filters
        public static readonly string SPECTRUM_FILE_DIALOG_FILTER = string.Format("Specter data files (*.{0})|*.{0}", SPECTRUM_EXT);
        public static readonly string PRECISE_SPECTRUM_FILE_DIALOG_FILTER = string.Format("Precise specter files (*.{0})|*.{0}", PRECISE_SPECTRUM_EXT);
        #endregion

        public const ushort MIN_STEP = 0;
        public const ushort MAX_STEP = 2112;
        //public const ushort MAX_STEP = 4095;

        //public static readonly double[] COLLECTOR_COEFFS = { 2770 * 28, 896.5 * 18 };
        public static readonly double[] COLLECTOR_COEFFS = { 2770 * 28, 896.5 * 18, 1 };
        
        public const int PEAK_NUMBER = 20;

        public static CommonOptions CommonOptions { get; private set; }

        static PreciseSpectrum preciseData = new PreciseSpectrum();
        public static PreciseSpectrum PreciseData {
            get { return preciseData; }
        }

        static int reperPeakIndex = -1;
        public static int CheckerPeakIndex {
            get { return reperPeakIndex + 1; }
            set { reperPeakIndex = value - 1; }
        }

        static PreciseEditorData reperPeak = null;
        public static PreciseEditorData CustomCheckerPeak {
            get {
                return reperPeak == null ? null :
                    new PreciseEditorData(false, 255, reperPeak.Step, reperPeak.Collector, countMaxIteration(), reperPeak.Width, 0, "checker peak");
            }
        }
        public static PreciseEditorData CheckerPeak {
            get {
                if (reperPeakIndex == -1)
                    return CustomCheckerPeak;
                int index = preciseData.FindIndex(peak => peak.pNumber == reperPeakIndex);
                if (index == -1)
                    return null;
                return PreciseEditorData.GetCheckerPeak(preciseData[index], countMaxIteration());
            }
        }
        static ushort countMaxIteration() {
            return countMaxIteration(preciseData.getUsed());
        }
        static ushort countMaxIteration(List<PreciseEditorData> pedl) {
            ushort maxIteration = 0;
            foreach (var ped in pedl) {
                maxIteration = maxIteration < ped.Iterations ? ped.Iterations : maxIteration;
            }
            return maxIteration;
        }

        public static List<PreciseEditorData> PreciseDataWithChecker {
            get {
                var res = preciseData.getUsed();
                if (res.Count == 0) {
                    return null;
                }
                if (reperPeakIndex != -1) {
                    // TODO: move this costly operation to combined Predicate several lines before
                    // how to mark it?
                    int index = res.FindIndex(peak => peak.pNumber == reperPeakIndex);
                    if (index == -1) {
                        // no peak
                        res.Add(CheckerPeak);
                    } else {
                        // peak is also measured. error can be caused by this line (copying)
                        res[index] = PreciseEditorData.GetCheckerPeak(res[index], countMaxIteration(res));
                    }
                    return res;
                }
                if (reperPeak != null) {
                    res.Add(new PreciseEditorData(false, 255, reperPeak.Step, reperPeak.Collector, countMaxIteration(res), reperPeak.Width, 0, "checker peak"));
                }
                return res;
            }
        }
        public static int Iterations { get; private set; }
        public static int TimeLimit { get; private set; }
        public static ushort AllowedShift { get; private set; }

        public static string Port { get; private set; }
        public static int BaudRate { get; private set; }
        public static byte Try { get; private set; }

        public static ushort sPoint { get; private set; }
        public static ushort ePoint { get; private set; }
        public static byte BackgroundCycles { get; private set; }

        public static void getInitialDirectory() {
            mainConfigName = Path.Combine(INITIAL_DIR, CONFIG_NAME);
            logName = Path.Combine(INITIAL_DIR, CRASH_LOG_NAME);
            libraryName = Path.Combine(INITIAL_DIR, LIBRARY_NAME);
        }
        #region Global Config I/O
        public static void loadGlobalConfig() {
            mainConfig = TagHolder.getMainConfig(mainConfigName);
            mainConfig.read();
            mainConfigWriter = TagHolder.getMainConfigWriter(mainConfigName, mainConfig.XML);
        }
        public static void saveGlobalScanOptions(ushort sPointReal, ushort ePointReal) {
            sPoint = sPointReal;//!!!
            ePoint = ePointReal;//!!!
            mainConfigWriter.write();
        }
        public static void saveGlobalDelaysOptions(bool forwardAsBefore, ushort befTimeReal, ushort fTimeReal, ushort bTimeReal) {
            CommonOptions.befTimeReal = befTimeReal;
            CommonOptions.fTimeReal = fTimeReal;
            CommonOptions.bTimeReal = bTimeReal;
            CommonOptions.ForwardTimeEqualsBeforeTime = forwardAsBefore;
            mainConfigWriter.write();
        }
        public static void saveGlobalConnectOptions(string port, int baudrate) {
            Port = port;
            BaudRate = baudrate;
            mainConfigWriter.write();
        }
        public static void saveGlobalCheckOptions(int iter, int timeLim, ushort shift, PreciseEditorData peak, int index, byte backgroundCount) {
            Iterations = iter;
            TimeLimit = timeLim;
            AllowedShift = shift;
            reperPeak = peak;
            CheckerPeakIndex = index;
            BackgroundCycles = backgroundCount;
            mainConfigWriter.write();
        }
        public static void saveGlobalPreciseOptions(PreciseSpectrum peds) {
            // be very careful: variable reference is changed!
            preciseData = peds;
            mainConfigWriter.savePreciseData(peds, false);
            mainConfigWriter.write();
        }
        public static void temp_saveGO(double d1v, double d2v, double d3v, double iV, double eC, double fv1, double fv2, double c, double k, double expT) {
            CommonOptions.iVoltageReal = iV;
            CommonOptions.eCurrentReal = eC;
            CommonOptions.fV1Real = fv1;
            CommonOptions.fV2Real = fv2;
            CommonOptions.d1VReal = d1v;
            CommonOptions.d2VReal = d2v;
            CommonOptions.d3VReal = d3v;
            CommonOptions.C = c;
            CommonOptions.K = k;
            CommonOptions.eTimeReal = (ushort)expT;
            mainConfigWriter.saveCommonOptions(CommonOptions);
            mainConfigWriter.write();
        }
        //public static void saveGlobalCommonOptions(ushort eT, ushort iT, double iV, double cp, double eC, double hC, double fv1, double fv2) {
        //    commonOpts.eTimeReal = eT;
        //    commonOpts.iTimeReal = iT;
        //    commonOpts.iVoltageReal = iV;
        //    commonOpts.CPReal = cp;
        //    commonOpts.eCurrentReal = eC;
        //    commonOpts.hCurrentReal = hC;
        //    commonOpts.fV1Real = fv1;
        //    commonOpts.fV2Real = fv2;
        //    mainConfigWriter.saveCommonOptions(commonOpts);
        //    mainConfigWriter.write();
        //}
        public static void saveGlobalConfig() {
            mainConfigWriter.write();
        }
        public const string ID_PREFIX_TEMPORARY = "id_";
        public const char COMMENT_DELIMITER_TEMPORARY = '_';
        // #1 - id, #3 - mass, #4 - comment (can contain next info o2/co2)
        // and/or use id+mass pair
        public static readonly Regex expression = new Regex(@"^id_([\w-[_]]+)(_([1-9]\d*?)(_.*?){0,1}){0,1}$");
        public static double[,] LoadLibrary(List<PreciseEditorData> peds) {
            int rank = peds.Count;
            var lib = TagHolder.getLibraryReader(libraryName);
            
            Match match;
            var ids = new List<string>(rank);
            var masses = new List<string>(rank);
            foreach (var ped in peds) {
                match = expression.Match(ped.Comment);
                if (match.Success) {
                    var groups = match.Groups;
                    try {
                        var id = groups[1].Value;
                        if (ids.Contains(id)) {
                            // duplicate substance
                            return null;
                        }
                        ids.Add(id);
                        if (!groups[3].Success){
                            //no mass
                            return null;
                        }
                        var mass = groups[3].Value;
                        if (masses.Contains(mass)) {
                            // duplicate mass
                            return null;
                        }
                        masses.Add(mass);
                    } catch (FormatException) {
                        //error. wrong string format.
                        return null;
                    }
                } else {
                    //error. no id.
                    return null;
                }
            }
            lib.readOnce(ids, masses);
            
            // TODO: form PreciseData and matrix simultaneously directly from library
            // use current coeffs for that
            double[,] matrix = new double[rank, rank];

            // Important! In sort order according to ids
            for (int i = 0; i < rank; ++i) {
                var massesForIdTable = lib.Masses(ids[i]);
                for (int j = 0; j < rank; ++j) {
                    // Important! In sort order according to masses
                    string currentMass = masses[j];
                    // !!! rows and columns
                    if (massesForIdTable.ContainsKey(currentMass)) {
                        var temp = massesForIdTable[currentMass];
                        matrix[j, i] = (double)temp;
                    } else
                        matrix[j, i] = 0;
                }
            }
            return matrix;
        }
        #endregion
        #region Spectra I/O
        #region Manual Actions
        public static bool openSpectrumFile(string filename, bool hint, out Graph graph) {
            return TagHolder.getSpectrumReader(filename, hint).readSpectrum(out graph);
        }
        public static void saveSpectrumFile(string filename, Graph graph) {
            var writer = TagHolder.getSpectrumWriter(filename, graph);
            DateTime dt = graph.DateTime;
            if (dt != DateTime.MaxValue)
                writer.setTimeStamp(dt);
            writer.write();
        }
        public static void savePreciseSpectrumFile(string filename, Graph graph) {
            var writer = TagHolder.getSpectrumWriter(filename, graph);
            writer.savePreciseData(graph.PreciseData, false);
            DateTime dt = graph.DateTime;
            if (dt != DateTime.MaxValue)
                writer.setTimeStamp(dt);
            writer.setShift(graph.Shift);
            writer.write();
        }
        #region Spectra Distraction
        public static void distractSpectra(string what, ushort step, byte? collectorNumber, PreciseEditorData pedReference, Graph graph) {
            bool hint = !graph.isPreciseSpectrum;
            var reader = TagHolder.getSpectrumReader(what, hint);
            if (reader.Hint != hint) {
                throw new ConfigLoadException("Несовпадение типов спектров", "Ошибка при вычитании спектров", what);
            }
            if (graph.DisplayingMode == Graph.Displaying.Diff) {
                //diffs can't be distracted!
                throw new ArgumentOutOfRangeException();
            }
            if (hint) {
                var points = new PointPairListPlus[COLLECTOR_COEFFS.Length];
                for (int i = 0; i < points.Length; ++i) {
                    points[i] = new PointPairListPlus();
                }
                CommonOptions commonOpts;
                if (reader.openSpectrumFile(out commonOpts, points) == Graph.Displaying.Measured) {
                    // TODO: check commonOpts for equality?
                    if (points.Length != graph.Collectors.Count)
                        throw new ConfigLoadException("Несовпадение рядов данных", "Ошибка при вычитании спектров", what);
                    // coeff counting
                    double coeff = 1.0;
                    if (collectorNumber.HasValue) {
                        // TODO: proper rows for variable number
                        var PL = graph.Collectors[collectorNumber.Value - 1][0].Step;
                        var pl = points[collectorNumber.Value - 1];
                        
                        if (step != 0) {
                            for (int i = 0; i < PL.Count; ++i) {
                                if (step == PL[i].X) {
                                    if (step != pl[i].X)
                                        throw new ArgumentException();
                                    if ((pl[i].Y != 0) && (PL[i].Y != 0))
                                        coeff = PL[i].Y / pl[i].Y;
                                    break;
                                }
                            }
                        }
                    }
                    try {
                        var diffs = new PointPairListPlus[COLLECTOR_COEFFS.Length];
                        for (int i = 0; i < diffs.Length; ++i) {
                            diffs[i] = PointPairListDiff(graph.Collectors[i][0].Step, points[i], coeff);
                        }
                        graph.updateGraphAfterScanDiff(diffs);
                    } catch (System.ArgumentException) {
                        throw new ConfigLoadException("Несовпадение рядов данных", "Ошибка при вычитании спектров", what);
                    }
                } else {
                    //diffs can't be distracted!
                    throw new ArgumentOutOfRangeException();
                }
                return;
            }
            var peds = new PreciseSpectrum();
            if (reader.openPreciseSpectrumFile(peds)) {
                var temp = new List<PreciseEditorData>(graph.PreciseData);
                temp.Sort();
                try {
                    temp = PreciseEditorDataListDiff(temp, peds, step, pedReference);
                    graph.updateGraphAfterPreciseDiff(temp);
                } catch (ArgumentException) {
                    throw new ConfigLoadException("Несовпадение рядов данных", "Ошибка при вычитании спектров", what);
                }
            }
        }
        static PointPairListPlus PointPairListDiff(PointPairListPlus from, PointPairListPlus what, double coeff) {
            if (from.Count != what.Count)
                throw new System.ArgumentOutOfRangeException();

            var res = new PointPairListPlus(from, null, null);
            for (int i = 0; i < res.Count; ++i) {
                if (res[i].X != what[i].X)
                    throw new ArgumentException();
                res[i].Y -= what[i].Y * coeff;
            }
            return res;
        }
        static PreciseEditorData PreciseEditorDataDiff(PreciseEditorData target, PreciseEditorData what, double coeff) {
            if (target.CompareTo(what) != 0)
                throw new ArgumentException();
            if ((target.AssociatedPoints == null || target.AssociatedPoints.Count == 0) ^ (what.AssociatedPoints == null || what.AssociatedPoints.Count == 0))
                throw new ArgumentException();
            if (target.AssociatedPoints != null && what.AssociatedPoints != null && target.AssociatedPoints.Count != what.AssociatedPoints.Count)
                throw new ArgumentException();
            if ((target.AssociatedPoints == null || target.AssociatedPoints.Count == 0) && (what.AssociatedPoints == null || what.AssociatedPoints.Count == 0))
                return new PreciseEditorData(target);
            if (target.AssociatedPoints.Count != 2 * target.Width + 1)
                throw new ArgumentException();
            var res = new PreciseEditorData(target);
            for (int i = 0; i < res.AssociatedPoints.Count; ++i) {
                if (res.AssociatedPoints[i].X != what.AssociatedPoints[i].X)
                    throw new ArgumentException();
                res.AssociatedPoints[i].Y -= what.AssociatedPoints[i].Y * coeff;
            }
            return res;
        }
        static List<PreciseEditorData> PreciseEditorDataListDiff(List<PreciseEditorData> from, List<PreciseEditorData> what, ushort step, PreciseEditorData pedReference) {
            if (from.Count != what.Count)
                throw new ArgumentOutOfRangeException();

            // coeff counting
            double coeff = 1.0;
            if (pedReference != null) {
                int fromIndex = from.IndexOf(pedReference);
                int whatIndex = what.IndexOf(pedReference);
                if ((fromIndex == -1) || (whatIndex == -1))
                    throw new ArgumentException();
                if (System.Math.Abs(from[fromIndex].Step - step) > from[fromIndex].Width)
                    throw new ArgumentOutOfRangeException();
                if (from[fromIndex].AssociatedPoints.Count != what[whatIndex].AssociatedPoints.Count)
                    throw new ArgumentException();
                if (from[fromIndex].AssociatedPoints.Count != 2 * from[fromIndex].Width + 1)
                    throw new ArgumentException();

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
                        foreach (var pp in what[whatIndex].AssociatedPoints) {
                            sumWhat += pp.Y;
                        }
                        if (sumWhat != 0)
                            coeff = sumFrom / sumWhat;
                    }
                }
            }

            var res = new List<PreciseEditorData>(from);
            for (int i = 0; i < res.Count; ++i) {
                res[i] = PreciseEditorDataDiff(res[i], what[i], coeff);
            }
            return res;
        }
        #endregion
        #endregion
        #region Automatic Actions
        static string genAutoSaveFilename(string extension, DateTime now) {
            string dirname;
            dirname = Path.Combine(INITIAL_DIR, string.Format("{0}-{1}-{2}", now.Year, now.Month, now.Day));
            if (!Directory.Exists(@dirname)) {
                Directory.CreateDirectory(@dirname);
            }
            return Path.Combine(dirname, string.Format("{0}-{1}-{2}-{3}.", now.Hour, now.Minute, now.Second, now.Millisecond) + extension);
        }
        public static void autoSaveSpectrumFile() {
            var g = Graph.MeasureGraph.Instance;
            DateTime dt = g.DateTime;
            string filename = genAutoSaveFilename(SPECTRUM_EXT, dt);

            var writer = TagHolder.getSpectrumWriter(filename, g);
            writer.setTimeStamp(dt);
            writer.write();
        }
        public static void autoSavePreciseSpectrumFile() {
            var g = Graph.MeasureGraph.Instance;
            DateTime dt = g.DateTime;
            short? shift = g.Shift;
            string filename = genAutoSaveFilename(PRECISE_SPECTRUM_EXT, dt);

            var writer = TagHolder.getSpectrumWriter(filename, g);
            writer.setTimeStamp(dt);
            writer.setShift(shift);
            writer.savePreciseData(g.PreciseData, false);
            writer.write();
        }
        public static void autoSaveMonitorSpectrumFile(int? labelNumber) {
            var g = Graph.MeasureGraph.Instance;
            DateTime dt = g.DateTime;
            short? shift = g.Shift;
            //autoSavePreciseSpectrumFile(shift);
            var writer = MonitorSaveMaintainer.getMonitorWriter(dt, g);
            writer.setShift(shift);
            if (savedSolution != null) {
                AutoSaveSolvedSpectra(writer);
            }
            // TODO: separate resolved file write-out
            if (labelNumber.HasValue)
                writer.write(labelNumber.Value);
            else
                writer.write();
        }
        static double[] savedSolution = null;
        static void AutoSaveSolvedSpectra(IMonitorWriter writer) {
            if (savedSolution == null) {
                // error
            }
            writer.setSolvedResult(savedSolution);
            savedSolution = null;
        }
        public static void AutoSaveSolvedSpectra(double[] solution) {
            // TODO: simplify
            if (MonitorSaveMaintainer.InstanceExists)
                MonitorSaveMaintainer.getMonitorWriter(DateTime.MinValue, Graph.MeasureGraph.Instance).setSolvedResult(solution);
            else
                savedSolution = solution;
        }
        public static void finalizeMonitorFile() {
            // TODO: simplify
            if (MonitorSaveMaintainer.InstanceExists)
                MonitorSaveMaintainer.getMonitorWriter(DateTime.MinValue, Graph.MeasureGraph.Instance).finalize();
        }
        #endregion
        #endregion
        #region Config I/O
        public static List<PreciseEditorData> loadPreciseOptions(string pedConfName) {
            return TagHolder.getPreciseDataReader(pedConfName).loadPreciseData();
        }
        public static void savePreciseOptions(List<PreciseEditorData> peds, string pedConfName, bool savePeakSum) {
            var writer = TagHolder.getPreciseDataWriter(pedConfName);
            writer.savePreciseData(peds, savePeakSum);
            writer.write();
        }

        public static CommonOptions loadCommonOptions(string cdConfName) {
            return TagHolder.getCommonOptionsReader(cdConfName).loadCommonOptions();
        }
        public static void saveCommonOptions(string filename, ushort eT, ushort iT, double iV, double cp, double eC, double hC, double fv1, double fv2) {
            var writer = TagHolder.getCommonOptionsWriter(filename);
            writer.saveCommonOptions(eT, iT, iV, cp, eC, hC, fv1, fv2);
            writer.write();
        }
        public static void temp_saveCO(string filename, double d1v, double d2v, double d3v, double iV, double eC, double fv1, double fv2, double c, double k, double expT) {
            var writer = TagHolder.getCommonOptionsWriter(filename);
            var co = new CommonOptions();
            co.iVoltageReal = iV;
            co.eCurrentReal = eC;
            co.fV1Real = fv1;
            co.fV2Real = fv2;
            co.d1VReal = d1v;
            co.d2VReal = d2v;
            co.d3VReal = d3v;
            co.C = c;
            co.K = k;
            co.eTimeReal = (ushort)expT;
            writer.saveCommonOptions(co);
            writer.write();
        }
        #endregion
        #region Error messages on loading different configs
        public class ConfigLoadException: Exception {
            public ConfigLoadException(string message, string filestring, string confname)
                : base(message) {
                Data["FS"] = filestring;
                Data["CN"] = confname;
            }
            public void visualise() {
                // TODO: move messageboxes up!
                if (!(Data["CN"].Equals(mainConfigName)))
                    System.Windows.Forms.MessageBox.Show(Message, (string)(Data["FS"]));
                else
                    System.Windows.Forms.MessageBox.Show(Message, "Ошибка чтения конфигурационного файла");
            }
        }
        class wrongFormatOnLoadPrecise: wrongFormatOnLoad {
            public wrongFormatOnLoadPrecise(string configName)
                : base(configName, "Ошибка чтения файла прецизионных точек") { }
        }
        class wrongFormatOnLoad: ConfigLoadException {
            public wrongFormatOnLoad(string configName, string errorFile)
                : base("Неверный формат данных", errorFile, configName) { }
        }
        class structureErrorOnLoad: ConfigLoadException {
            public structureErrorOnLoad(string configName, string errorFile)
                : base("Ошибка структуры файла", errorFile, configName) { }
        }
        class structureErrorOnLoadCommonData: structureErrorOnLoad {
            public structureErrorOnLoadCommonData(string configName)
                : base(configName, "Ошибка чтения файла общих настроек") { }
        }
        class structureErrorOnLoadPrecise: structureErrorOnLoad {
            public structureErrorOnLoadPrecise(string configName)
                : base(configName, "Ошибка чтения файла прецизионных точек") { }
        }
        #endregion
        #region Graph scaling to mass coeffs
        public static void setScalingCoeff(byte col, ushort pnt, double mass) {
            // TODO: use params or remove them
            mainConfigWriter.write();
        }
        #endregion
        #region Logging routines
        static StreamWriter openLog() {
            try {
                return new StreamWriter(@logName, true){ AutoFlush = true };
            } catch (Exception e) {
                // TODO: move messagebox up
                System.Windows.Forms.MessageBox.Show(e.Message, "Ошибка при открытии файла отказов");
                return null;
            }
        }
        static string genMessage(string data, DateTime moment) {
            return string.Format("{0}-{1}-{2}|", moment.Year, moment.Month, moment.Day) +
                string.Format("{0}.{1}.{2}.{3}: ", moment.Hour, moment.Minute, moment.Second, moment.Millisecond) + data;
        }
        static void log(StreamWriter errorLog, string msg) {
            DateTime now = System.DateTime.Now;
            try {
                errorLog.WriteLine(genMessage(msg, now));
                //errorLog.Flush();
            } catch (Exception Error) {
                string message = "Error log write failure ";
                string cause = "(" + msg + ") -- " + Error.Message;
                ConsoleWriter.WriteLine(message + cause);
                // TODO: move messagebox up
                System.Windows.Forms.MessageBox.Show(cause, message);
            } finally {
                errorLog.Close();
            }
        }
        // TODO: remove duplicate
        public static void logCrash(string command) {
            StreamWriter errorLog;
            if ((errorLog = openLog()) == null)
                return;
            log(errorLog, command);
        }
        public static void logTurboPumpAlert(string message) {
            StreamWriter errorLog;
            if ((errorLog = openLog()) == null)
                return;
            log(errorLog, message);
        }
        #endregion
        #region Common config interfaces
        interface ITimeStamp {
            void setTimeStamp(DateTime dt);
        }
        interface IShift {
            void setShift(short? shift);
        }
        interface IAnyConfig {}
        interface IAnyReader: IAnyConfig {}
        interface IAnyWriter: IAnyConfig {
            void write(params object[] data);
        }
        interface ICommonOptionsReader: IAnyReader {
            CommonOptions loadCommonOptions();
        }
        interface IPreciseDataReader: IAnyReader {
            List<PreciseEditorData> loadPreciseData();
        }
        interface ICommonOptionsWriter: IAnyWriter {
            void saveCommonOptions(ushort eT, ushort iT, double iV, double cp, double eC, double hC, double fv1, double fv2);
            void saveCommonOptions(CommonOptions opts);
        }
        interface IPreciseDataWriter: IAnyWriter {
            void savePreciseData(List<PreciseEditorData> peds, bool savePeakSum);
        }
        #endregion
        #region Additive configs
        interface IMonitorWriter: IAnyWriter, IShift {
            void setSolvedResult(double[] solution);
            void finalize();
        }
        abstract class MonitorSaveMaintainer {
            abstract class CurrentMonitorSaveMaintainer: MonitorSaveMaintainer {
                const string VERSION_NUMBER = "1.0";

                const string LINE_TERMINATOR = "\r\n";

                const char HEADER_FOOTER_FIRST_SYMBOL = '#';
                const char HEADER_FOOTER_DELIMITER = ' ';
                const string HEADER_TITLE = "monitor";
                const string HEADER_VERSION = "version";
                const string HEADER_COMMON_OPTIONS = "common";
                const string HEADER_PRECISE_OPTIONS = "precise";
                const string HEADER_START_TIME = "start";
                const string LABEL = "label";
                const string FOOTER_TITLE = "end";
                const string FOOTER_REASON = "reason";
                const string FOOTER_REASON_FINISH = "finish";
                const string FOOTER_REASON_DATE_CHANGE = "next";
                const string FOOTER_LINK = "link";

                //const char DATA_DELIMITER = '|';
                const char DATA_DELIMITER = '\t';
                const string NO_SHIFT_PLACEHOLDER = "-";

                const string RESOLVED_APPENDIX = "r";

                public class Writer: CurrentMonitorSaveMaintainer, IMonitorWriter {
                    readonly DateTime initialDT;
                    readonly string filename;
                    readonly CommonOptions opts;
                    readonly List<PreciseEditorData> precData;
                    readonly string header;
                    readonly StreamWriter sw;
                    readonly StreamWriter swResolved;
                    static Writer instance = null;
                    double[] solution = null;
                    public static IMonitorWriter getInstance(DateTime dt, Graph graph) {
                        if (instance == null) {
                            instance = new Writer(dt, graph);
                        } else if (instance.initialDT.Date < dt.Date) {
                            Writer old = instance;
                            instance = new Writer(instance, dt);
                            //instance.graph = graph;
                            old.finalize(instance.filename);
                        }
                        instance.currentDT = dt;
                        return instance;
                    }
                    public static new bool InstanceExists {
                        get {
                            // temporary solution
                            return instance != null;
                        }
                    }
                    DateTime currentDT;
                    short? shift = null;
                    Writer(DateTime dt, Graph graph) {
                        initialDT = dt;
                        // TODO: copy!
                        //this.graph = graph;
                        opts = graph.CommonOptions;
                        precData = new List<PreciseEditorData>(graph.PreciseData);
                        //!!!
                        precData.Sort(PreciseEditorData.ComparePreciseEditorDataByPeakValue);
                        header = generateHeader();
                        initFile(dt, out filename, out sw, out swResolved);
                    }
                    Writer(Writer other, DateTime dt) {
                        initialDT = dt;
                        opts = other.opts;
                        precData = other.precData;
                        header = other.header;
                        initFile(dt, out filename, out sw, out swResolved);
                    }
                    void initFile(DateTime dt, out string filename, out StreamWriter sw, out StreamWriter swResolved) {
                        filename = genAutoSaveFilename(MONITOR_SPECTRUM_EXT, dt);
                        sw = new StreamWriter(filename, true);
                        sw.WriteLine(header);
                        sw.WriteLine(string.Format(DateTimeFormatInfo.InvariantInfo, "{0}{1}{2}{3:G}", HEADER_FOOTER_FIRST_SYMBOL, HEADER_START_TIME, HEADER_FOOTER_DELIMITER, initialDT));

                        swResolved = new StreamWriter(filename + RESOLVED_APPENDIX, true);
                        swResolved.WriteLine(header);
                        swResolved.WriteLine(string.Format(DateTimeFormatInfo.InvariantInfo, "{0}{1}{2}{3:G}", HEADER_FOOTER_FIRST_SYMBOL, HEADER_START_TIME, HEADER_FOOTER_DELIMITER, initialDT));
                    }
                    string generateHeader() {
                        var sb = (new StringBuilder(header))
                            .Append(HEADER_FOOTER_FIRST_SYMBOL)
                            .Append(HEADER_TITLE)
                            .Append(HEADER_FOOTER_DELIMITER)
                            .Append(HEADER_VERSION)
                            .Append(HEADER_FOOTER_DELIMITER)
                            .Append(VERSION_NUMBER)
                            .Append(LINE_TERMINATOR)
                            .Append(HEADER_FOOTER_FIRST_SYMBOL)
                            .Append(HEADER_COMMON_OPTIONS)
                            .Append(HEADER_FOOTER_DELIMITER)
                            .Append(opts)
                            .Append(LINE_TERMINATOR)
                            .Append(HEADER_FOOTER_FIRST_SYMBOL)
                            .Append(HEADER_PRECISE_OPTIONS)
                            .Append(HEADER_FOOTER_DELIMITER);
                        foreach (var ped in precData) {
                            sb.Append(ped);
                        }
                        return sb.ToString();
                    }
                    void finalize(string nextFilename) {
                        var sb = (new StringBuilder())
                            .Append(HEADER_FOOTER_FIRST_SYMBOL)
                            .Append(FOOTER_TITLE);
                        if (nextFilename == null) {
                            sb
                                .Append(HEADER_FOOTER_DELIMITER)
                                .Append(FOOTER_REASON)
                                .Append(HEADER_FOOTER_DELIMITER)
                                .Append(FOOTER_REASON_FINISH);
                        } else {
                            sb
                                .Append(HEADER_FOOTER_DELIMITER)
                                .Append(FOOTER_REASON)
                                .Append(HEADER_FOOTER_DELIMITER)
                                .Append(FOOTER_REASON_DATE_CHANGE)
                                .Append(HEADER_FOOTER_DELIMITER)
                                .Append(FOOTER_LINK)
                                .Append(HEADER_FOOTER_DELIMITER)
                                .Append(nextFilename);
                        }
                        sw.WriteLine(sb);
                        sw.Close();

                        swResolved.WriteLine(sb + (nextFilename == null ? "" : RESOLVED_APPENDIX));
                        swResolved.Close();
                    }
                    #region IAnyWriter Members
                    public void write(params object[] data) {
                        // special label write-out
                        if (data.Length != 0) {
                            string label = LABEL + (int)data[0];
                            sw.WriteLine(label);
                            swResolved.WriteLine(label);
                        }
                        var sb = (new StringBuilder())
                            .AppendFormat(DateTimeFormatInfo.InvariantInfo, "{0:T}", currentDT)
                            .Append(DATA_DELIMITER)
                            .Append(shift == null ? NO_SHIFT_PLACEHOLDER : shift.ToString());
                        
                        if (solution != null) {
                            swResolved.Write(sb);
                            foreach (double d in solution) {
                                swResolved.Write(DATA_DELIMITER);
                                swResolved.Write(d);
                            }
                            swResolved.WriteLine();
                        }
                        
                        foreach (var ped in precData) {
                            if (ped.Use) {
                                sb
                                    .Append(DATA_DELIMITER)
                                    .Append(ped.AssociatedPoints.PLSreference.PeakSum);
                            }
                        }
                        sw.WriteLine(sb);
                        sw.Flush();
                        swResolved.Flush();
                    }
                    #endregion
                    #region IShift Members
                    public void setShift(short? shift) {
                        this.shift = shift;
                    }
                    #endregion
                    #region IMonitorWriter Members
                    public void setSolvedResult(double[] solution) {
                        // in fact this method is called before write(). check this!
                        this.solution = solution;
                    }
                    public void finalize() {
                        finalize(null);
                        instance = null;
                    }
                    #endregion
                }
                public static new IMonitorWriter getMonitorWriter(DateTime dt, Graph graph) {
                    return Writer.getInstance(dt, graph);
                }
                public new static bool InstanceExists {
                    // temporary solution
                    get { return Writer.InstanceExists; }
                }
            }
            public static IMonitorWriter getMonitorWriter(DateTime dt, Graph graph) {
                return CurrentMonitorSaveMaintainer.getMonitorWriter(dt, graph);
            }
            public static bool InstanceExists {
                // temporary solution
                get { return CurrentMonitorSaveMaintainer.InstanceExists; }
            }
        }
        #endregion
        #region XML configs
        interface IScalingCoeffsReader: IAnyReader {
            double[] loadScalingCoeffs();
        }
        interface ISpectrumReader: ICommonOptionsReader, IPreciseDataReader {
            bool readSpectrum(out Graph graph);
            bool Hint { get; set; }
            Graph.Displaying openSpectrumFile(out CommonOptions commonOpts, params PointPairListPlus[] points);
            bool openPreciseSpectrumFile(PreciseSpectrum peds);
        }
        interface IMainConfig: ICommonOptionsReader, IPreciseDataReader, IScalingCoeffsReader {
            void read();
            XmlDocument XML { get; }
        }
        interface ILibraryReader: IAnyReader {
            void readOnce(List<string> ids, List<string> loadedMasses);
            System.Collections.Hashtable Masses(string id);
        }
        interface IScalingCoeffsWriter: IAnyWriter {
            void saveScalingCoeffs(params double[] coeffs);
        }
        interface ISpectrumWriter: ICommonOptionsWriter, IPreciseDataWriter, ITimeStamp, IShift, IScalingCoeffsWriter {
            void saveScanOptions(Graph graph);
        }
        interface IMainConfigWriter: ICommonOptionsWriter, IPreciseDataWriter, IScalingCoeffsWriter { }
        abstract class TagHolder {
            #region Tags
            const string ROOT_CONFIG_TAG = "control";
            const string VERSION_ATTRIBUTE = "version";

            const string HEADER_CONFIG_TAG = "header";

            const string CONNECT_CONFIG_TAG = "connect";
            const string PORT_CONFIG_TAG = "port";
            const string BAUDRATE_CONFIG_TAG = "baudrate";
            const string TRY_NUMBER_CONFIG_TAG = "try";

            const string COMMON_CONFIG_TAG = "common";
            const string EXPOSITURE_TIME_CONFIG_TAG = "exptime";
            const string TRANSITION_TIME_CONFIG_TAG = "meastime";
            const string IONIZATION_VOLTAGE_CONFIG_TAG = "ivoltage";
            const string CAPACITOR_VOLTAGE_COEFF_CONFIG_TAG = "cp";
            const string EMISSION_CURRENT_CONFIG_TAG = "ecurrent";
            const string HEAT_CURRENT_CONFIG_TAG = "hcurrent";
            const string FOCUS_VOLTAGE1_CONFIG_TAG = "focus1";
            const string FOCUS_VOLTAGE2_CONFIG_TAG = "focus2";

            const string DELAY_BEFORE_MEASURE_CONFIG_TAG = "before";
            const string EQUAL_DELAYS_CONFIG_TAG = "equal";
            const string DELAY_FORWARD_MEASURE_CONFIG_TAG = "forward";
            const string DELAY_BACKWARD_MEASURE_CONFIG_TAG = "back";

            const string POINT_CONFIG_TAG = "p";

            const string OVERVIEW_CONFIG_TAG = "overview";
            const string START_SCAN_CONFIG_TAG = "start";
            const string END_SCAN_CONFIG_TAG = "end";
            const string COL1_CONFIG_TAG = "collector1";
            const string COL2_CONFIG_TAG = "collector2";

            const string SENSE_CONFIG_TAG = "sense";
            const string PEAK_TAGS_FORMAT = "region{0}";
            const string PEAK_NUMBER_CONFIG_TAG = "peak";
            const string PEAK_COL_NUMBER_CONFIG_TAG = "col";
            const string PEAK_WIDTH_CONFIG_TAG = "width";
            const string PEAK_ITER_NUMBER_CONFIG_TAG = "iteration";
            const string PEAK_PRECISION_CONFIG_TAG = "error";
            const string PEAK_COMMENT_CONFIG_TAG = "comment";
            const string PEAK_USE_CONFIG_TAG = "use";

            const string PEAK_COUNT_SUM_CONFIG_TAG = "sum";

            const string CHECK_CONFIG_TAG = "check";
            const string CHECK_ITER_NUMBER_CONFIG_TAG = "iterations";
            const string CHECK_TIME_LIMIT_CONFIG_TAG = "limit";
            const string CHECK_MAX_SHIFT_CONFIG_TAG = "allowed";

            const string BACKGROUND_CYCLES_NUMBER_TAG = "cycles";

            const string INTERFACE_CONFIG_TAG = "interface";
            const string C1_CONFIG_TAG = "coeff1";
            const string C2_CONFIG_TAG = "coeff2";

            const string TIME_SPECTRUM_ATTRIBUTE = "time";
            const string SHIFT_SPECTRUM_ATTRIBUTE = "shift";
            #endregion
            #region Spectra headers
            const string PRECISE_OPTIONS_HEADER = "Precise options";
            const string COMMON_OPTIONS_HEADER = "Common options";
            const string MEASURED_SPECTRUM_HEADER = "Measure";
            const string DIFF_SPECTRUM_HEADER = "Diff";
            #endregion
            #region Error Messages
            const string CONFIG_FILE_STRUCTURE_ERROR = "Ошибка структуры конфигурационного файла";
            const string CONFIG_FILE_READ_ERROR = "Ошибка чтения конфигурационного файла";
            const string LIBRARY_FILE_READ_ERROR = "Ошибка чтения файла библиотеки спектров";
            #endregion
            string filename;
            XmlDocument xmlData;
            protected void initialize(string filename, XmlDocument doc) {
                this.filename = filename;
                xmlData = doc;
            }
            abstract class Version1_2TagHolder: TagHolder {
                const char COUNTS_SEPARATOR = ' ';
                const string CHECK_PEAK_NUMBER_TAG = "region";
                public const string CONFIG_VERSION = "1.2";
                #region Version1_2 Readers
                public abstract class Reader: Version1_2TagHolder {
                    public CommonOptions loadCommonOptions() {
                        var commonNode = xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, COMMON_CONFIG_TAG));
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
                            {
                                CommonOptions opts = new CommonOptions();
                                opts.eTime = eT;
                                opts.iTime = iT;
                                opts.iVoltage = iV;
                                opts.CP = CP;
                                opts.eCurrent = eC;
                                //opts.hCurrent = hC;
                                opts.fV1 = fV1;
                                opts.fV2 = fV2;
                                loadDelays(commonNode, opts);
                                return opts;
                            }
                        } catch (NullReferenceException) {
                            throw new structureErrorOnLoadCommonData(filename);
                        }
                    }
                    public List<PreciseEditorData> loadPreciseData() {
                        try {
                            return LoadPED("");
                        } catch (ConfigLoadException) {
                            return null;
                        }
                    }
                    protected List<PreciseEditorData> LoadPED(string errorMessage) {
                        string prefix = combine(ROOT_CONFIG_TAG, SENSE_CONFIG_TAG);
                        var peds = new List<PreciseEditorData>();
                        for (int i = 1; i <= Config.PEAK_NUMBER; ++i) {
                            string peak, iter, width, col;
                            try {
                                var regionNode = xmlData.SelectSingleNode(combine(prefix, string.Format(PEAK_TAGS_FORMAT, i)));
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
                                        var temp = new PreciseEditorData(use, (byte)(i - 1), peakStep,
                                                                            byte.Parse(col), ushort.Parse(iter),
                                                                            ushort.Parse(width), (float)0, comment);
                                        peakStep -= peakWidth;
                                        peakWidth += peakWidth += peakStep;
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
                        peds.Sort();
                        return peds;
                    }
                    protected virtual PointPairListPlus readPeaks(XmlNode regionNode, ushort peakStep, ushort peakWidth) { return null; }
                    protected virtual void loadDelays(XmlNode commonNode, CommonOptions opts) { }
                }
                public class CommonOptionsReader: Reader, ICommonOptionsReader { }
                public class PreciseDataReader: Reader, IPreciseDataReader { }
                public abstract class ComplexReader: Reader, IScalingCoeffsReader {
                    public double[] loadScalingCoeffs() {
                        // TODO: class-dependent messages
                        try {
                            var interfaceNode = xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, INTERFACE_CONFIG_TAG));
                            double col1Coeff = double.Parse(interfaceNode.SelectSingleNode(C1_CONFIG_TAG).InnerText, CultureInfo.InvariantCulture);
                            double col2Coeff = double.Parse(interfaceNode.SelectSingleNode(C2_CONFIG_TAG).InnerText, CultureInfo.InvariantCulture);
                            return new[] { col1Coeff, col2Coeff };
                        } catch (NullReferenceException) {
                            throw new ConfigLoadException(CONFIG_FILE_STRUCTURE_ERROR, CONFIG_FILE_READ_ERROR, filename);
                        } catch (FormatException) {
                            throw new ConfigLoadException("Неверный формат данных", CONFIG_FILE_READ_ERROR, filename);
                        }
                    }
                }
                public class MainConfig: ComplexReader, IMainConfig {
                    #region IMainConfig implementation
                    public void read() {
                        string prefix;
                        try {
                            prefix = combine(ROOT_CONFIG_TAG, CONNECT_CONFIG_TAG);
                            Port = (xmlData.SelectSingleNode(combine(prefix, PORT_CONFIG_TAG)).InnerText);
                            BaudRate = int.Parse(xmlData.SelectSingleNode(combine(prefix, BAUDRATE_CONFIG_TAG)).InnerText);
                            Try = byte.Parse(xmlData.SelectSingleNode(combine(prefix, TRY_NUMBER_CONFIG_TAG)).InnerText);
                        } catch (NullReferenceException) {
                            (new ConfigLoadException(CONFIG_FILE_STRUCTURE_ERROR, CONFIG_FILE_READ_ERROR, filename)).visualise();
                            //use hard-coded defaults
                        }
                        try {
                            prefix = combine(ROOT_CONFIG_TAG, OVERVIEW_CONFIG_TAG);
                            sPoint = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, START_SCAN_CONFIG_TAG)).InnerText);
                            ePoint = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, END_SCAN_CONFIG_TAG)).InnerText);
                        } catch (NullReferenceException) {
                            (new ConfigLoadException(CONFIG_FILE_STRUCTURE_ERROR, CONFIG_FILE_READ_ERROR, filename)).visualise();
                            //use hard-coded defaults
                        }
                        try {
                            Graph.MeasureGraph.Instance.Collectors.RecomputeMassRows(loadScalingCoeffs());
                        } catch (ConfigLoadException) {
                            //cle.visualise();
                            //use hard-coded defaults
                        }
                        try {
                            CommonOptions = loadCommonOptions();
                        } catch (ConfigLoadException cle) {
                            cle.visualise();
                            //use hard-coded defaults
                        }
                        try {
                            var pedl = loadPreciseData();
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
                            reperPeak = new PreciseEditorData(false, 255, step, collector, 0, width, 0, "checker peak");
                        } catch (NullReferenceException) {
                            //use hard-coded defaults (null checker peak)
                        } catch (FormatException) {
                            // TODO: very bad..
                            //use hard-coded defaults (null checker peak)
                        }
                        try {
                            Iterations = int.Parse(xmlData.SelectSingleNode(combine(prefix, CHECK_ITER_NUMBER_CONFIG_TAG)).InnerText);
                        } catch (NullReferenceException) {
                            //use hard-coded defaults (infinite iterations)
                        }
                        try {
                            TimeLimit = int.Parse(xmlData.SelectSingleNode(combine(prefix, CHECK_TIME_LIMIT_CONFIG_TAG)).InnerText);
                        } catch (NullReferenceException) {
                            //use hard-coded defaults (no time limit)
                        }
                        try {
                            AllowedShift = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, CHECK_MAX_SHIFT_CONFIG_TAG)).InnerText);
                        } catch (NullReferenceException) {
                            //use hard-coded defaults (zero shift allowed)
                        }
                        try {
                            CheckerPeakIndex = int.Parse(xmlData.SelectSingleNode(combine(prefix, CHECK_PEAK_NUMBER_TAG)).InnerText);
                        } catch (NullReferenceException) {
                            //use hard-coded defaults (-1)
                        }
                        try {
                            BackgroundCycles = byte.Parse(xmlData.SelectSingleNode(combine(prefix, BACKGROUND_CYCLES_NUMBER_TAG)).InnerText);
                        } catch (NullReferenceException) {
                            //use hard-coded defaults (false)
                        }
                        // BAD: really uses previous values! (not default)
                    }
                    public XmlDocument XML {
                        get {
                            return xmlData;
                        }
                    }
                    #endregion
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
                public class LibraryReader: ILibraryReader {
                    const string LIBRARY_TAG = "library";
                    const string SPECTRUM_TAG = "spectrum";
                    const string ID_ATTRIBUTE = "id";
                    const string MASS_ATTRIBUTE = "mass";
                    const string PEAK_TAG = "peak";
                    const string VALUE_ATTRIBUTE = "value";
                    const string CALIBRATION_TIME_ATTRIBUTE = "ct";
                    readonly XmlTextReader reader;
                    System.Collections.Hashtable table;
                    public LibraryReader(string filename) {
                        reader = new XmlTextReader(filename);
                    }
                    #region ILibraryReader Members
                    public void readOnce(List<string> ids, List<string> loadedMasses) {
                        if (reader.ReadState != ReadState.Initial)
                            reader.ResetState();
                        reader.ReadToFollowing(LIBRARY_TAG);
                        reader.GetAttribute(VERSION_ATTRIBUTE);
                        var table = new System.Collections.Hashtable(ids.Count);
                        if (reader.ReadToDescendant(SPECTRUM_TAG)) {
                            do {
                                string id = reader.GetAttribute(ID_ATTRIBUTE);
                                // only 1 peak of a substance! otherwise will be dependent columns in the matrix
                                int index = ids.IndexOf(id);
                                if (index != -1) {
                                    if (loadedMasses[index] == "") {
                                        //error! mass should be stated explicitly!
                                    }
                                    var result = new System.Collections.Hashtable();
                                    if (reader.ReadToDescendant(PEAK_TAG)) {
                                        // TODO: use proper calibration data later in library
                                        string calibrationCoeffString;
                                        string ctString;
                                        string currentMass;
                                        do {
                                            currentMass = reader.GetAttribute(MASS_ATTRIBUTE);
                                            // only desired masses
                                            if (loadedMasses.Contains(currentMass)) {
                                                calibrationCoeffString = reader.GetAttribute(VALUE_ATTRIBUTE);
                                                ctString = reader.GetAttribute(CALIBRATION_TIME_ATTRIBUTE);
                                                if (calibrationCoeffString == null || ctString == null) {
                                                    //error!!! all peaks must have calibration for solving matrix
                                                } else try {
                                                        //actual calibration for current exposition time                                                    
                                                        double calibrationCoeff = Double.Parse(calibrationCoeffString) * Graph.MeasureGraph.Instance.CommonOptions.eTimeReal / Int32.Parse(ctString);
                                                        //check for integral value
                                                        Int32.Parse(currentMass);
                                                        result.Add(currentMass, calibrationCoeff);
                                                    } catch (FormatException) {
                                                        //error!!! all peaks must have calibration for solving matrix
                                                    };
                                            }
                                        } while (reader.ReadToNextSibling(PEAK_TAG));
                                    }
                                    table.Add(id, result);
                                }
                            } while (reader.ReadToNextSibling(SPECTRUM_TAG));
                        }
                        this.table = table;
                    }
                    public System.Collections.Hashtable Masses(string id) {
                        return table[id] as System.Collections.Hashtable;
                    }
                    #endregion
                }
                public class SpectrumReader: ComplexReader, ISpectrumReader {
                    bool hint;
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
                    public Graph.Displaying openSpectrumFile(out CommonOptions commonOpts, params PointPairListPlus[] points) {
                        var pl1 = points[0];
                        var pl2 = points[1];
                        try {
                            string prefix = combine(ROOT_CONFIG_TAG, OVERVIEW_CONFIG_TAG);
                            ushort start = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, START_SCAN_CONFIG_TAG)).InnerText);
                            ushort end = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, END_SCAN_CONFIG_TAG)).InnerText);
                            pl1.AddRange(readPeaks(xmlData.SelectSingleNode(combine(prefix, COL1_CONFIG_TAG)), start, end));
                            pl2.AddRange(readPeaks(xmlData.SelectSingleNode(combine(prefix, COL2_CONFIG_TAG)), start, end));
                        } catch (NullReferenceException) {
                            throw new ConfigLoadException("Ошибка структуры файла", "Ошибка чтения файла спектра", filename);
                        } catch (FormatException) {
                            throw new ConfigLoadException("Неверный формат данных", "Ошибка чтения файла спектра", filename);
                        }
                        commonOpts = loadCommonOptions();

                        pl1.Sort(ZedGraph.SortType.XValues);
                        pl2.Sort(ZedGraph.SortType.XValues);
                        return spectrumType();
                    }
                    public bool openPreciseSpectrumFile(PreciseSpectrum peds) {
                        peds.CommonOptions = loadCommonOptions();
                        try {
                            peds.AddRange(LoadPED("Ошибка чтения файла прецизионного спектра"));
                            return true;
                        } catch (ConfigLoadException) {
                            return false;
                        }
                    }
                    #endregion
                    bool OpenSpecterFile(out Graph graph) {
                        var points = new PointPairListPlus[COLLECTOR_COEFFS.Length];
                        for (int i = 0; i < points.Length; ++i) {
                            points[i] = new PointPairListPlus();
                        }
                        CommonOptions commonOpts;
                        Graph.Displaying result = openSpectrumFile(out commonOpts, points);

                        graph = new Graph(commonOpts, loadScalingCoeffs());
                        switch (result) {
                            case Graph.Displaying.Measured:
                                graph.updateGraphAfterScanLoad(loadTimeStamp(), points);
                                return true;
                            case Graph.Displaying.Diff:
                                graph.updateGraphAfterScanDiff(false, points);
                                return true;
                            default:
                                return false;
                        }
                    }
                    bool OpenPreciseSpecterFile(out Graph graph) {
                        Graph.Displaying res = spectrumType();
                        var peds = new PreciseSpectrum();
                        bool result = openPreciseSpectrumFile(peds);
                        if (result) {
                            graph = new Graph(peds.CommonOptions, loadScalingCoeffs());
                            switch (res) {
                                case Graph.Displaying.Measured:
                                    short shift = short.MaxValue;
                                    try {
                                        shift = loadShift();
                                    } catch (Exception) {
                                        // do nothing
                                    }
                                    graph.updateGraphAfterPreciseLoad(peds, loadTimeStamp(), shift);
                                    return true;
                                case Graph.Displaying.Diff:
                                    graph.updateGraphAfterPreciseDiff(peds, false);
                                    return true;
                                default:
                                    return false;
                            }
                        } else {
                            //TODO: other solution!
                            graph = null;
                        }
                        return result;
                    }
                    Graph.Displaying spectrumType() {
                        var headerNode = xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, HEADER_CONFIG_TAG));
                        return (headerNode != null && headerNode.InnerText == DIFF_SPECTRUM_HEADER) ? Graph.Displaying.Diff : Graph.Displaying.Measured;
                    }
                    DateTime loadTimeStamp() {
                        return DateTime.Parse(getHeaderAttributeText(TIME_SPECTRUM_ATTRIBUTE), DateTimeFormatInfo.InvariantInfo);
                    }
                    short loadShift() {
                        return short.Parse(getHeaderAttributeText(SHIFT_SPECTRUM_ATTRIBUTE));
                    }
                    protected sealed override PointPairListPlus readPeaks(XmlNode regionNode, ushort peakStart, ushort peakEnd) {
                        var tempPntLst = new PointPairListPlus();
                        try {
                            var temp = regionNode.SelectSingleNode(POINT_CONFIG_TAG);
                            if (temp == null)
                                return null;
                            string text = temp.InnerText;
                            string[] parts = text.Split(COUNTS_SEPARATOR);
                            foreach (string str in parts) {
                                // locale?
                                //! double for non-integral counts (after subtraction with renormalization on point/peak)
                                tempPntLst.Add(peakStart, double.Parse(str));
                                ++peakStart;
                            }
                        } catch (FormatException) {
                            // TODO: store exception messages
                            throw new ConfigLoadException("Неверный формат данных", hint ? "Ошибка чтения файла спектра" : "Ошибка чтения файла прецизионного спектра", filename);
                        }
                        if (--peakStart != peakEnd)
                            throw new ConfigLoadException("Несовпадение рядов данных", hint ? "Ошибка чтения файла спектра" : "Ошибка чтения файла прецизионного спектра", filename);

                        return tempPntLst;
                    }
                }
                #endregion
                #region Version1_2 Writers
                public abstract class Writer: Version1_2TagHolder {
                    public virtual void write(params object[] data) {
                        xmlData.Save(filename);
                    }
                    public void saveCommonOptions(CommonOptions opts) {
                        var commonNode = xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, COMMON_CONFIG_TAG));
                        saveCommonOptions(commonNode, opts);
                    }
                    public void saveCommonOptions(ushort eT, ushort iT, double iV, double cp, double eC, double hC, double fv1, double fv2) {
                        var opts = new CommonOptions();
                        opts.eTimeReal = eT;
                        opts.iTimeReal = iT;
                        opts.iVoltageReal = iV;
                        opts.CPReal = cp;
                        opts.eCurrentReal = eC;
                        //opts.hCurrentReal = hC;
                        opts.fV1Real = fv1;
                        opts.fV2Real = fv2;
                        saveCommonOptions(opts);
                    }
                    void saveCommonOptions(XmlNode commonNode, CommonOptions opts) {
                        commonNode.SelectSingleNode(EXPOSITURE_TIME_CONFIG_TAG).InnerText = opts.eTime.ToString();
                        commonNode.SelectSingleNode(TRANSITION_TIME_CONFIG_TAG).InnerText = opts.iTime.ToString();
                        commonNode.SelectSingleNode(IONIZATION_VOLTAGE_CONFIG_TAG).InnerText = opts.iVoltage.ToString();
                        commonNode.SelectSingleNode(CAPACITOR_VOLTAGE_COEFF_CONFIG_TAG).InnerText = opts.CP.ToString();
                        commonNode.SelectSingleNode(EMISSION_CURRENT_CONFIG_TAG).InnerText = opts.eCurrent.ToString();
                        //commonNode.SelectSingleNode(HEAT_CURRENT_CONFIG_TAG).InnerText = opts.hCurrent.ToString();
                        commonNode.SelectSingleNode(FOCUS_VOLTAGE1_CONFIG_TAG).InnerText = opts.fV1.ToString();
                        commonNode.SelectSingleNode(FOCUS_VOLTAGE2_CONFIG_TAG).InnerText = opts.fV2.ToString();
                        /*commonNode.SelectSingleNode(DELAY_BEFORE_MEASURE_CONFIG_TAG).InnerText = Config.commonOpts.befTime.ToString();
                        commonNode.SelectSingleNode(EQUAL_DELAYS_CONFIG_TAG).InnerText = Config.commonOpts.ForwardTimeEqualsBeforeTime.ToString();
                        commonNode.SelectSingleNode(DELAY_FORWARD_MEASURE_CONFIG_TAG).InnerText = Config.commonOpts.fTime.ToString();
                        commonNode.SelectSingleNode(DELAY_BACKWARD_MEASURE_CONFIG_TAG).InnerText = Config.commonOpts.bTime.ToString();*/
                    }
                    public void savePreciseData(List<PreciseEditorData> peds, bool savePeakSum) {
                        clearOldValues();
                        foreach (var ped in peds) {
                            var regionNode = xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, SENSE_CONFIG_TAG, string.Format(PEAK_TAGS_FORMAT, ped.pNumber + 1)));
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
                    protected virtual void clearOldValues() { }
                    protected virtual void savePointRows(PointPairListPlus row, XmlNode node) { }
                }
                public class CommonOptionsWriter: Writer, ICommonOptionsWriter { }
                public class PreciseDataWriter: Writer, IPreciseDataWriter { }
                public abstract class ComplexWriter: Writer, IScalingCoeffsWriter {
                    public void saveScalingCoeffs(params double[] coeffs) {
                        clearInnerText(ROOT_CONFIG_TAG, INTERFACE_CONFIG_TAG);
                        string prefix = combine(ROOT_CONFIG_TAG, INTERFACE_CONFIG_TAG);
                        fillInnerText(prefix, C1_CONFIG_TAG, coeffs[0].ToString("R", CultureInfo.InvariantCulture));
                        fillInnerText(prefix, C2_CONFIG_TAG, coeffs[1].ToString("R", CultureInfo.InvariantCulture));
                    }
                }
                public class SpectrumWriter: ComplexWriter, ISpectrumWriter {
                    protected sealed override void savePointRows(PointPairListPlus row, XmlNode node) {
                        if (row.Count == 0)
                            return;
                        var sb = new StringBuilder();
                        foreach (ZedGraph.PointPair pp in row) {
                            sb.Append(pp.Y);
                            sb.Append(COUNTS_SEPARATOR);
                        }
                        var temp = xmlData.CreateElement(POINT_CONFIG_TAG); ;
                        temp.InnerText = sb.ToString(0, sb.Length - 1);
                        node.AppendChild(temp);
                    }
                    public void saveScanOptions(Graph graph) {
                        var scanNode = xmlData.SelectSingleNode(ROOT_CONFIG_TAG).AppendChild(xmlData.CreateElement(OVERVIEW_CONFIG_TAG));
                        var temp = xmlData.CreateElement(START_SCAN_CONFIG_TAG);
                        var ppl1 = graph.Displayed1Steps[0];
                        var ppl2 = graph.Displayed2Steps[0];
                        // TODO: check for data mismatch?
                        temp.InnerText = ppl1[0].X.ToString();
                        scanNode.AppendChild(temp);
                        temp = xmlData.CreateElement(END_SCAN_CONFIG_TAG);
                        // TODO: check for data mismatch?
                        temp.InnerText = ppl2[ppl2.Count - 1].X.ToString();
                        scanNode.AppendChild(temp);

                        var colNode = scanNode.AppendChild(xmlData.CreateElement(COL1_CONFIG_TAG));
                        savePointRows(ppl1, colNode);
                        colNode = scanNode.AppendChild(xmlData.CreateElement(COL2_CONFIG_TAG));
                        savePointRows(ppl2, colNode);
                    }
                    public void setTimeStamp(DateTime dt) {
                        var attr = xmlData.CreateAttribute(TIME_SPECTRUM_ATTRIBUTE);
                        attr.Value = dt.ToString("G", DateTimeFormatInfo.InvariantInfo);
                        xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, HEADER_CONFIG_TAG)).Attributes.Append(attr);
                    }
                    public void setShift(short? shift) {
                        var attr = xmlData.CreateAttribute(SHIFT_SPECTRUM_ATTRIBUTE);
                        attr.Value = shift == null ? "0" : shift.ToString();
                        xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, HEADER_CONFIG_TAG)).Attributes.Append(attr);
                    }
                }
                public class MainConfigWriter: ComplexWriter, IMainConfigWriter {
                    #region IAnyWriter Members
                    public override void write(params object[] data) {
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
                    void saveConnectOptions() {
                        string prefix = combine(ROOT_CONFIG_TAG, CONNECT_CONFIG_TAG);
                        fillInnerText(prefix, PORT_CONFIG_TAG, Port);
                        fillInnerText(prefix, BAUDRATE_CONFIG_TAG, BaudRate);
                        fillInnerText(prefix, TRY_NUMBER_CONFIG_TAG, Try);
                    }
                    void saveScanOptions() {
                        string prefix = combine(ROOT_CONFIG_TAG, OVERVIEW_CONFIG_TAG);
                        fillInnerText(prefix, START_SCAN_CONFIG_TAG, sPoint);
                        fillInnerText(prefix, END_SCAN_CONFIG_TAG, ePoint);
                    }
                    void saveCommonOptions() {
                        saveCommonOptions(CommonOptions);
                    }
                    void saveDelaysOptions() {
                        string prefix = combine(ROOT_CONFIG_TAG, COMMON_CONFIG_TAG);
                        fillInnerText(prefix, DELAY_BEFORE_MEASURE_CONFIG_TAG, CommonOptions.befTime);
                        fillInnerText(prefix, EQUAL_DELAYS_CONFIG_TAG, CommonOptions.ForwardTimeEqualsBeforeTime);
                        fillInnerText(prefix, DELAY_FORWARD_MEASURE_CONFIG_TAG, CommonOptions.fTime);
                        fillInnerText(prefix, DELAY_BACKWARD_MEASURE_CONFIG_TAG, CommonOptions.bTime);
                    }
                    void saveMassCoeffs() {
                        saveScalingCoeffs(Graph.MeasureGraph.Instance.Collectors.ConvertAll(col => col.Coeff).ToArray());
                    }
                    void saveCheckOptions() {
                        //checkpeak & iterations
                        string prefix = combine(ROOT_CONFIG_TAG, CHECK_CONFIG_TAG);
                        if (xmlData.SelectSingleNode(prefix) == null) {
                            var checkRegion = xmlData.CreateElement(CHECK_CONFIG_TAG);
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
                        fillInnerText(prefix, CHECK_ITER_NUMBER_CONFIG_TAG, Iterations);
                        fillInnerText(prefix, CHECK_TIME_LIMIT_CONFIG_TAG, TimeLimit);
                        fillInnerText(prefix, CHECK_MAX_SHIFT_CONFIG_TAG, AllowedShift);
                        fillInnerText(prefix, CHECK_PEAK_NUMBER_TAG, CheckerPeakIndex);

                        fillInnerText(prefix, BACKGROUND_CYCLES_NUMBER_TAG, BackgroundCycles);
                    }
                    void SavePreciseOptions() {
                        savePreciseData(preciseData, false);
                    }
                    protected override void clearOldValues() {
                        for (int i = 1; i <= Config.PEAK_NUMBER; ++i) {
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
            }
            abstract class CurrentTagHolder: TagHolder {
                const char COUNTS_SEPARATOR = ' ';
                const string CHECK_PEAK_NUMBER_TAG = "region";
                const string SOURCE_VOLTAGE_COEFF_CONFIG_TAG = "k";
                new const string CAPACITOR_VOLTAGE_COEFF_CONFIG_TAG = "c";
                const string DETECTOR_VOLTAGE_TAG = "dv";
                const string COL_CONFIG_TAG = "collector";
                const string COL_COEFF_CONFIG_TAG = "coeff";
                const string NUMBER_ATTRIBUTE = "n";
                const string COUNT_ATTRIBUTE = "count";
                public const string CONFIG_VERSION = "1.3";
                #region Current Readers
                public abstract class Reader: CurrentTagHolder {
                    public CommonOptions loadCommonOptions() {
                        XmlNode commonNode = xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, COMMON_CONFIG_TAG));
                        try {
                            ushort eT, iT, iV, eC, hC, fV1, fV2, CP;
                            double c, k;
                            eT = ushort.Parse(commonNode.SelectSingleNode(EXPOSITURE_TIME_CONFIG_TAG).InnerText);
                            //iT = ushort.Parse(commonNode.SelectSingleNode(TRANSITION_TIME_CONFIG_TAG).InnerText);
                            iV = ushort.Parse(commonNode.SelectSingleNode(IONIZATION_VOLTAGE_CONFIG_TAG).InnerText);

                            c = double.Parse(commonNode.SelectSingleNode(CAPACITOR_VOLTAGE_COEFF_CONFIG_TAG).InnerText, CultureInfo.InvariantCulture);
                            k = double.Parse(commonNode.SelectSingleNode(SOURCE_VOLTAGE_COEFF_CONFIG_TAG).InnerText, CultureInfo.InvariantCulture);
                            
                            eC = ushort.Parse(commonNode.SelectSingleNode(EMISSION_CURRENT_CONFIG_TAG).InnerText);
                            //hC = ushort.Parse(commonNode.SelectSingleNode(HEAT_CURRENT_CONFIG_TAG).InnerText);
                            fV1 = ushort.Parse(commonNode.SelectSingleNode(FOCUS_VOLTAGE1_CONFIG_TAG).InnerText);
                            fV2 = ushort.Parse(commonNode.SelectSingleNode(FOCUS_VOLTAGE2_CONFIG_TAG).InnerText);

                            ushort[] dVs = new ushort[3];
                            foreach (XmlNode node in commonNode.SelectNodes(DETECTOR_VOLTAGE_TAG)) {
                                var numberAttribute = node.Attributes[NUMBER_ATTRIBUTE];
                                int n = int.Parse(numberAttribute.Value);
                                ushort value = ushort.Parse(node.InnerText);
                                dVs[n - 1] = value;
                            }

                            {
                                CommonOptions opts = new CommonOptions();
                                opts.eTime = eT;
                                //opts.iTime = iT;
                                opts.iVoltage = iV;
                                //opts.CP = CP;
                                opts.C = c;
                                opts.K = k;

                                opts.d1V = dVs[0];
                                opts.d2V = dVs[1];
                                opts.d3V = dVs[2];

                                opts.eCurrent = eC;
                                //opts.hCurrent = hC;
                                opts.fV1 = fV1;
                                opts.fV2 = fV2;
                                loadDelays(commonNode, opts);
                                return opts;
                            }
                        } catch (NullReferenceException) {
                            throw new structureErrorOnLoadCommonData(filename);
                        }
                    }
                    public List<PreciseEditorData> loadPreciseData() {
                        try {
                            return LoadPED("");
                        } catch (ConfigLoadException) {
                            return null;
                        }
                    }
                    protected List<PreciseEditorData> LoadPED(string errorMessage) {
                        string prefix = combine(ROOT_CONFIG_TAG, SENSE_CONFIG_TAG);
                        var peds = new List<PreciseEditorData>();
                        for (int i = 1; i <= Config.PEAK_NUMBER; ++i) {
                            string peak, iter, width, col;
                            try {
                                var regionNode = xmlData.SelectSingleNode(combine(prefix, string.Format(PEAK_TAGS_FORMAT, i)));
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
                                        var temp = new PreciseEditorData(use, (byte)(i - 1), peakStep,
                                                                            byte.Parse(col), ushort.Parse(iter),
                                                                            ushort.Parse(width), (float)0, comment);
                                        peakStep -= peakWidth;
                                        peakWidth += peakWidth += peakStep;
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
                        peds.Sort();
                        return peds;
                    }
                    protected virtual PointPairListPlus readPeaks(XmlNode regionNode, ushort peakStep, ushort peakWidth) { return null; }
                    protected virtual void loadDelays(XmlNode commonNode, CommonOptions opts) { }
                }
                public class CommonOptionsReader: Reader, ICommonOptionsReader { }
                public class PreciseDataReader: Reader, IPreciseDataReader { }
                public abstract class ComplexReader: Reader, IScalingCoeffsReader {
                    public double[] loadScalingCoeffs() {
                        // TODO: class-dependent messages
                        try {
                            XmlNode interfaceNode = xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, INTERFACE_CONFIG_TAG));

                            double[] coeffs = { 2770 * 28, 896.5 * 18, 1 };
                            foreach (XmlNode node in interfaceNode.SelectNodes(COL_COEFF_CONFIG_TAG)) {
                                var numberAttribute = node.Attributes[NUMBER_ATTRIBUTE];
                                int n = int.Parse(numberAttribute.Value);
                                double value = double.Parse(node.InnerText, CultureInfo.InvariantCulture);
                                coeffs[n - 1] = value;
                            }
                            return coeffs;
                        } catch (NullReferenceException) {
                            throw new ConfigLoadException(CONFIG_FILE_STRUCTURE_ERROR, CONFIG_FILE_READ_ERROR, filename);
                        } catch (FormatException) {
                            throw new ConfigLoadException("Неверный формат данных", CONFIG_FILE_READ_ERROR, filename);
                        }
                    }
                }
                public class MainConfig: ComplexReader, IMainConfig {
                    #region IMainConfig implementation
                    public void read() {
                        string prefix;
                        try {
                            prefix = combine(ROOT_CONFIG_TAG, CONNECT_CONFIG_TAG);
                            Port = (xmlData.SelectSingleNode(combine(prefix, PORT_CONFIG_TAG)).InnerText);
                            BaudRate = int.Parse(xmlData.SelectSingleNode(combine(prefix, BAUDRATE_CONFIG_TAG)).InnerText);
                            Try = byte.Parse(xmlData.SelectSingleNode(combine(prefix, TRY_NUMBER_CONFIG_TAG)).InnerText);
                        } catch (NullReferenceException) {
                            (new ConfigLoadException(CONFIG_FILE_STRUCTURE_ERROR, CONFIG_FILE_READ_ERROR, filename)).visualise();
                            //use hard-coded defaults
                        }
                        try {
                            prefix = combine(ROOT_CONFIG_TAG, OVERVIEW_CONFIG_TAG);
                            sPoint = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, START_SCAN_CONFIG_TAG)).InnerText);
                            ePoint = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, END_SCAN_CONFIG_TAG)).InnerText);
                        } catch (NullReferenceException) {
                            (new ConfigLoadException(CONFIG_FILE_STRUCTURE_ERROR, CONFIG_FILE_READ_ERROR, filename)).visualise();
                            //use hard-coded defaults
                        }
                        try {
                            CommonOptions = loadCommonOptions();
                        } catch (ConfigLoadException cle) {
                            cle.visualise();
                            //use hard-coded defaults
                        }
                        try {
                            // init Graph.Instance after CommonOptions!
                            Graph.MeasureGraph.Instance.Collectors.RecomputeMassRows(loadScalingCoeffs());
                        } catch (ConfigLoadException) {
                            //cle.visualise();
                            //use hard-coded defaults
                        }
                        try {
                            var pedl = loadPreciseData();
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
                            reperPeak = new PreciseEditorData(false, 255, step, collector, 0, width, 0, "checker peak");
                        } catch (NullReferenceException) {
                            //use hard-coded defaults (null checker peak)
                        } catch (FormatException) {
                            // TODO: very bad..
                            //use hard-coded defaults (null checker peak)
                        }
                        try {
                            Iterations = int.Parse(xmlData.SelectSingleNode(combine(prefix, CHECK_ITER_NUMBER_CONFIG_TAG)).InnerText);
                        } catch (NullReferenceException) {
                            //use hard-coded defaults (infinite iterations)
                        }
                        try {
                            TimeLimit = int.Parse(xmlData.SelectSingleNode(combine(prefix, CHECK_TIME_LIMIT_CONFIG_TAG)).InnerText);
                        } catch (NullReferenceException) {
                            //use hard-coded defaults (no time limit)
                        }
                        try {
                            AllowedShift = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, CHECK_MAX_SHIFT_CONFIG_TAG)).InnerText);
                        } catch (NullReferenceException) {
                            //use hard-coded defaults (zero shift allowed)
                        }
                        try {
                            CheckerPeakIndex = int.Parse(xmlData.SelectSingleNode(combine(prefix, CHECK_PEAK_NUMBER_TAG)).InnerText);
                        } catch (NullReferenceException) {
                            //use hard-coded defaults (-1)
                        }
                        try {
                            BackgroundCycles = byte.Parse(xmlData.SelectSingleNode(combine(prefix, BACKGROUND_CYCLES_NUMBER_TAG)).InnerText);
                        } catch (NullReferenceException) {
                            //use hard-coded defaults (false)
                        }
                        // BAD: really uses previous values! (not default)
                    }
                    public XmlDocument XML {
                        get {
                            return xmlData;
                        }
                    }
                    #endregion
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
                public class LibraryReader: ILibraryReader {
                    const string LIBRARY_TAG = "library";
                    const string SPECTRUM_TAG = "spectrum";
                    const string ID_ATTRIBUTE = "id";
                    const string MASS_ATTRIBUTE = "mass";
                    const string PEAK_TAG = "peak";
                    const string VALUE_ATTRIBUTE = "value";
                    const string CALIBRATION_TIME_ATTRIBUTE = "ct";
                    readonly XmlTextReader reader;
                    System.Collections.Hashtable table;
                    public LibraryReader(string filename) {
                        reader = new XmlTextReader(filename);
                    }
                    #region ILibraryReader Members
                    public void readOnce(List<string> ids, List<string> loadedMasses) {
                        if (reader.ReadState != ReadState.Initial)
                            reader.ResetState();
                        reader.ReadToFollowing(LIBRARY_TAG);
                        reader.GetAttribute(VERSION_ATTRIBUTE);
                        System.Collections.Hashtable table = new System.Collections.Hashtable(ids.Count);
                        if (reader.ReadToDescendant(SPECTRUM_TAG)) {
                            do {
                                string id = reader.GetAttribute(ID_ATTRIBUTE);
                                // only 1 peak of a substance! otherwise will be dependent columns in the matrix
                                int index = ids.IndexOf(id);
                                if (index != -1) {
                                    if (loadedMasses[index] == "") {
                                        //error! mass should be stated explicitly!
                                    }
                                    System.Collections.Hashtable result = new System.Collections.Hashtable();
                                    if (reader.ReadToDescendant(PEAK_TAG)) {
                                        // TODO: use proper calibration data later in library
                                        string calibrationCoeffString;
                                        string ctString;
                                        string currentMass;
                                        do {
                                            currentMass = reader.GetAttribute(MASS_ATTRIBUTE);
                                            // only desired masses
                                            if (loadedMasses.Contains(currentMass)) {
                                                calibrationCoeffString = reader.GetAttribute(VALUE_ATTRIBUTE);
                                                ctString = reader.GetAttribute(CALIBRATION_TIME_ATTRIBUTE);
                                                if (calibrationCoeffString == null || ctString == null) {
                                                    //error!!! all peaks must have calibration for solving matrix
                                                } else try {
                                                    //actual calibration for current exposition time                                                    
                                                        double calibrationCoeff = Double.Parse(calibrationCoeffString) * Graph.MeasureGraph.Instance.CommonOptions.eTimeReal / Int32.Parse(ctString);
                                                    //check for integral value
                                                    Int32.Parse(currentMass);
                                                    result.Add(currentMass, calibrationCoeff);
                                                } catch (FormatException) {
                                                    //error!!! all peaks must have calibration for solving matrix
                                                };
                                            }
                                        } while (reader.ReadToNextSibling(PEAK_TAG));
                                    }
                                    table.Add(id, result);
                                }
                            } while (reader.ReadToNextSibling(SPECTRUM_TAG));
                        }
                        this.table = table;
                    }
                    public System.Collections.Hashtable Masses(string id) {
                        return table[id] as System.Collections.Hashtable;
                    }
                    #endregion
                }
                public class SpectrumReader: ComplexReader, ISpectrumReader {
                    bool hint;
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
                    public Graph.Displaying openSpectrumFile(out CommonOptions commonOpts, params PointPairListPlus[] points) {
                        //var pl1 = points[0];
                        //var pl2 = points[1];
                        try {
                            string prefix = combine(ROOT_CONFIG_TAG, OVERVIEW_CONFIG_TAG);
                            ushort start = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, START_SCAN_CONFIG_TAG)).InnerText);
                            ushort end = ushort.Parse(xmlData.SelectSingleNode(combine(prefix, END_SCAN_CONFIG_TAG)).InnerText);
                            foreach (XmlNode node in xmlData.SelectNodes(combine(prefix, COL_CONFIG_TAG))) {
                                int i = int.Parse(node.Attributes[NUMBER_ATTRIBUTE].Value) - 1;
                                points[i].AddRange(readPeaks(node, start, end));
                            }
                            //pl1.AddRange(readPeaks(xmlData.SelectSingleNode(combine(prefix, COL1_CONFIG_TAG)), start, end));
                            //pl2.AddRange(readPeaks(xmlData.SelectSingleNode(combine(prefix, COL2_CONFIG_TAG)), start, end));
                        } catch (NullReferenceException) {
                            throw new ConfigLoadException("Ошибка структуры файла", "Ошибка чтения файла спектра", filename);
                        } catch (FormatException) {
                            throw new ConfigLoadException("Неверный формат данных", "Ошибка чтения файла спектра", filename);
                        }
                        commonOpts = loadCommonOptions();

                        foreach (var list in points) {
                            list.Sort(ZedGraph.SortType.XValues);
                        }
                        //pl1.Sort(ZedGraph.SortType.XValues);
                        //pl2.Sort(ZedGraph.SortType.XValues);
                        return spectrumType();
                    }
                    public bool openPreciseSpectrumFile(PreciseSpectrum peds) {
                        peds.CommonOptions = loadCommonOptions();
                        try {
                            peds.AddRange(LoadPED("Ошибка чтения файла прецизионного спектра"));
                            return true;
                        } catch (ConfigLoadException) {
                            return false;
                        }
                    }
                    #endregion
                    bool OpenSpecterFile(out Graph graph) {
                        PointPairListPlus[] points = new PointPairListPlus[COLLECTOR_COEFFS.Length];
                        for (int i = 0; i < points.Length; ++i) {
                            points[i] = new PointPairListPlus();
                        }
                        CommonOptions commonOpts;
                        Graph.Displaying result = openSpectrumFile(out commonOpts, points);

                        graph = new Graph(commonOpts, loadScalingCoeffs());
                        switch (result) {
                            case Graph.Displaying.Measured:
                            // TODO: fix not loading timestamp here    
                            //graph.updateGraphAfterScanLoad(loadTimeStamp(), points);
                                graph.updateGraphAfterScanLoad(points);
                                return true;
                            case Graph.Displaying.Diff:
                                graph.updateGraphAfterScanDiff(false, points);
                                return true;
                            default:
                                return false;
                        }
                    }
                    bool OpenPreciseSpecterFile(out Graph graph) {
                        Graph.Displaying res = spectrumType();
                        PreciseSpectrum peds = new PreciseSpectrum();
                        bool result = openPreciseSpectrumFile(peds);
                        if (result) {
                            graph = new Graph(peds.CommonOptions, loadScalingCoeffs());
                            switch (res) {
                                case Graph.Displaying.Measured:
                                    short shift = short.MaxValue;
                                    try {
                                        shift = loadShift();
                                    } catch (Exception) {
                                        // do nothing
                                    }
                                    graph.updateGraphAfterPreciseLoad(peds, loadTimeStamp(), shift);
                                    return true;
                                case Graph.Displaying.Diff:
                                    graph.updateGraphAfterPreciseDiff(peds, false);
                                    return true;
                                default:
                                    return false;
                            }
                        } else {
                            //TODO: other solution!
                            graph = null;
                        }
                        return result;
                    }
                    Graph.Displaying spectrumType() {
                        XmlNode headerNode = xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, HEADER_CONFIG_TAG));
                        return (headerNode != null && headerNode.InnerText == DIFF_SPECTRUM_HEADER) ? Graph.Displaying.Diff : Graph.Displaying.Measured;
                    }
                    DateTime loadTimeStamp() {
                        return DateTime.Parse(getHeaderAttributeText(TIME_SPECTRUM_ATTRIBUTE), DateTimeFormatInfo.InvariantInfo);
                    }
                    short loadShift() {
                        return short.Parse(getHeaderAttributeText(SHIFT_SPECTRUM_ATTRIBUTE));
                    }
                    protected sealed override PointPairListPlus readPeaks(XmlNode regionNode, ushort peakStart, ushort peakEnd) {
                        PointPairListPlus tempPntLst = new PointPairListPlus();
                        try {
                            XmlNode temp = regionNode.SelectSingleNode(POINT_CONFIG_TAG);
                            if (temp == null)
                                return null;
                            string text = temp.InnerText;
                            string[] parts = text.Split(COUNTS_SEPARATOR);
                            foreach (string str in parts) {
                                // locale?
                                //! double for non-integral counts (after subtraction with renormalization on point/peak)
                                tempPntLst.Add(peakStart, double.Parse(str, CultureInfo.InvariantCulture));
                                ++peakStart;
                            }
                        } catch (FormatException) {
                            // TODO: store exception messages
                            throw new ConfigLoadException("Неверный формат данных", hint ? "Ошибка чтения файла спектра" : "Ошибка чтения файла прецизионного спектра", filename);
                        }
                        if (--peakStart != peakEnd)
                            throw new ConfigLoadException("Несовпадение рядов данных", hint ? "Ошибка чтения файла спектра" : "Ошибка чтения файла прецизионного спектра", filename);

                        return tempPntLst;
                    }
                }
                #endregion
                #region Current Writers
                public abstract class Writer: CurrentTagHolder {
                    public virtual void write(params object[] data) {
                        xmlData.Save(filename);
                    }
                    public void saveCommonOptions(CommonOptions opts) {
                        XmlNode commonNode = xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, COMMON_CONFIG_TAG));
                        saveCommonOptions(commonNode, opts);
                    }
                    [Obsolete]
                    public void saveCommonOptions(ushort eT, ushort iT, double iV, double cp, double eC, double hC, double fv1, double fv2) {
                        CommonOptions opts = new CommonOptions();
                        opts.eTimeReal = eT;
                        opts.iTimeReal = iT;
                        opts.iVoltageReal = iV;
                        opts.CPReal = cp;
                        opts.eCurrentReal = eC;
                        //opts.hCurrentReal = hC;
                        opts.fV1Real = fv1;
                        opts.fV2Real = fv2;
                        saveCommonOptions(opts);
                    }
                    void saveCommonOptions(XmlNode commonNode, CommonOptions opts) {
                        commonNode.SelectSingleNode(EXPOSITURE_TIME_CONFIG_TAG).InnerText = opts.eTime.ToString();
                        //commonNode.SelectSingleNode(TRANSITION_TIME_CONFIG_TAG).InnerText = opts.iTime.ToString();
                        commonNode.SelectSingleNode(IONIZATION_VOLTAGE_CONFIG_TAG).InnerText = opts.iVoltage.ToString();

                        var temp = commonNode.SelectSingleNode(CAPACITOR_VOLTAGE_COEFF_CONFIG_TAG);
                        if (temp == null)
                            commonNode.AppendChild(xmlData.CreateElement(CAPACITOR_VOLTAGE_COEFF_CONFIG_TAG));
                        temp = commonNode.SelectSingleNode(SOURCE_VOLTAGE_COEFF_CONFIG_TAG);
                        if (temp == null)
                            commonNode.AppendChild(xmlData.CreateElement(SOURCE_VOLTAGE_COEFF_CONFIG_TAG));

                        commonNode.SelectSingleNode(CAPACITOR_VOLTAGE_COEFF_CONFIG_TAG).InnerText = opts.C.ToString("R", CultureInfo.InvariantCulture);
                        commonNode.SelectSingleNode(SOURCE_VOLTAGE_COEFF_CONFIG_TAG).InnerText = opts.K.ToString("R", CultureInfo.InvariantCulture);

                        commonNode.SelectSingleNode(EMISSION_CURRENT_CONFIG_TAG).InnerText = opts.eCurrent.ToString();
                        //commonNode.SelectSingleNode(HEAT_CURRENT_CONFIG_TAG).InnerText = opts.hCurrent.ToString();
                        commonNode.SelectSingleNode(FOCUS_VOLTAGE1_CONFIG_TAG).InnerText = opts.fV1.ToString();
                        commonNode.SelectSingleNode(FOCUS_VOLTAGE2_CONFIG_TAG).InnerText = opts.fV2.ToString();

                        var nodes = commonNode.SelectNodes(DETECTOR_VOLTAGE_TAG);
                        {
                            bool missed = true;
                            foreach (XmlNode node in nodes) {
                                if (node.Attributes[NUMBER_ATTRIBUTE].Value == "1") {
                                    node.InnerText = opts.d1V.ToString();
                                    missed = false;
                                    break;
                                }
                            }
                            if (missed) {
                                var elem = xmlData.CreateElement(DETECTOR_VOLTAGE_TAG);
                                elem.SetAttribute(NUMBER_ATTRIBUTE, "1");
                                elem.InnerText = opts.d1V.ToString();
                                commonNode.AppendChild(elem);
                            }
                        }
                        {
                            bool missed = true;
                            foreach (XmlNode node in nodes) {
                                if (node.Attributes[NUMBER_ATTRIBUTE].Value == "2") {
                                    node.InnerText = opts.d2V.ToString();
                                    missed = false;
                                    break;
                                }
                            }
                            if (missed) {
                                var elem = xmlData.CreateElement(DETECTOR_VOLTAGE_TAG);
                                elem.SetAttribute(NUMBER_ATTRIBUTE, "2");
                                elem.InnerText = opts.d2V.ToString();
                                commonNode.AppendChild(elem);
                            }
                        }
                        {
                            bool missed = true;
                            foreach (XmlNode node in nodes) {
                                if (node.Attributes[NUMBER_ATTRIBUTE].Value == "3") {
                                    node.InnerText = opts.d3V.ToString();
                                    missed = false;
                                    break;
                                }
                            }
                            if (missed) {
                                var elem = xmlData.CreateElement(DETECTOR_VOLTAGE_TAG);
                                elem.SetAttribute(NUMBER_ATTRIBUTE, "3");
                                elem.InnerText = opts.d3V.ToString();
                                commonNode.AppendChild(elem);
                            }
                        }
                        /*commonNode.SelectSingleNode(DELAY_BEFORE_MEASURE_CONFIG_TAG).InnerText = Config.commonOpts.befTime.ToString();
                        commonNode.SelectSingleNode(EQUAL_DELAYS_CONFIG_TAG).InnerText = Config.commonOpts.ForwardTimeEqualsBeforeTime.ToString();
                        commonNode.SelectSingleNode(DELAY_FORWARD_MEASURE_CONFIG_TAG).InnerText = Config.commonOpts.fTime.ToString();
                        commonNode.SelectSingleNode(DELAY_BACKWARD_MEASURE_CONFIG_TAG).InnerText = Config.commonOpts.bTime.ToString();*/
                    }
                    public void savePreciseData(List<PreciseEditorData> peds, bool savePeakSum) {
                        clearOldValues();
                        foreach (var ped in peds) {
                            var regionNode = xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, SENSE_CONFIG_TAG, string.Format(PEAK_TAGS_FORMAT, ped.pNumber + 1)));
                            regionNode.SelectSingleNode(PEAK_NUMBER_CONFIG_TAG).InnerText = ped.Step.ToString();
                            regionNode.SelectSingleNode(PEAK_ITER_NUMBER_CONFIG_TAG).InnerText = ped.Iterations.ToString();
                            regionNode.SelectSingleNode(PEAK_WIDTH_CONFIG_TAG).InnerText = ped.Width.ToString();
                            regionNode.SelectSingleNode(PEAK_PRECISION_CONFIG_TAG).InnerText = ped.Precision.ToString("R", CultureInfo.InvariantCulture);
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
                    protected virtual void clearOldValues() { }
                    protected virtual void savePointRows(PointPairListPlus row, XmlNode node) { }
                }
                public class CommonOptionsWriter: Writer, ICommonOptionsWriter { }
                public class PreciseDataWriter: Writer, IPreciseDataWriter { }
                public abstract class ComplexWriter: Writer, IScalingCoeffsWriter {
                    public void saveScalingCoeffs(params double[] coeffs) {
                        clearInnerText(ROOT_CONFIG_TAG, INTERFACE_CONFIG_TAG);
                        string prefix = combine(ROOT_CONFIG_TAG, INTERFACE_CONFIG_TAG);
                        //fillInnerText(prefix, C1_CONFIG_TAG, coeff1.ToString("R", CultureInfo.InvariantCulture));
                        //fillInnerText(prefix, C2_CONFIG_TAG, coeff2.ToString("R", CultureInfo.InvariantCulture));

                        var interfaceNode = xmlData.SelectSingleNode(prefix);
                        var nodes = interfaceNode.SelectNodes(COL_COEFF_CONFIG_TAG);
                        {
                            bool missed = true;
                            foreach (XmlNode node in nodes) {
                                if (node.Attributes[NUMBER_ATTRIBUTE].Value == "1") {
                                    node.InnerText = coeffs[0].ToString("R", CultureInfo.InvariantCulture);
                                    missed = false;
                                    break;
                                }
                            }
                            if (missed) {
                                var elem = xmlData.CreateElement(COL_COEFF_CONFIG_TAG);
                                elem.SetAttribute(NUMBER_ATTRIBUTE, "1");
                                elem.InnerText = coeffs[0].ToString("R", CultureInfo.InvariantCulture);
                                interfaceNode.AppendChild(elem);
                            }
                        }
                        {
                            bool missed = true;
                            foreach (XmlNode node in nodes) {
                                if (node.Attributes[NUMBER_ATTRIBUTE].Value == "2") {
                                    node.InnerText = coeffs[1].ToString("R", CultureInfo.InvariantCulture);
                                    missed = false;
                                    break;
                                }
                            }
                            if (missed) {
                                var elem = xmlData.CreateElement(COL_COEFF_CONFIG_TAG);
                                elem.SetAttribute(NUMBER_ATTRIBUTE, "2");
                                elem.InnerText = coeffs[1].ToString("R", CultureInfo.InvariantCulture);
                                interfaceNode.AppendChild(elem);
                            }
                        }
                        {
                            bool missed = true;
                            foreach (XmlNode node in nodes) {
                                if (node.Attributes[NUMBER_ATTRIBUTE].Value == "3") {
                                    node.InnerText = coeffs[2].ToString("R", CultureInfo.InvariantCulture);
                                    missed = false;
                                    break;
                                }
                            }
                            if (missed) {
                                var elem = xmlData.CreateElement(COL_COEFF_CONFIG_TAG);
                                elem.SetAttribute(NUMBER_ATTRIBUTE, "3");
                                elem.InnerText = coeffs[2].ToString("R", CultureInfo.InvariantCulture);
                                interfaceNode.AppendChild(elem);
                            }
                        }
                    }
                }
                public class SpectrumWriter: ComplexWriter, ISpectrumWriter {
                    protected sealed override void savePointRows(PointPairListPlus row, XmlNode node) {
                        if (row.Count == 0)
                            return;
                        StringBuilder sb = new StringBuilder();
                        foreach (ZedGraph.PointPair pp in row) {
                            sb.Append(pp.Y);
                            sb.Append(COUNTS_SEPARATOR);
                        }
                        XmlNode temp = xmlData.CreateElement(POINT_CONFIG_TAG); ;
                        temp.InnerText = sb.ToString(0, sb.Length - 1);
                        node.AppendChild(temp);
                    }
                    public void saveScanOptions(Graph graph) {
                        XmlNode scanNode = xmlData.SelectSingleNode(ROOT_CONFIG_TAG).AppendChild(xmlData.CreateElement(OVERVIEW_CONFIG_TAG));
                        XmlElement temp = xmlData.CreateElement(START_SCAN_CONFIG_TAG);
                        //PointPairListPlus ppl1 = graph.Displayed1Steps[0];
                        //PointPairListPlus ppl2 = graph.Displayed2Steps[0];
                        var steps = graph.Collectors[0][0].Step;
                        temp.InnerText = steps[0].X.ToString();// actually integral type
                        // TODO: check for data mismatch?
                        //temp.InnerText = ppl1[0].X.ToString();
                        scanNode.AppendChild(temp);
                        temp = xmlData.CreateElement(END_SCAN_CONFIG_TAG);
                        // TODO: check for data mismatch?
                        temp.InnerText = steps[steps.Count - 1].X.ToString();// actually integral type
                        //temp.InnerText = ppl2[ppl2.Count - 1].X.ToString();
                        scanNode.AppendChild(temp);

                        for (int i = 0; i < graph.Collectors.Count; ++i) {
                            var collector = graph.Collectors[i];
                            var colNode = scanNode.AppendChild(xmlData.CreateElement(COL_CONFIG_TAG));
                            var numberAttribute = xmlData.CreateAttribute(NUMBER_ATTRIBUTE);
                            numberAttribute.Value = (i + 1).ToString();
                            colNode.Attributes.Append(numberAttribute);
                            savePointRows(collector[0].Step, colNode);
                        }
                        //XmlNode colNode = scanNode.AppendChild(xmlData.CreateElement(COL1_CONFIG_TAG));
                        //savePointRows(ppl1, colNode);
                        //colNode = scanNode.AppendChild(xmlData.CreateElement(COL2_CONFIG_TAG));
                        //savePointRows(ppl2, colNode);
                    }
                    public void setTimeStamp(DateTime dt) {
                        XmlAttribute attr = xmlData.CreateAttribute(TIME_SPECTRUM_ATTRIBUTE);
                        attr.Value = dt.ToString("G", DateTimeFormatInfo.InvariantInfo);
                        xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, HEADER_CONFIG_TAG)).Attributes.Append(attr);
                    }
                    public void setShift(short? shift) {
                        XmlAttribute attr = xmlData.CreateAttribute(SHIFT_SPECTRUM_ATTRIBUTE);
                        attr.Value = shift == null ? "0" : shift.ToString();
                        xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, HEADER_CONFIG_TAG)).Attributes.Append(attr);
                    }
                }
                public class MainConfigWriter: ComplexWriter, IMainConfigWriter {
                    #region IAnyWriter Members
                    public override void write(params object[] data) {
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
                    void saveConnectOptions() {
                        string prefix = combine(ROOT_CONFIG_TAG, CONNECT_CONFIG_TAG);
                        fillInnerText(prefix, PORT_CONFIG_TAG, Port);
                        fillInnerText(prefix, BAUDRATE_CONFIG_TAG, BaudRate);
                        fillInnerText(prefix, TRY_NUMBER_CONFIG_TAG, Try);
                    }
                    void saveScanOptions() {
                        string prefix = combine(ROOT_CONFIG_TAG, OVERVIEW_CONFIG_TAG);
                        fillInnerText(prefix, START_SCAN_CONFIG_TAG, sPoint);
                        fillInnerText(prefix, END_SCAN_CONFIG_TAG, ePoint);
                    }
                    void saveCommonOptions() {
                        saveCommonOptions(CommonOptions);
                    }
                    void saveDelaysOptions() {
                        string prefix = combine(ROOT_CONFIG_TAG, COMMON_CONFIG_TAG);
                        fillInnerText(prefix, DELAY_BEFORE_MEASURE_CONFIG_TAG, CommonOptions.befTime);
                        fillInnerText(prefix, EQUAL_DELAYS_CONFIG_TAG, CommonOptions.ForwardTimeEqualsBeforeTime);
                        fillInnerText(prefix, DELAY_FORWARD_MEASURE_CONFIG_TAG, CommonOptions.fTime);
                        fillInnerText(prefix, DELAY_BACKWARD_MEASURE_CONFIG_TAG, CommonOptions.bTime);
                    }
                    void saveMassCoeffs() {
                        saveScalingCoeffs(Graph.MeasureGraph.Instance.Collectors.ConvertAll(col => col.Coeff).ToArray());
                    }
                    void saveCheckOptions() {
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
                        fillInnerText(prefix, CHECK_ITER_NUMBER_CONFIG_TAG, Iterations);
                        fillInnerText(prefix, CHECK_TIME_LIMIT_CONFIG_TAG, TimeLimit);
                        fillInnerText(prefix, CHECK_MAX_SHIFT_CONFIG_TAG, AllowedShift);
                        fillInnerText(prefix, CHECK_PEAK_NUMBER_TAG, CheckerPeakIndex);

                        fillInnerText(prefix, BACKGROUND_CYCLES_NUMBER_TAG, BackgroundCycles);
                    }
                    void SavePreciseOptions() {
                        savePreciseData(preciseData, false);
                    }
                    protected override void clearOldValues() {
                        for (int i = 1; i <= Config.PEAK_NUMBER; ++i) {
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
            }
            #region Static Getters
            public static ISpectrumReader getSpectrumReader(string confName, bool hint) {
                ISpectrumReader reader = findCorrespondingReaderVersion<ISpectrumReader,
                    CurrentTagHolder.SpectrumReader,
                    Version1_2TagHolder.SpectrumReader>(confName, "Ошибка чтения файла спектра");
                reader.Hint = hint;
                return reader;
            }
            public static ISpectrumWriter getSpectrumWriter(string confName, Graph graph) {
                XmlDocument doc = new XmlDocument();
                string header = graph.DisplayingMode == Graph.Displaying.Diff ? DIFF_SPECTRUM_HEADER : MEASURED_SPECTRUM_HEADER;
                XmlNode rootNode = createRootStub(doc, header);

                ISpectrumWriter writer = getInitializedConfig<ISpectrumWriter, CurrentTagHolder.SpectrumWriter>(confName, doc);
                if (graph.isPreciseSpectrum)
                    createPEDStub(doc, rootNode);
                else
                    writer.saveScanOptions(graph);

                if (graph.CommonOptions != null) {
                    // TODO: do not allow saving! data lack for properly save in new formats
                    createCommonOptsStub(doc, rootNode);
                    writer.saveCommonOptions(graph.CommonOptions);
                }
                writer.saveScalingCoeffs(graph.Collectors.ConvertAll(col => col.Coeff).ToArray());
                return writer;
            }
            public static ICommonOptionsReader getCommonOptionsReader(string confName) {
                return findCorrespondingReaderVersion<ICommonOptionsReader,
                    CurrentTagHolder.CommonOptionsReader,
                    Version1_2TagHolder.CommonOptionsReader>(confName, "Ошибка чтения файла общих настроек");
            }
            public static IPreciseDataReader getPreciseDataReader(string confName) {
                return findCorrespondingReaderVersion<IPreciseDataReader,
                    CurrentTagHolder.PreciseDataReader,
                    Version1_2TagHolder.PreciseDataReader>(confName, "Ошибка чтения файла прецизионных точек");
            }
            public static IMainConfig getMainConfig(string confName) {
                return findCorrespondingReaderVersion<IMainConfig,
                    CurrentTagHolder.MainConfig,
                    Version1_2TagHolder.MainConfig>(confName, CONFIG_FILE_READ_ERROR);
            }
            public static ICommonOptionsWriter getCommonOptionsWriter(string confName) {
                XmlDocument doc = new XmlDocument();
                createCommonOptsStub(doc, createRootStub(doc, COMMON_OPTIONS_HEADER));
                return getInitializedConfig<ICommonOptionsWriter, CurrentTagHolder.CommonOptionsWriter>(confName, doc);
            }
            public static IPreciseDataWriter getPreciseDataWriter(string confName) {
                XmlDocument doc = new XmlDocument();
                createPEDStub(doc, createRootStub(doc, PRECISE_OPTIONS_HEADER));
                return getInitializedConfig<IPreciseDataWriter, CurrentTagHolder.PreciseDataWriter>(confName, doc);
            }
            public static IMainConfigWriter getMainConfigWriter(string confName, XmlDocument doc) {
                return getInitializedConfig<IMainConfigWriter, CurrentTagHolder.MainConfigWriter>(confName, doc);
            }
            internal static ILibraryReader getLibraryReader(string confName) {
                return new CurrentTagHolder.LibraryReader(confName);
                /*return findCorrespondingReaderVersion<ILibraryReader,
                    CurrentTagHolder.LibraryReader,
                    CurrentTagHolder.LibraryReader,
                    CurrentTagHolder.LibraryReader,
                    CurrentTagHolder.LibraryReader>(confName, LIBRARY_FILE_READ_ERROR);*/
            }
            #endregion
            #region Private Service Methods
            static RETURN_INTERFACE findCorrespondingReaderVersion<RETURN_INTERFACE, CURRENT_TYPE, TYPE0>(string filename, string errorMessage)
                where RETURN_INTERFACE: IAnyReader
                where CURRENT_TYPE: TagHolder, RETURN_INTERFACE, new()
                where TYPE0: TagHolder, RETURN_INTERFACE, new() {
                XmlDocument doc = new XmlDocument();
                try {
                    doc.Load(filename);
                } catch (Exception Error) {
                    throw new ConfigLoadException(Error.Message, errorMessage, filename);
                }
                XmlNode rootNode = doc.SelectSingleNode(ROOT_CONFIG_TAG);
                if (rootNode == null) {
                    throw new ConfigLoadException("Root node is absent", errorMessage, filename);
                }
                XmlNode version = rootNode.Attributes.GetNamedItem(VERSION_ATTRIBUTE);
                if (version == null) {
                    throw new ConfigLoadException("No config version", errorMessage, filename);
                }
                string versionText = version.Value;
                switch (versionText) {
                    case Version1_2TagHolder.CONFIG_VERSION:
                        return getInitializedConfig<RETURN_INTERFACE, TYPE0>(filename, doc);
                    case CurrentTagHolder.CONFIG_VERSION:
                        return getInitializedConfig<RETURN_INTERFACE, CURRENT_TYPE>(filename, doc);
                    default:
                        throw new ConfigLoadException("Unknown version", errorMessage, filename);
                }
            }
            static RETURN_INTERFACE getInitializedConfig<RETURN_INTERFACE, TYPE>(string filename, XmlDocument doc)                
                where RETURN_INTERFACE: IAnyConfig
                where TYPE: TagHolder, RETURN_INTERFACE, new() {
                RETURN_INTERFACE config = new TYPE();
                (config as TagHolder).initialize(filename, doc);
                return config;
            }
            static string combine(params string[] args) {
                return string.Join("/", args);
            }
            void clearInnerText(string prefix, string nodeName) {
                fillInnerText(prefix, nodeName, "");
            }
            void fillInnerText(string prefix, string nodeName, object value) {
                string fullName = combine(prefix, nodeName);
                try {
                    xmlData.SelectSingleNode(fullName).InnerText = value.ToString();
                } catch (NullReferenceException) {
                    xmlData.SelectSingleNode(prefix).AppendChild(xmlData.CreateElement(nodeName));
                    xmlData.SelectSingleNode(fullName).InnerText = value.ToString();
                }
            }
            string getHeaderAttributeText(string tag) {
                XmlNode headerNode = xmlData.SelectSingleNode(combine(ROOT_CONFIG_TAG, HEADER_CONFIG_TAG));
                XmlAttributeCollection attrs = headerNode.Attributes;
                XmlNode attr = attrs.GetNamedItem(tag);
                return attr.InnerText;
            }
            // static below!
            static XmlNode createRootStub(XmlDocument conf, string header) {
                conf.AppendChild(conf.CreateXmlDeclaration("1.0", "utf-8", ""));
                XmlElement rootNode = conf.CreateElement(ROOT_CONFIG_TAG);
                conf.AppendChild(rootNode);

                XmlAttribute attr = conf.CreateAttribute(VERSION_ATTRIBUTE);
                attr.Value = CurrentTagHolder.CONFIG_VERSION;
                rootNode.Attributes.Append(attr);

                XmlElement headerNode = conf.CreateElement(HEADER_CONFIG_TAG);
                headerNode.InnerText = header;
                rootNode.AppendChild(headerNode);

                return rootNode;
            }
            // TODO: move to proper version!
            static XmlNode createCommonOptsStub(XmlDocument conf, XmlNode mountPoint) {
                XmlNode commonNode = conf.CreateElement(COMMON_CONFIG_TAG);
                commonNode.AppendChild(conf.CreateElement(EXPOSITURE_TIME_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(TRANSITION_TIME_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(IONIZATION_VOLTAGE_CONFIG_TAG));
                //commonNode.AppendChild(conf.CreateElement(CAPACITOR_VOLTAGE_COEFF_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(EMISSION_CURRENT_CONFIG_TAG));
                //commonNode.AppendChild(conf.CreateElement(HEAT_CURRENT_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(FOCUS_VOLTAGE1_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(FOCUS_VOLTAGE2_CONFIG_TAG));

                /*commonNode.AppendChild(conf.CreateElement(DELAY_BEFORE_MEASURE_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(EQUAL_DELAYS_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(DELAY_FORWARD_MEASURE_CONFIG_TAG));
                commonNode.AppendChild(conf.CreateElement(DELAY_BACKWARD_MEASURE_CONFIG_TAG));*/
                mountPoint.AppendChild(commonNode);
                return commonNode;
            }
            static XmlNode createPEDStub(XmlDocument pedConf, XmlNode mountPoint) {
                XmlNode senseNode = pedConf.CreateElement(SENSE_CONFIG_TAG);

                for (int i = 1; i <= Config.PEAK_NUMBER; ++i) {
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