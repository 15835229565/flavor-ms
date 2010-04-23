using System;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;

namespace Flavor.Common {
    static class Config {
        private static XmlDocument _conf = new XmlDocument();
        private static string mainConfigPrefix = string.Format("/{0}/", ROOT_CONFIG_TAG);

        private static string initialDir;
        private static string confName;
        private static string logName;

        internal const string SPECTRUM_EXT = "sdf";
        internal const string PRECISE_SPECTRUM_EXT = "psf";
        internal const string MONITOR_SPECTRUM_EXT = "mon";

        internal const string MONITOR_SPECTRUM_HEADER = "Monitor";
        internal const string PRECISE_OPTIONS_HEADER = "Precise options";

        internal const string ROOT_CONFIG_TAG = "control";

        internal const string TIME_SPECTRUM_ATTRIBUTE = "time";
        internal const string SHIFT_SPECTRUM_ATTRIBUTE = "shift";

        private static string SerialPort = "COM1";
        private static ushort SerialBaudRate = 38400;
        private static byte SendTry = 1;

        internal const ushort MIN_STEP = 0;
        internal const ushort MAX_STEP = 1056;
        private static ushort startPoint = MIN_STEP;
        private static ushort endPoint = MAX_STEP;

        private static CommonOptions commonOpts = new CommonOptions();
        internal static CommonOptions CommonOptions {
            get { return commonOpts; }
        }

        private static List<Utility.PreciseEditorData> preciseData = new List<Utility.PreciseEditorData>();
        private static List<Utility.PreciseEditorData> preciseDataLoaded = new List<Utility.PreciseEditorData>();
        private static List<Utility.PreciseEditorData> preciseDataDiff = new List<Utility.PreciseEditorData>();

        internal static List<Utility.PreciseEditorData> PreciseData {
            get { return preciseData; }
            //set { preciseData = value; }
        }
        internal static List<Utility.PreciseEditorData> PreciseDataLoaded {
            get { return preciseDataLoaded; }
            //set { preciseData = value; }
        }
        internal static List<Utility.PreciseEditorData> PreciseDataDiff {
            get { return preciseDataDiff; }
            //set { preciseData = value; }
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
            /*set {
                if (value < 0) {
                    iterations = 0;
                    return;
                }
                iterations = value;
            }*/
        }
        private static int timeLimit = 0;
        internal static int TimeLimit {
            get { return timeLimit; }
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
            get { return SendTry; }
            set { SendTry = value; }
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
            initialDir = System.IO.Directory.GetCurrentDirectory();
            confName = System.IO.Path.Combine(initialDir, "config.xml");
            logName = System.IO.Path.Combine(initialDir, "MScrash.log");
        }

        private static void fillInnerText(string prefix, string nodeName, object value) {
            try {
                _conf.SelectSingleNode(prefix + "/" + nodeName).InnerText = value.ToString();
            } catch (NullReferenceException) {
                _conf.SelectSingleNode(prefix).AppendChild(_conf.CreateNode(XmlNodeType.Element, nodeName, ""));
                _conf.SelectSingleNode(prefix + "/" + nodeName).InnerText = value.ToString();
            }
        }

        internal static void loadConfig() {
            try {
                _conf.Load(confName);
            } catch (Exception Error) {
                throw new ConfigLoadException(Error.Message, "Ошибка чтения конфигурационного файла", confName);
            }
            try {
                SerialPort = (_conf.SelectSingleNode(string.Format("/{0}/connect/port", ROOT_CONFIG_TAG)).InnerText);
                SerialBaudRate = ushort.Parse(_conf.SelectSingleNode(string.Format("/{0}/connect/baudrate", ROOT_CONFIG_TAG)).InnerText);
                SendTry = byte.Parse(_conf.SelectSingleNode(string.Format("/{0}/connect/try", ROOT_CONFIG_TAG)).InnerText);
            } catch (NullReferenceException) {
                (new ConfigLoadException("Ошибка структуры конфигурационного файла", "Ошибка чтения конфигурационного файла", confName)).visualise();
                //use hard-coded defaults
            }
            try {
                sPoint = ushort.Parse(_conf.SelectSingleNode(string.Format("/{0}/overview/start", ROOT_CONFIG_TAG)).InnerText);
                ePoint = ushort.Parse(_conf.SelectSingleNode(string.Format("/{0}/overview/end", ROOT_CONFIG_TAG)).InnerText);
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
            try {
                ushort step = ushort.Parse(_conf.SelectSingleNode(string.Format("/{0}/check/peak", ROOT_CONFIG_TAG)).InnerText);
                byte collector = byte.Parse(_conf.SelectSingleNode(string.Format("/{0}/check/col", ROOT_CONFIG_TAG)).InnerText);
                ushort width = ushort.Parse(_conf.SelectSingleNode(string.Format("/{0}/check/width", ROOT_CONFIG_TAG)).InnerText);
                reperPeak = new Utility.PreciseEditorData(false, 255, step, collector, 0, width, 0, "checker peak");
            } catch (NullReferenceException) {
                //use hard-coded defaults (null checker peak)
            } catch (FormatException) {
                // TODO: very bad..
                //use hard-coded defaults (null checker peak)
            }
            try {
                iterations = int.Parse(_conf.SelectSingleNode(string.Format("/{0}/check/iterations", ROOT_CONFIG_TAG)).InnerText);
            } catch (NullReferenceException) {
                //use hard-coded defaults (infinite iterations)
            }
            try {
                timeLimit = int.Parse(_conf.SelectSingleNode(string.Format("/{0}/check/limit", ROOT_CONFIG_TAG)).InnerText);
            } catch (NullReferenceException) {
                //use hard-coded defaults (no time limit)
            }
        }

        private static void saveScanOptions() {
            fillInnerText("/control/overview", "start", sPoint);
            fillInnerText("/control/overview", "end", ePoint);
            _conf.Save(@confName);
        }
        internal static void saveScanOptions(ushort sPointReal, ushort ePointReal) {
            Config.sPoint = sPointReal;//!!!
            Config.ePoint = ePointReal;//!!!
            Config.saveScanOptions();
        }

        private static void saveDelaysOptions() {
            fillInnerText("/control/common", "before", commonOpts.befTime);
            fillInnerText("/control/common", "equal", commonOpts.ForwardTimeEqualsBeforeTime);
            fillInnerText("/control/common", "forward", commonOpts.fTime);
            fillInnerText("/control/common", "back", commonOpts.bTime);
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
            fillInnerText("/control", "interface", "");
            fillInnerText("/control/interface", "coeff1", col1Coeff.ToString("R"));
            fillInnerText("/control/interface", "coeff2", col2Coeff.ToString("R"));
            _conf.Save(@confName);
        }

        private static void saveConnectOptions() {
            fillInnerText("/control/connect", "port", Port);
            fillInnerText("/control/connect", "baudrate", BaudRate);
            _conf.Save(@confName);
        }
        internal static void saveConnectOptions(string port, ushort baudrate) {
            Config.Port = port;
            Config.BaudRate = baudrate;
            Config.saveConnectOptions();
        }

        private static void saveCheckOptions() {
            //checkpeak & iterations
            if (_conf.SelectSingleNode("/control/check") == null) {
                XmlNode checkRegion = _conf.CreateNode(XmlNodeType.Element, "check", "");
                _conf.SelectSingleNode("/control").AppendChild(checkRegion);
            }
            if (reperPeak != null) {
                fillInnerText("/control/check", "peak", reperPeak.Step);
                fillInnerText("/control/check", "col", reperPeak.Collector);
                fillInnerText("/control/check", "width", reperPeak.Width);
            } else {
                fillInnerText("/control/check", "peak", "");
                fillInnerText("/control/check", "col", "");
                fillInnerText("/control/check", "width", "");
            }
            fillInnerText("/control/check", "iterations", iterations);
            fillInnerText("/control/check", "limit", timeLimit);
            _conf.Save(@confName);
        }
        internal static void saveCheckOptions(int iter, int timeLim, Utility.PreciseEditorData peak) {
            iterations = iter;
            timeLimit = timeLim;
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

        internal static bool openSpectrumFile(string filename, bool hint) {
            bool result;
            ConfigLoadException resultException = null;
            try {
                result = hint ? OpenSpecterFile(filename) : OpenPreciseSpecterFile(filename);
            } catch (ConfigLoadException cle) {
                resultException = cle;
                result = false;
            }
            if (result)
                return hint;
            try {
                result = (!hint) ? OpenSpecterFile(filename) : OpenPreciseSpecterFile(filename);
            } catch (ConfigLoadException cle) {
                resultException = (resultException == null) ? cle : resultException;
                result = false;
            }
            if (result)
                return (!hint);
            throw resultException;
        }

        private static string genAutoSaveFilename(string extension, DateTime now) {
            string dirname;
            dirname = System.IO.Path.Combine(initialDir, string.Format("{0}-{1}-{2}", now.Year, now.Month, now.Day));
            if (!System.IO.Directory.Exists(@dirname)) {
                System.IO.Directory.CreateDirectory(@dirname);
            }
            return System.IO.Path.Combine(dirname, string.Format("{0}-{1}-{2}-{3}.", now.Hour, now.Minute, now.Second, now.Millisecond) + extension);
        }

        private static bool OpenSpecterFile(string p) {
            PointPairListPlus pl1 = new PointPairListPlus(), pl2 = new PointPairListPlus();
            Graph.Displaying result = OpenSpecterFile(p, pl1, pl2);

            switch (result) {
                case Graph.Displaying.Measured:
                    Graph.ResetLoadedPointLists();
                    Graph.updateLoaded(pl1, pl2);
                    return true;
                case Graph.Displaying.Diff:
                    Graph.ResetDiffPointLists();
                    Graph.updateGraphAfterScanDiff(pl1, pl2);
                    return true;
                default:
                    return false;
            }
        }
        private static Graph.Displaying OpenSpecterFile(string filename, PointPairListPlus pl1, PointPairListPlus pl2) {
            XmlDocument sf = new XmlDocument();
            XmlNode headerNode = null;
            Graph.Displaying spectrumType = Graph.Displaying.Measured;
            try {
                sf.Load(filename);
            } catch (Exception Error) {
                throw new ConfigLoadException(Error.Message, "Ошибка чтения файла спектра", filename);
            }
            string prefix = "";
            if (sf.SelectSingleNode("control/overview") != null) {
                prefix = mainConfigPrefix;
                headerNode = sf.SelectSingleNode("control/header");
            } else if (sf.SelectSingleNode("overview") != null) {
                headerNode = sf.SelectSingleNode("overview/header");
            } else {
                throw new ConfigLoadException("Ошибка структуры файла", "Ошибка чтения файла спектра", filename);
            }
            if (headerNode != null && headerNode.InnerText == "Diff")
                spectrumType = Graph.Displaying.Diff;

            ushort X = 0;
            long Y = 0;
            try {
                foreach (XmlNode pntNode in sf.SelectNodes(prefix + "overview/collector1/p")) {
                    X = ushort.Parse(pntNode.SelectSingleNode("s").InnerText);
                    Y = long.Parse(pntNode.SelectSingleNode("c").InnerText);
                    pl1.Add(X, Y);
                }
                foreach (XmlNode pntNode in sf.SelectNodes(prefix + "overview/collector2/p")) {
                    X = ushort.Parse(pntNode.SelectSingleNode("s").InnerText);
                    Y = long.Parse(pntNode.SelectSingleNode("c").InnerText);
                    pl2.Add(X, Y);
                }
            } catch (NullReferenceException) {
                throw new ConfigLoadException("Ошибка структуры файла", "Ошибка чтения файла спектра", filename);
            }
            //the whole logic of displaying spertra must be modified
            //!!!!!!!!!!!!!!!!!!!!!!!!
            CommonOptions co = null;
            try {
                // what version of function will be called here?
                loadCommonOptions(filename, sf, out co);
            } catch (structureErrorOnLoadCommonData) {
                co = null;
            }
            //!!!!!!!!!!!!!!!!!!!!!!!!
            pl1.Sort(ZedGraph.SortType.XValues);
            pl2.Sort(ZedGraph.SortType.XValues);
            return spectrumType;
        }
        internal static XmlDocument SaveSpecterFile(string p, Graph.Displaying displayMode) {
            XmlDocument sf = new XmlDocument();
            XmlNode temp;
            XmlNode scanNode = createRootStub(sf, "").AppendChild(sf.CreateNode(XmlNodeType.Element, "overview", ""));
            switch (displayMode) {
                case Graph.Displaying.Loaded:
                    sf.SelectSingleNode("/control/header").InnerText = "Measure";
                    break;
                case Graph.Displaying.Measured:
                    sf.SelectSingleNode("/control/header").InnerText = "Measure";
                    scanNode.AppendChild(sf.CreateNode(XmlNodeType.Element, "start", ""));
                    scanNode.AppendChild(sf.CreateNode(XmlNodeType.Element, "end", ""));
                    scanNode.SelectSingleNode("start").InnerText = sPoint.ToString();
                    scanNode.SelectSingleNode("end").InnerText = ePoint.ToString();
                    // In case of loaded (not auto) start/end points and measure parameters are not connected to spectrum data..
                    break;
                case Graph.Displaying.Diff:
                    sf.SelectSingleNode("/control/header").InnerText = "Diff";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            scanNode.AppendChild(sf.CreateNode(XmlNodeType.Element, "collector1", ""));
            scanNode.AppendChild(sf.CreateNode(XmlNodeType.Element, "collector2", ""));
            foreach (ZedGraph.PointPair pp in Graph.Displayed1Steps[0]) {
                temp = sf.CreateNode(XmlNodeType.Element, "p", "");
                temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "s", "")).InnerText = pp.X.ToString();
                temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "c", "")).InnerText = ((long)(pp.Y)).ToString();
                scanNode.SelectSingleNode("collector1").AppendChild(temp);
            }
            foreach (ZedGraph.PointPair pp in Graph.Displayed2Steps[0]) {
                temp = sf.CreateNode(XmlNodeType.Element, "p", "");
                temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "s", "")).InnerText = pp.X.ToString();
                temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "c", "")).InnerText = ((long)(pp.Y)).ToString();
                scanNode.SelectSingleNode("collector2").AppendChild(temp);
            }
            sf.Save(@p);
            return sf;
        }
        internal static void AutoSaveSpecterFile() {
            DateTime dt = System.DateTime.Now;
            string filename = genAutoSaveFilename(SPECTRUM_EXT, dt);
            XmlDocument file = SaveSpecterFile(filename, Graph.Displaying.Measured);

            XmlNode attr = file.CreateNode(XmlNodeType.Attribute, TIME_SPECTRUM_ATTRIBUTE, "");
            attr.Value = dt.ToString("G", DateTimeFormatInfo.InvariantInfo);
            file.SelectSingleNode("/control/header").Attributes.Append(attr as XmlAttribute);

            XmlNode commonNode = createCommonOptsStub(file, file.SelectSingleNode(ROOT_CONFIG_TAG));
            saveCommonOptions(commonNode);
            file.Save(filename);
        }

        internal static void DistractSpectra(string what, ushort step, Graph.pListScaled plsReference, Utility.PreciseEditorData pedReference) {
            if (Graph.isPreciseSpectrum) {
                PreciseSpectrum peds = new PreciseSpectrum();
                if (OpenPreciseSpecterFile(what, peds)) {
                    List<Utility.PreciseEditorData> temp;
                    switch (Graph.DisplayingMode) {
                        case Graph.Displaying.Loaded:
                            temp = new List<Utility.PreciseEditorData>(preciseDataLoaded);
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
                        preciseDataDiff = temp;
                        Graph.updateGraphAfterPreciseDiff(temp);
                    } catch (System.ArgumentException) {
                        throw new ConfigLoadException("Несовпадение рядов данных", "Ошибка при вычитании спектров", what);
                    }
                }
            } else {
                PointPairListPlus pl12 = new PointPairListPlus();
                PointPairListPlus pl22 = new PointPairListPlus();
                if (OpenSpecterFile(what, pl12, pl22) == Graph.Displaying.Measured) {
                    // coeff counting
                    double coeff = 1.0;
                    if (plsReference != null) {
                        PointPairListPlus PL = plsReference.IsFirstCollector ? Graph.Displayed1Steps[0] : Graph.Displayed2Steps[0];
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
                        PointPairListPlus diff1 = PointPairListDiff(Graph.Displayed1Steps[0], pl12, coeff);
                        PointPairListPlus diff2 = PointPairListDiff(Graph.Displayed2Steps[0], pl22, coeff);
                        Graph.ResetDiffPointLists();
                        Graph.updateGraphAfterScanDiff(diff1, diff2);
                    } catch (System.ArgumentException) {
                        throw new ConfigLoadException("Несовпадение рядов данных", "Ошибка при вычитании спектров", what);
                    }
                } else {
                    //diffs can't be distracted!
                    throw new ArgumentOutOfRangeException();
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
            if (from.AssociatedPoints.Count != what.AssociatedPoints.Count)
                throw new System.ArgumentException();
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

        private static bool OpenPreciseSpecterFile(string filename) {
            PreciseSpectrum peds = new PreciseSpectrum();
            bool result = OpenPreciseSpecterFile(filename, peds);
            if (result) {
                preciseDataLoaded = peds;
                Graph.updateGraphAfterPreciseLoad();
            }
            return result;
        }
        private static bool OpenPreciseSpecterFile(string filename, PreciseSpectrum peds) {
            XmlDocument sf = new XmlDocument();
            string prefix = "";
            try {
                sf.Load(filename);
            } catch (Exception Error) {
                throw new ConfigLoadException(Error.Message, "Ошибка чтения файла прецизионного спектра", filename);
            }
            if (sf.SelectSingleNode("control/sense") != null)
                prefix = mainConfigPrefix;
            else if (sf.SelectSingleNode("sense") == null) {
                throw new ConfigLoadException("Ошибка структуры файла", "Ошибка чтения файла прецизионного спектра", filename);
            }

            CommonOptions co = null;
            try {
                // what version of function will be called here?
                loadCommonOptions(filename, sf, out co);
            } catch (structureErrorOnLoadCommonData) {
                co = null;
            }
            peds.CommonOptions = co;

            return LoadPED(sf, filename, peds, true, prefix);
        }
        internal static XmlDocument SavePreciseSpecterFile(string filename, Graph.Displaying displayMode) {
            List<Utility.PreciseEditorData> processed;
            string header;
            switch (displayMode) {
                case Graph.Displaying.Loaded:
                    processed = Config.PreciseDataLoaded;
                    //!!!!
                    header = "Measure";
                    break;
                case Graph.Displaying.Measured:
                    processed = Config.PreciseData;
                    header = "Measure";
                    break;
                case Graph.Displaying.Diff:
                    processed = Config.PreciseDataDiff;
                    header = "Diff";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return SavePreciseOptions(processed, filename, header, true, false);
        }
        internal static DateTime AutoSavePreciseSpecterFile(short shift) {
            DateTime dt = System.DateTime.Now;
            string filename = genAutoSaveFilename(PRECISE_SPECTRUM_EXT, dt);
            XmlDocument file = SavePreciseSpecterFile(filename, Graph.Displaying.Measured);

            XmlAttributeCollection headerNodeAttributes = file.SelectSingleNode("/control/header").Attributes;
            XmlNode attr = file.CreateNode(XmlNodeType.Attribute, TIME_SPECTRUM_ATTRIBUTE, "");
            attr.Value = dt.ToString("G", DateTimeFormatInfo.InvariantInfo);
            headerNodeAttributes.Append(attr as XmlAttribute);

            attr = file.CreateNode(XmlNodeType.Attribute, SHIFT_SPECTRUM_ATTRIBUTE, "");
            attr.Value = shift.ToString();
            headerNodeAttributes.Append(attr as XmlAttribute);

            XmlNode commonNode = createCommonOptsStub(file, file.SelectSingleNode(ROOT_CONFIG_TAG));
            saveCommonOptions(commonNode);
            file.Save(filename);

            return dt;
        }
        // TODO: unify with previous!
        internal static void autoSaveMonitorSpectrumFile(short shift) {
            DateTime dt = AutoSavePreciseSpecterFile(shift);// now both files are saved
            string filename = genAutoSaveFilename(MONITOR_SPECTRUM_EXT, dt);
            XmlDocument file = SavePreciseOptions(Config.PreciseData, filename, MONITOR_SPECTRUM_HEADER, false, true);

            XmlAttributeCollection headerNodeAttributes = file.SelectSingleNode("/control/header").Attributes;
            XmlNode attr = file.CreateNode(XmlNodeType.Attribute, TIME_SPECTRUM_ATTRIBUTE, "");
            attr.Value = dt.ToString("G", DateTimeFormatInfo.InvariantInfo);
            headerNodeAttributes.Append(attr as XmlAttribute);

            attr = file.CreateNode(XmlNodeType.Attribute, SHIFT_SPECTRUM_ATTRIBUTE, "");
            attr.Value = shift.ToString();
            headerNodeAttributes.Append(attr as XmlAttribute);

            XmlNode commonNode = createCommonOptsStub(file, file.SelectSingleNode(ROOT_CONFIG_TAG));
            saveCommonOptions(commonNode);
            file.Save(filename);
        }

        private static void SavePreciseOptions() {
            SavePreciseOptions(Config.PreciseData, confName, PRECISE_OPTIONS_HEADER, false, false);
        }
        internal static void SavePreciseOptions(List<Utility.PreciseEditorData> peds) {
            preciseData = peds;
            SavePreciseOptions(peds, confName, PRECISE_OPTIONS_HEADER, false, false);
        }
        internal static XmlDocument SavePreciseOptions(List<Utility.PreciseEditorData> peds, string pedConfName, string header, bool savePoints, bool savePeakSum) {
            XmlDocument pedConf;
            string mainConfPrefix = "";
            mainConfPrefix = mainConfigPrefix;

            if (newConfigFile(out pedConf, pedConfName)) {
                XmlNode rootNode = createRootStub(pedConf, header);
                createPEDStub(pedConf, rootNode);
            } else {
                for (int i = 1; i <= 20; ++i) {
                    string prefix = string.Format("/control/sense/region{0}", i);
                    fillInnerText(prefix, "peak", "");
                    fillInnerText(prefix, "iteration", "");
                    fillInnerText(prefix, "width", "");
                    fillInnerText(prefix, "error", "");
                    fillInnerText(prefix, "col", "");
                    fillInnerText(prefix, "comment", "");
                    fillInnerText(prefix, "use", "");
                }
            }

            foreach (Utility.PreciseEditorData ped in peds) {
                XmlNode regionNode = pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}", ped.pNumber + 1));
                regionNode.SelectSingleNode("peak").InnerText = ped.Step.ToString();
                regionNode.SelectSingleNode("iteration").InnerText = ped.Iterations.ToString();
                regionNode.SelectSingleNode("width").InnerText = ped.Width.ToString();
                regionNode.SelectSingleNode("error").InnerText = ped.Precision.ToString();
                regionNode.SelectSingleNode("col").InnerText = ped.Collector.ToString();
                regionNode.SelectSingleNode("comment").InnerText = ped.Comment;
                regionNode.SelectSingleNode("use").InnerText = ped.Use.ToString();

                if (ped.AssociatedPoints == null) {
                    continue;
                }

                if (savePeakSum) {
                    regionNode.AppendChild(pedConf.CreateNode(XmlNodeType.Element, "sum", "")).InnerText = ped.AssociatedPoints.PLSreference.PeakSum.ToString();
                }
                if (savePoints) {
                    XmlNode temp;
                    foreach (ZedGraph.PointPair pp in ped.AssociatedPoints) {
                        temp = pedConf.CreateNode(XmlNodeType.Element, "p", "");
                        temp.AppendChild(pedConf.CreateNode(XmlNodeType.Element, "s", "")).InnerText = pp.X.ToString();
                        temp.AppendChild(pedConf.CreateNode(XmlNodeType.Element, "c", "")).InnerText = ((long)(pp.Y)).ToString();
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
                if (pedConf.SelectSingleNode("control/sense") != null)
                    mainConfPrefix = mainConfigPrefix;
                else if (pedConf.SelectSingleNode("sense") == null) {
                    throw new structureErrorOnLoadPrecise(pedConfName);
                }
            } else {
                pedConf = _conf;
                mainConfPrefix = mainConfigPrefix;
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
                    XmlNode regionNode = pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}", i));
                    peak = regionNode.SelectSingleNode("peak").InnerText;
                    col = regionNode.SelectSingleNode("col").InnerText;
                    iter = regionNode.SelectSingleNode("iteration").InnerText;
                    width = regionNode.SelectSingleNode("width").InnerText;
                    bool allFilled = ((peak != "") && (iter != "") && (width != "") && (col != ""));
                    if (allFilled) {
                        string comment = "";
                        try {
                            comment = regionNode.SelectSingleNode("comment").InnerText;
                        } catch (NullReferenceException) { }
                        bool use = true;
                        try {
                            use = bool.Parse(regionNode.SelectSingleNode("use").InnerText);
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
                                foreach (XmlNode pntNode in regionNode.SelectNodes("p")) {
                                    X = ushort.Parse(pntNode.SelectSingleNode("s").InnerText);
                                    Y = long.Parse(pntNode.SelectSingleNode("c").InnerText);
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
                XmlNode rootNode = createRootStub(cdConf, "Common options");
                commonNode = createCommonOptsStub(cdConf, rootNode);
            } else {
                commonNode = cdConf.SelectSingleNode("control/common");
            }
            saveCommonOptions(commonNode);
            cdConf.Save(filename);
        }
        private static void saveCommonOptions(XmlNode commonNode) {
            commonNode.SelectSingleNode("exptime").InnerText = Config.commonOpts.eTime.ToString();
            commonNode.SelectSingleNode("meastime").InnerText = Config.commonOpts.iTime.ToString();
            commonNode.SelectSingleNode("ivoltage").InnerText = Config.commonOpts.iVoltage.ToString();
            commonNode.SelectSingleNode("cp").InnerText = Config.commonOpts.CP.ToString();
            commonNode.SelectSingleNode("ecurrent").InnerText = Config.commonOpts.eCurrent.ToString();
            commonNode.SelectSingleNode("hcurrent").InnerText = Config.commonOpts.hCurrent.ToString();
            commonNode.SelectSingleNode("focus1").InnerText = Config.commonOpts.fV1.ToString();
            commonNode.SelectSingleNode("focus2").InnerText = Config.commonOpts.fV2.ToString();

            commonNode.SelectSingleNode("before").InnerText = Config.commonOpts.befTime.ToString();
            commonNode.SelectSingleNode("equal").InnerText = Config.commonOpts.ForwardTimeEqualsBeforeTime.ToString();
            commonNode.SelectSingleNode("forward").InnerText = Config.commonOpts.fTime.ToString();
            commonNode.SelectSingleNode("back").InnerText = Config.commonOpts.bTime.ToString();
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
            loadCommonOptions(confName, out Config.commonOpts);// not good fix
        }
        internal static void loadCommonOptions(string cdConfName) {
            loadCommonOptions(cdConfName, out Config.commonOpts);// not good fix
        }
        private static void loadCommonOptions(string cdConfName, out CommonOptions commonOpts) {
            XmlDocument cdConf;
            newCommonOptionsFileOnLoad(out cdConf, cdConfName);

            loadCommonOptions(cdConfName, cdConf, out commonOpts);
        }
        private static void loadCommonOptions(string cdConfName, XmlDocument cdConf, out CommonOptions commonOpts) {
            string mainConfPrefix = "";

            if (cdConf.SelectSingleNode("control/common") != null)
                mainConfPrefix = mainConfigPrefix;
            else if (cdConf.SelectSingleNode("common") == null) {
                throw new structureErrorOnLoadCommonData(cdConfName);
            }
            XmlNode commonNode = cdConf.SelectSingleNode(mainConfPrefix + "common");

            loadCommonOptions(cdConfName, commonNode, out commonOpts);
        }
        private static void loadCommonOptions(string cdConfName, XmlNode commonNode, out CommonOptions commonOpts) {
            try {
                ushort eT, iT, iV, CP, eC, hC, fV1, fV2;

                eT = ushort.Parse(commonNode.SelectSingleNode("exptime").InnerText);
                iT = ushort.Parse(commonNode.SelectSingleNode("meastime").InnerText);
                iV = ushort.Parse(commonNode.SelectSingleNode("ivoltage").InnerText);
                CP = ushort.Parse(commonNode.SelectSingleNode("cp").InnerText);
                eC = ushort.Parse(commonNode.SelectSingleNode("ecurrent").InnerText);
                hC = ushort.Parse(commonNode.SelectSingleNode("hcurrent").InnerText);
                fV1 = ushort.Parse(commonNode.SelectSingleNode("focus1").InnerText);
                fV2 = ushort.Parse(commonNode.SelectSingleNode("focus2").InnerText);

                commonOpts = new CommonOptions();
                commonOpts.eTime = eT;
                commonOpts.iTime = iT;
                commonOpts.iVoltage = iV;
                commonOpts.CP = CP;
                commonOpts.eCurrent = eC;
                commonOpts.hCurrent = hC;
                commonOpts.fV1 = fV1;
                commonOpts.fV2 = fV2;
            } catch (NullReferenceException) {
                commonOpts = null;
                throw new structureErrorOnLoadCommonData(cdConfName);
            }

            try {
                ushort befT, fT, bT;
                bool fAsbef;

                befT = ushort.Parse(commonNode.SelectSingleNode("before").InnerText);
                fT = ushort.Parse(commonNode.SelectSingleNode("forward").InnerText);
                bT = ushort.Parse(commonNode.SelectSingleNode("back").InnerText);
                fAsbef = bool.Parse(commonNode.SelectSingleNode("equal").InnerText);

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
            XmlNode interfaceNode = conf.SelectSingleNode(mainConfigPrefix + "interface");

            try {
                col1Coeff = double.Parse(interfaceNode.SelectSingleNode("coeff1").InnerText);
                col2Coeff = double.Parse(interfaceNode.SelectSingleNode("coeff2").InnerText);
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
                    Graph.RecomputeMassRows(col);
                }
            } else {
                if (value != col2Coeff) {
                    col2Coeff = value;
                    Graph.RecomputeMassRows(col);
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
                Console.WriteLine(message + cause);
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
            conf.AppendChild(conf.CreateNode(XmlNodeType.XmlDeclaration, "?xml version=\"1.0\" encoding=\"utf-8\" ?", ""));
            XmlNode rootNode = conf.CreateNode(XmlNodeType.Element, ROOT_CONFIG_TAG, "");
            conf.AppendChild(rootNode);
            XmlNode headerNode = conf.CreateNode(XmlNodeType.Element, "header", "");
            headerNode.InnerText = header;


            rootNode.AppendChild(headerNode);

            return rootNode;
        }
        private static XmlNode createCommonOptsStub(XmlDocument conf, XmlNode mountPoint) {
            XmlNode commonNode = conf.CreateNode(XmlNodeType.Element, "common", "");
            commonNode.AppendChild(conf.CreateNode(XmlNodeType.Element, "header", ""));
            commonNode.AppendChild(conf.CreateNode(XmlNodeType.Element, "exptime", ""));
            commonNode.AppendChild(conf.CreateNode(XmlNodeType.Element, "meastime", ""));
            commonNode.AppendChild(conf.CreateNode(XmlNodeType.Element, "ivoltage", ""));
            commonNode.AppendChild(conf.CreateNode(XmlNodeType.Element, "cp", ""));
            commonNode.AppendChild(conf.CreateNode(XmlNodeType.Element, "ecurrent", ""));
            commonNode.AppendChild(conf.CreateNode(XmlNodeType.Element, "hcurrent", ""));
            commonNode.AppendChild(conf.CreateNode(XmlNodeType.Element, "focus1", ""));
            commonNode.AppendChild(conf.CreateNode(XmlNodeType.Element, "focus2", ""));

            commonNode.AppendChild(conf.CreateNode(XmlNodeType.Element, "before", ""));
            commonNode.AppendChild(conf.CreateNode(XmlNodeType.Element, "equal", ""));
            commonNode.AppendChild(conf.CreateNode(XmlNodeType.Element, "forward", ""));
            commonNode.AppendChild(conf.CreateNode(XmlNodeType.Element, "back", ""));
            mountPoint.AppendChild(commonNode);
            return commonNode;
        }
        private static XmlNode createPEDStub(XmlDocument pedConf, XmlNode mountPoint) {
            XmlNode senseNode = pedConf.CreateNode(XmlNodeType.Element, "sense", "");

            for (int i = 1; i <= 20; ++i) {
                XmlNode tempRegion = pedConf.CreateNode(XmlNodeType.Element, string.Format("region{0}", i), "");
                tempRegion.AppendChild(pedConf.CreateNode(XmlNodeType.Element, "peak", ""));
                tempRegion.AppendChild(pedConf.CreateNode(XmlNodeType.Element, "col", ""));
                tempRegion.AppendChild(pedConf.CreateNode(XmlNodeType.Element, "iteration", ""));
                tempRegion.AppendChild(pedConf.CreateNode(XmlNodeType.Element, "width", ""));
                tempRegion.AppendChild(pedConf.CreateNode(XmlNodeType.Element, "error", ""));
                tempRegion.AppendChild(pedConf.CreateNode(XmlNodeType.Element, "comment", ""));
                tempRegion.AppendChild(pedConf.CreateNode(XmlNodeType.Element, "use", ""));
                senseNode.AppendChild(tempRegion);
            }
            mountPoint.AppendChild(senseNode);
            return senseNode;
        }
    }
}