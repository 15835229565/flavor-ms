using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Flavor
{
    static class Config
    {
        private static XmlDocument _conf = new XmlDocument();
        private static string mainConfigPrefix = "/control/";

        private static string initialDir;
        private static string confName;
        private static string logName;

        private static string SerialPort = "COM1";
        private static ushort SerialBaudRate = 38400;
        private static byte SendTry = 1;

        private static ushort startPoint = 0;
        private static ushort endPoint = 1056;

        private static ushort beforeTime = 100;
        private static ushort forwardTime = 100;
        private static ushort backwardTime = 400;
        private static bool forwardAsBefore = false;

        private static ushort expTime = 200;
        private static ushort idleTime = 10;
        private static ushort ionizationVoltage = 1911;
        private static ushort CPVoltage = 3780;
        private static ushort heatCurrent = 0;
        private static ushort emissionCurrent = 79;
        private static ushort focusVoltage1 = 2730;
        private static ushort focusVoltage2 = 2730;
        
        private static List<Utility.PreciseEditorData> preciseData = new List<Utility.PreciseEditorData>();
        private static List<Utility.PreciseEditorData> preciseDataLoaded = new List<Utility.PreciseEditorData>();
        private static List<Utility.PreciseEditorData> preciseDataDiff = new List<Utility.PreciseEditorData>();

        public static List<Utility.PreciseEditorData> PreciseData
        {
            get { return preciseData; }
            //set { preciseData = value; }
        }
        public static List<Utility.PreciseEditorData> PreciseDataLoaded
        {
            get { return preciseDataLoaded; }
            //set { preciseData = value; }
        }
        public static List<Utility.PreciseEditorData> PreciseDataDiff
        {
            get { return preciseDataDiff; }
            //set { preciseData = value; }
        }
        
        public static string Port
        {
            get { return SerialPort; }
            set { SerialPort = value; }
        }
        public static ushort BaudRate
        {
            get { return SerialBaudRate; }
            set { SerialBaudRate = value; }
        }

        public static byte Try
        {
            get { return SendTry; }
            set { SendTry = value; }
        }

        public static ushort sPoint
        {
            get { return startPoint; }
            set { startPoint = value; }
        }
        public static ushort ePoint
        {
            get { return endPoint; }
            set { endPoint = value; }
        }

        public static ushort befTime
        {
            get { return beforeTime; }
        }
        public static ushort befTimeReal
        {
            get { return (ushort)(beforeTime * 5); }
            set
            {
                beforeTime = (ushort)(value / 5);
            }
        }
        public static ushort fTime
        {
            get { return forwardTime; }
        }
        public static ushort fTimeReal
        {
            get { return (ushort)(forwardTime * 5); }
            set
            {
                forwardTime = (ushort)(value / 5);
            }
        }
        public static ushort bTime
        {
            get { return backwardTime; }
        }
        public static ushort bTimeReal
        {
            get { return (ushort)(backwardTime * 5); }
            set
            {
                backwardTime = (ushort)(value / 5);
            }
        }
        public static bool ForwardTimeEqualsBeforeTime
        {
            get { return forwardAsBefore; }
            set { forwardAsBefore = value; }
        }

        public static ushort eTime
        {
            get { return expTime; }
            //set { expTime = value; }
        }
        public static ushort eTimeReal
        {
            get { return (ushort)(expTime * 5); }
            set
            {
                expTime = (ushort)(value / 5);
            }
        }

        public static ushort iTime
        {
            get { return idleTime; }
            //set { idleTime = value; }
        }
        public static ushort iTimeReal
        {
            get { return (ushort)(5 * idleTime); }
            set
            {
                idleTime = (ushort)(value / 5);
            }
        }

        public static ushort iVoltage
        {
            get { return ionizationVoltage; }
            set { ionizationVoltage = value; }
        }
        public static double iVoltageReal
        {
            get { return iVoltageConvert(ionizationVoltage); }
            set
            {
                ionizationVoltage = iVoltageConvert(value);
            }
        }
        public static double iVoltageConvert(ushort voltage)
        {
            return (double)(150 * (double)voltage / 4096);
        }
        public static ushort iVoltageConvert(double voltage)
        {
            ushort x = (ushort)((voltage / 150) * 4096);
            if (x >= 4096) x = 4095;
            return x;
        }

        public static ushort CP
        {
            get { return CPVoltage; }
            set { CPVoltage = value; }
        }
        public static double CPReal
        {
            get { return CPConvert(CPVoltage); }
            set
            {
                CPVoltage = CPConvert(value);
            }
        }
        public static double CPConvert(ushort coeff)
        {
            return (double)((10 / (double)coeff) * 4096);
        }
        public static ushort CPConvert(double coeff)
        {
            ushort x = (ushort)((10 / coeff) * 4096);
            if (x >= 4096) x = 4095;
            return x;
        }

        public static ushort eCurrent
        {
            get { return emissionCurrent; }
            set { emissionCurrent = value; }
        }
        public static double eCurrentReal
        {
            get { return eCurrentConvert(emissionCurrent); }
            set
            {
                emissionCurrent = eCurrentConvert(value);
            }
        }
        public static double eCurrentConvert(ushort current)
        {
            return (double)((50 * (double)current) / 4096);
        }
        public static ushort eCurrentConvert(double current)
        {
            ushort x = (ushort)((current / 50) * 4096);
            if (x >= 4096) x = 4095;
            return x;
        }

        public static ushort hCurrent
        {
            get { return heatCurrent; }
            set { heatCurrent = value; }
        }
        public static double hCurrentReal
        {
            get { return hCurrentConvert(heatCurrent); }
            set
            {
                heatCurrent = hCurrentConvert(value);
            }
        }
        public static double hCurrentConvert(ushort current)
        {
            return (double)((double)current / 4096);
        }
        public static ushort hCurrentConvert(double current)
        {
            ushort x = (ushort)(current * 4096);
            if (x >= 4096) x = 4095;
            return x;
        }

        public static ushort fV1
        {
            get { return focusVoltage1; }
            set { focusVoltage1 = value; }
        }
        public static double fV1Real
        {
            get { return fV1Convert(focusVoltage1); }
            set
            {
                focusVoltage1 = fV1Convert(value);
            }
        }
        public static double fV1Convert(ushort voltage)
        {
            return (double)(150 * (double)voltage / 4096);
        }
        public static ushort fV1Convert(double voltage)
        {
            ushort x = (ushort)((voltage / 150) * 4096);
            if (x >= 4096) x = 4095;
            return x;
        }

        public static ushort fV2
        {
            get { return focusVoltage2; }
            set { focusVoltage2 = value; }
        }
        public static double fV2Real
        {
            get { return fV2Convert(focusVoltage2); }
            set
            {
                focusVoltage2 = fV2Convert(value);
            }
        }
        public static double fV2Convert(ushort voltage)
        {
            return (double)(150 * (double)voltage / 4096);
        }
        public static ushort fV2Convert(double voltage)
        {
            ushort x = (ushort)((voltage / 150) * 4096);
            if (x >= 4096) x = 4095;
            return x;
        }

        public static ushort scanVoltage(ushort step)
        {
            if (step > 1056) step = 1056;
            return Convert.ToUInt16(4095 * Math.Pow(((double)527 / (double)528), 1056 - step));
            //if (step <= 456) return (ushort)(4095 - 5 * step);
            //return (ushort)(4095 - 5 * 456 - 2 * (step - 456));
        }
        public static double scanVoltageReal(ushort step)
        {
            return (double)(scanVoltage(step) * 5 * 600) / 4096;
        }

        internal static void getInitialDirectory()
        {
            initialDir = System.IO.Directory.GetCurrentDirectory();
            confName = initialDir + "\\config.xml";
            logName = initialDir + "\\MScrash.log";
        }

        private static void fillInnerText(string prefix, string nodeName, object value)
        {
            try
            {
                _conf.SelectSingleNode(prefix + "/" + nodeName).InnerText = value.ToString();
            }
            catch (NullReferenceException)
            {
                _conf.SelectSingleNode(prefix).AppendChild(_conf.CreateNode(XmlNodeType.Element, nodeName, ""));
                _conf.SelectSingleNode(prefix + "/" + nodeName).InnerText = value.ToString();
            }
        }

        internal static void loadConfig()
        {
            try
            {
                _conf.Load(confName);
            }
            catch (Exception Error)
            {
                throw new ConfigLoadException(Error.Message, "Ошибка чтения конфигурационного файла", confName);
            }
            try
            {
                SerialPort = (_conf.SelectSingleNode("/control/connect/port").InnerText);
                SerialBaudRate = ushort.Parse(_conf.SelectSingleNode("/control/connect/baudrate").InnerText);
                SendTry = byte.Parse(_conf.SelectSingleNode("/control/connect/try").InnerText);
            }
            catch (NullReferenceException)
            {
                (new ConfigLoadException("Ошибка структуры конфигурационного файла", "Ошибка чтения конфигурационного файла", confName)).visualise();
                //use hard-coded defaults
            }
            try
            {
                sPoint = ushort.Parse(_conf.SelectSingleNode("/control/overview/start").InnerText);
                ePoint = ushort.Parse(_conf.SelectSingleNode("/control/overview/end").InnerText);
            }
            catch (NullReferenceException)
            {
                (new ConfigLoadException("Ошибка структуры конфигурационного файла", "Ошибка чтения конфигурационного файла", confName)).visualise();
                //use hard-coded defaults
            }
            try
            {
                loadMassCoeffs();
            }
            catch (ConfigLoadException)
            {
                //cle.visualise();
                //use hard-coded defaults
            }
            try
            {
                loadCommonOptions();
            }
            catch (ConfigLoadException cle)
            {
                cle.visualise();
                //use hard-coded defaults
            }
            try
            {
                LoadPreciseEditorData();
            }
            catch (ConfigLoadException cle)
            {
                cle.visualise();
                //use empty default ped
            }
        }

        private static void saveScanOptions()
        {
            fillInnerText("/control/overview", "start", sPoint);
            fillInnerText("/control/overview", "end", ePoint);
            _conf.Save(@confName);
        }
        internal static void saveScanOptions(ushort sPointReal, ushort ePointReal)
        {
            Config.sPoint = sPointReal;//!!!
            Config.ePoint = ePointReal;//!!!
            Config.saveScanOptions();
        }

        private static void saveDelaysOptions()
        {
            fillInnerText("/control/common", "before", beforeTime);
            fillInnerText("/control/common", "equal", forwardAsBefore);
            fillInnerText("/control/common", "forward", forwardTime);
            fillInnerText("/control/common", "back", backwardTime);
            _conf.Save(@confName);
        }
        internal static void saveDelaysOptions(bool forwardAsBefore, ushort befTimeReal, ushort fTimeReal, ushort bTimeReal)
        {
            Config.befTimeReal = befTimeReal;
            Config.fTimeReal = fTimeReal;
            Config.bTimeReal = bTimeReal;
            Config.forwardAsBefore = forwardAsBefore;
            Config.saveDelaysOptions();
        }

        private static void saveMassCoeffs()
        {
            fillInnerText("/control", "interface", "");
            fillInnerText("/control/interface", "coeff1", col1Coeff.ToString("R"));
            fillInnerText("/control/interface", "coeff2", col2Coeff.ToString("R"));
            _conf.Save(@confName);
        }

        private static void saveConnectOptions()
        {
            fillInnerText("/control/connect", "port", Port);
            fillInnerText("/control/connect", "baudrate", BaudRate);
            _conf.Save(@confName);
        }
        internal static void saveConnectOptions(string port, ushort baudrate)
        {
            Config.Port = port;
            Config.BaudRate = baudrate;
            Config.saveConnectOptions();
        }

        internal static void saveAll()
        {
            saveConnectOptions();
            saveScanOptions();
            saveCommonOptions();
            saveDelaysOptions();
            saveMassCoeffs();
            SavePreciseOptions();
        }

        internal static bool openSpectrumFile(string filename, bool hint)
        {
            bool result;
            ConfigLoadException resultException = null;
            try
            {
                result = hint ? OpenSpecterFile(filename) : OpenPreciseSpecterFile(filename);
            }
            catch (ConfigLoadException cle)
            {
                resultException = cle;
                result = false;
            }
            if (result)
                return hint;
            try
            {
                result = (!hint) ? OpenSpecterFile(filename) : OpenPreciseSpecterFile(filename);
            }
            catch (ConfigLoadException cle)
            {
                resultException = (resultException == null) ? cle : resultException;
                result = false;
            }
            if (result)
                return (!hint);
            throw resultException;
        }
        
        private static string genAutoSaveFilename(string extension)
        {
            string dirname;
            DateTime now = System.DateTime.Now;
            dirname = initialDir + string.Format("\\{0}-{1}-{2}", now.Year, now.Month, now.Day);
            if (!System.IO.Directory.Exists(@dirname))
                System.IO.Directory.CreateDirectory(@dirname);
            return dirname + "\\" + string.Format("{0}-{1}-{2}-{3}.", now.Hour, now.Minute, now.Second, now.Millisecond) + extension;
        }
        
        private static bool OpenSpecterFile(string p)
        {
            PointPairListPlus pl1 = new PointPairListPlus(), pl2 = new PointPairListPlus();
            Graph.Displaying result = OpenSpecterFile(p, pl1, pl2);
            
            switch (result)
            {
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
        private static Graph.Displaying OpenSpecterFile(string filename, PointPairListPlus pl1, PointPairListPlus pl2)
        {
            XmlDocument sf = new XmlDocument();
            XmlNode headerNode = null;
            Graph.Displaying spectrumType = Graph.Displaying.Measured;
            try
            {
                sf.Load(filename);
            }
            catch (Exception Error)
            {
                throw new ConfigLoadException(Error.Message, "Ошибка чтения файла спектра", filename);
            }
            string prefix = "";
            if (sf.SelectSingleNode("control/overview") != null)
            {
                prefix = mainConfigPrefix;
                headerNode = sf.SelectSingleNode("control/header");
            }
            else if (sf.SelectSingleNode("overview") != null)
            {
                headerNode = sf.SelectSingleNode("overview/header");
            }
            else
            {
                throw new ConfigLoadException("Ошибка структуры файла", "Ошибка чтения файла спектра", filename);
            }
            if (headerNode != null && headerNode.InnerText == "Diff")
                spectrumType = Graph.Displaying.Diff;

            ushort X = 0;
            long Y = 0;
            try
            {
                foreach (XmlNode pntNode in sf.SelectNodes(prefix + "overview/collector1/p"))
                {
                    X = ushort.Parse(pntNode.SelectSingleNode("s").InnerText);
                    Y = long.Parse(pntNode.SelectSingleNode("c").InnerText);
                    pl1.Add(X, Y);
                }
                foreach (XmlNode pntNode in sf.SelectNodes(prefix + "overview/collector2/p"))
                {
                    X = ushort.Parse(pntNode.SelectSingleNode("s").InnerText);
                    Y = long.Parse(pntNode.SelectSingleNode("c").InnerText);
                    pl2.Add(X, Y);
                }
            }
            catch (NullReferenceException)
            {
                throw new ConfigLoadException("Ошибка структуры файла", "Ошибка чтения файла спектра", filename);
            }
            pl1.Sort(ZedGraph.SortType.XValues);
            pl2.Sort(ZedGraph.SortType.XValues);
            return spectrumType;
        }
        internal static XmlDocument SaveSpecterFile(string p, Graph.Displaying displayMode)
        {
            XmlDocument sf = new XmlDocument();
            XmlNode temp;
            XmlNode scanNode = createRootStub(sf, "").AppendChild(sf.CreateNode(XmlNodeType.Element, "overview", ""));
            switch (displayMode)
            {
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
            foreach (ZedGraph.PointPair pp in Graph.Displayed1Steps[0])
            {
                temp = sf.CreateNode(XmlNodeType.Element, "p", "");
                temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "s", "")).InnerText = pp.X.ToString();
                temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "c", "")).InnerText = ((long)(pp.Y)).ToString();
                scanNode.SelectSingleNode("collector1").AppendChild(temp);
            }
            foreach (ZedGraph.PointPair pp in Graph.Displayed2Steps[0])
            {
                temp = sf.CreateNode(XmlNodeType.Element, "p", "");
                temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "s", "")).InnerText = pp.X.ToString();
                temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "c", "")).InnerText = ((long)(pp.Y)).ToString();
                scanNode.SelectSingleNode("collector2").AppendChild(temp);
            }
            sf.Save(@p);
            return sf;
        }
        internal static void AutoSaveSpecterFile()
        {
            string filename = genAutoSaveFilename("sdf");
            XmlDocument file = SaveSpecterFile(filename, Graph.Displaying.Measured);
            XmlNode commonNode = createCommonOptsStub(file, file.SelectSingleNode("control"));
            saveCommonOptions(commonNode);
            file.Save(filename);
        }

        internal static void DistractSpectra(string what, ushort step, Graph.pListScaled plsReference, Utility.PreciseEditorData pedReference)
        {
            if (Graph.isPreciseSpectrum)
            {
                List<Utility.PreciseEditorData> peds = new List<Utility.PreciseEditorData>();
                if (OpenPreciseSpecterFile(what, peds))
                {
                    List<Utility.PreciseEditorData> temp;
                    switch (Graph.DisplayingMode)
                    {
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
                    try
                    {
                        temp = PreciseEditorDataListDiff(temp, peds, step, pedReference);
                        preciseDataDiff = temp;
                        Graph.updateGraphAfterPreciseDiff(temp);
                    }
                    catch (System.ArgumentException)
                    {
                        throw new ConfigLoadException("Несовпадение рядов данных", "Ошибка при вычитании спектров", what);
                    }
                }
            }
            else
            {
                PointPairListPlus pl12 = new PointPairListPlus();
                PointPairListPlus pl22 = new PointPairListPlus();
                if (OpenSpecterFile(what, pl12, pl22) == Graph.Displaying.Measured)
                {
                    // coeff counting
                    double coeff = 1.0;
                    PointPairListPlus PL = plsReference.IsFirstCollector ? Graph.Displayed1Steps[0] : Graph.Displayed2Steps[0];
                    PointPairListPlus pl = plsReference.IsFirstCollector ? pl12 : pl22;
                    if (step != 0)
                    {
                        for (int i = 0; i < PL.Count; ++i)
                        {
                            if (step == PL[i].X)
                            {
                                if (step != pl[i].X)
                                    throw new System.ArgumentException();
                                if ((pl[i].Y != 0) && (PL[i].Y != 0))
                                    coeff = PL[i].Y / pl[i].Y;
                                break;
                            }
                        }
                    }
                    try
                    {
                        PointPairListPlus diff1 = PointPairListDiff(Graph.Displayed1Steps[0], pl12, coeff);
                        PointPairListPlus diff2 = PointPairListDiff(Graph.Displayed2Steps[0], pl22, coeff);
                        Graph.ResetDiffPointLists();
                        Graph.updateGraphAfterScanDiff(diff1, diff2);
                    }
                    catch (System.ArgumentException)
                    {
                        throw new ConfigLoadException("Несовпадение рядов данных", "Ошибка при вычитании спектров", what);
                    }
                }
                else
                {
                    //diffs can't be distracted!
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
        private static PointPairListPlus PointPairListDiff(PointPairListPlus from, PointPairListPlus what, double coeff)
        {
            if (from.Count != what.Count)
                throw new System.ArgumentOutOfRangeException();
            
            PointPairListPlus res = new PointPairListPlus(from, null, null);
            for (int i = 0; i < res.Count; ++i)
            {
                if (res[i].X != what[i].X)
                    throw new System.ArgumentException();
                res[i].Y -= what[i].Y * coeff;
            }
            return res;
        }
        private static Utility.PreciseEditorData PreciseEditorDataDiff(Utility.PreciseEditorData from, Utility.PreciseEditorData what, double coeff)
        {
            if (!from.Equals(what))
                throw new System.ArgumentException();
            if (from.AssociatedPoints.Count != what.AssociatedPoints.Count)
                throw new System.ArgumentException();
            if (from.AssociatedPoints.Count != 2 * from.Width + 1)
                throw new System.ArgumentException();
            Utility.PreciseEditorData res = new Utility.PreciseEditorData(from);
            for (int i = 0; i < res.AssociatedPoints.Count; ++i)
            {
                if (res.AssociatedPoints[i].X != what.AssociatedPoints[i].X)
                    throw new System.ArgumentException();
                res.AssociatedPoints[i].Y -= what.AssociatedPoints[i].Y * coeff;
            }
            return res;
        }
        private static List<Utility.PreciseEditorData> PreciseEditorDataListDiff(List<Utility.PreciseEditorData> from, List<Utility.PreciseEditorData> what, ushort step, Utility.PreciseEditorData pedReference)
        {
            if (from.Count != what.Count)
                throw new System.ArgumentOutOfRangeException();
            
            // coeff counting
            double coeff = 1.0;
            if (pedReference != null)
            {
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

                if ((step != ushort.MaxValue))
                {
                    //diff on point
                    if ((what[whatIndex].AssociatedPoints[step - what[whatIndex].Step + what[whatIndex].Width].Y != 0) &&
                        (from[fromIndex].AssociatedPoints[step - from[fromIndex].Step + from[fromIndex].Width].Y != 0))
                        coeff = from[fromIndex].AssociatedPoints[step - from[fromIndex].Step + from[fromIndex].Width].Y /
                                what[whatIndex].AssociatedPoints[step - what[whatIndex].Step + what[whatIndex].Width].Y;
                }
                else
                {
                    //diff on peak sum
                    double sumFrom = from[fromIndex].AssociatedPoints.PLSreference.PeakSum;
                    if (sumFrom != 0)
                    {
                        double sumWhat = 0;
                        foreach (ZedGraph.PointPair pp in what[whatIndex].AssociatedPoints)
                        {
                            sumWhat += pp.Y;
                        }
                        if (sumWhat != 0)
                            coeff = sumFrom / sumWhat;
                    }
                }
            }
            
            List<Utility.PreciseEditorData> res = new List<Utility.PreciseEditorData>(from);
            for (int i = 0; i < res.Count; ++i)
            {
                res[i] = PreciseEditorDataDiff(res[i], what[i], coeff);
            }
            return res;
        }

        private static bool OpenPreciseSpecterFile(string filename)
        {
            List<Utility.PreciseEditorData> peds = new List<Utility.PreciseEditorData>();
            bool result = OpenPreciseSpecterFile(filename, peds);
            if (result)
            {
                preciseDataLoaded = peds;
                Graph.updateGraphAfterPreciseLoad();
            }
            return result;
        }
        private static bool OpenPreciseSpecterFile(string filename, List<Utility.PreciseEditorData> peds)
        {
            XmlDocument sf = new XmlDocument();
            string prefix = "";
            try
            {
                sf.Load(filename);
            }
            catch (Exception Error)
            {
                throw new ConfigLoadException(Error.Message, "Ошибка чтения файла прецизионного спектра", filename);
            }
            if (sf.SelectSingleNode("control/sense") != null)
                prefix = mainConfigPrefix;
            else if (sf.SelectSingleNode("sense") == null)
            {
                throw new ConfigLoadException("Ошибка структуры файла", "Ошибка чтения файла прецизионного спектра", filename);
            }
            return LoadPED(sf, filename, peds, true, prefix);
        }
        internal static XmlDocument SavePreciseSpecterFile(string filename, Graph.Displaying displayMode)
        {
            List<Utility.PreciseEditorData> processed;
            string header;
            switch (displayMode)
            {
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
            return SavePreciseOptions(processed, filename, true, header);
        }
        internal static void AutoSavePreciseSpecterFile()
        {
            string filename = genAutoSaveFilename("psf");
            XmlDocument file = SavePreciseSpecterFile(filename, Graph.Displaying.Measured);
            XmlNode commonNode = createCommonOptsStub(file, file.SelectSingleNode("control"));
            saveCommonOptions(commonNode);
            file.Save(filename);
        }

        private static void SavePreciseOptions() 
        {
            SavePreciseOptions(Config.PreciseData, confName, false, "Precise options");
        }
        internal static void SavePreciseOptions(List<Utility.PreciseEditorData> peds)
        {
            preciseData = peds;
            SavePreciseOptions(peds, confName, false, "Precise options");
        }
        internal static XmlDocument SavePreciseOptions(List<Utility.PreciseEditorData> peds, string pedConfName, bool savePoints, string header)
        {
            XmlDocument pedConf;
            string mainConfPrefix = "";
            mainConfPrefix = mainConfigPrefix;

            if (newConfigFile(out pedConf, pedConfName))
            {
                XmlNode rootNode = createRootStub(pedConf, header);
                createPEDStub(pedConf, rootNode);
            }
            else
            {
                for (int i = 1; i <= 20; ++i)
                {
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

            foreach (Utility.PreciseEditorData ped in peds)
            {
                XmlNode regionNode = pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}", ped.pNumber + 1));
                regionNode.SelectSingleNode("peak").InnerText = ped.Step.ToString();
                regionNode.SelectSingleNode("iteration").InnerText = ped.Iterations.ToString();
                regionNode.SelectSingleNode("width").InnerText = ped.Width.ToString();
                regionNode.SelectSingleNode("error").InnerText = ped.Precision.ToString();
                regionNode.SelectSingleNode("col").InnerText = ped.Collector.ToString();
                regionNode.SelectSingleNode("comment").InnerText = ped.Comment;
                regionNode.SelectSingleNode("use").InnerText = ped.Use.ToString();
                if (savePoints)
                {
                    XmlNode temp;
                    foreach (ZedGraph.PointPair pp in ped.AssociatedPoints)
                    {
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

        internal static List<Utility.PreciseEditorData> LoadPreciseEditorData(string pedConfName)
        {
            List<Utility.PreciseEditorData> peds = new List<Utility.PreciseEditorData>();
            XmlDocument pedConf;
            string mainConfPrefix = "";

            if (!pedConfName.Equals(confName))
            {
                pedConf = new XmlDocument();
                try
                {
                    pedConf.Load(pedConfName);
                }
                catch (Exception Error)
                {
                    throw new ConfigLoadException(Error.Message, "Ошибка чтения файла прецизионных точек", pedConfName);
                }
                if (pedConf.SelectSingleNode("control/sense") != null)
                    mainConfPrefix = mainConfigPrefix;
                else if (pedConf.SelectSingleNode("sense") == null)
                {
                    throw new structureErrorOnLoadPrecise(pedConfName);
                }
            }
            else 
            {
                pedConf = _conf;
                mainConfPrefix = mainConfigPrefix;
            }

            if (LoadPED(pedConf, pedConfName, peds, false, mainConfPrefix))
                return peds;
            return null;
        }
        private static void LoadPreciseEditorData()
        {
            List<Utility.PreciseEditorData> pedl = LoadPreciseEditorData(confName);
            if ((pedl != null) && (pedl.Count > 0)) 
            { 
                //BAD!!! cleaning previous points!!!
                preciseData.Clear();
                preciseData.AddRange(pedl); 
            }
        }

        private static bool LoadPED(XmlDocument pedConf, string pedConfName, List<Utility.PreciseEditorData> peds, bool readSpectrum, string mainConfPrefix)
        {
            for (int i = 1; i <= 20; ++i)
            {
                Utility.PreciseEditorData temp = null;
                string peak, iter, width, col;
                try
                {
                    XmlNode regionNode = pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}", i));
                    peak = regionNode.SelectSingleNode("peak").InnerText;
                    col = regionNode.SelectSingleNode("col").InnerText;
                    iter = regionNode.SelectSingleNode("iteration").InnerText;
                    width = regionNode.SelectSingleNode("width").InnerText;
                    bool allFilled = ((peak != "") && (iter != "") && (width != "") && (col != ""));
                    if (allFilled)
                    {
                        string comment = "";
                        try
                        {
                            comment = regionNode.SelectSingleNode("comment").InnerText;
                        }
                        catch (NullReferenceException) { }
                        bool use = true;
                        try
                        {
                            use = bool.Parse(regionNode.SelectSingleNode("use").InnerText);
                        }
                        catch (NullReferenceException) { }
                        catch (FormatException) { }
                        try
                        {
                            temp = new Utility.PreciseEditorData(use, (byte)(i - 1), ushort.Parse(peak),
                                                         byte.Parse(col), ushort.Parse(iter),
                                                         ushort.Parse(width), (float)0, comment);
                        }
                        catch (FormatException)
                        {
                            if (readSpectrum)
                                throw new ConfigLoadException("Неверный формат данных", "Ошибка чтения файла прецизионного спектра", pedConfName);
                            else
                                throw new wrongFormatOnLoadPrecise(pedConfName);
                        }
                        if (readSpectrum)
                        {
                            ushort X;
                            long Y;
                            PointPairListPlus tempPntLst = new PointPairListPlus();
                            try
                            {
                                foreach (XmlNode pntNode in regionNode.SelectNodes("p"))
                                {
                                    X = ushort.Parse(pntNode.SelectSingleNode("s").InnerText);
                                    Y = long.Parse(pntNode.SelectSingleNode("c").InnerText);
                                    tempPntLst.Add(X, Y);
                                }
                            }
                            catch (FormatException)
                            {
                                throw new ConfigLoadException("Неверный формат данных", "Ошибка чтения файла прецизионного спектра", pedConfName);
                            }
                            temp.AssociatedPoints = tempPntLst;
                        }
                    }
                }
                catch (NullReferenceException)
                {
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

        private static void saveCommonOptions()
        {
            saveCommonOptions(confName);
        }
        internal static void saveCommonOptions(ushort eT, ushort iT, double iV, double cp, double eC, double hC, double fv1, double fv2)
        {
            saveCommonOptions(confName, eT, iT, iV, cp, eC, hC, fv1, fv2);
        }
        internal static void saveCommonOptions(string filename, ushort eT, ushort iT, double iV, double cp, double eC, double hC, double fv1, double fv2)
        {
            Config.eTimeReal = eT;
            Config.iTimeReal = iT;
            Config.iVoltageReal = iV;
            Config.CPReal = cp;
            Config.eCurrentReal = eC;
            Config.hCurrentReal = hC;
            Config.fV1Real = fv1;
            Config.fV2Real = fv2;

            saveCommonOptions(filename);
        }
        private static void saveCommonOptions(string filename)
        {
            XmlDocument cdConf;
            XmlNode commonNode;

            if (newConfigFile(out cdConf, filename))
            {
                XmlNode rootNode = createRootStub(cdConf, "Common options");
                commonNode = createCommonOptsStub(cdConf, rootNode);
            }
            else 
            {
                commonNode = cdConf.SelectSingleNode("control/common");
            }
            saveCommonOptions(commonNode);
            cdConf.Save(filename);
        }
        private static void saveCommonOptions(XmlNode commonNode)
        {
            commonNode.SelectSingleNode("exptime").InnerText = Config.eTime.ToString();
            commonNode.SelectSingleNode("meastime").InnerText = Config.iTime.ToString();
            commonNode.SelectSingleNode("ivoltage").InnerText = Config.iVoltage.ToString();
            commonNode.SelectSingleNode("cp").InnerText = Config.CP.ToString();
            commonNode.SelectSingleNode("ecurrent").InnerText = Config.eCurrent.ToString();
            commonNode.SelectSingleNode("hcurrent").InnerText = Config.hCurrent.ToString();
            commonNode.SelectSingleNode("focus1").InnerText = Config.fV1.ToString();
            commonNode.SelectSingleNode("focus2").InnerText = Config.fV2.ToString();

            commonNode.SelectSingleNode("before").InnerText = Config.beforeTime.ToString();
            commonNode.SelectSingleNode("equal").InnerText = Config.forwardAsBefore.ToString();
            commonNode.SelectSingleNode("forward").InnerText = Config.forwardTime.ToString();
            commonNode.SelectSingleNode("back").InnerText = Config.backwardTime.ToString();
        }

        private static void newCommonOptionsFileOnLoad(out XmlDocument conf, string filename)
        {
            if (newConfigFile(out conf, filename))
            {
                try
                {
                    conf.Load(filename);
                }
                catch (Exception Error)
                {
                    throw new ConfigLoadException(Error.Message, "Ошибка чтения файла общих настроек", filename);
                }
            }
        }
        private static void loadCommonOptions() 
        {
            loadCommonOptions(confName);
        }
        internal static void loadCommonOptions(string cdConfName)
        {
            XmlDocument cdConf;
            string mainConfPrefix = mainConfigPrefix;

            newCommonOptionsFileOnLoad(out cdConf, cdConfName);
            XmlNode commonNode = cdConf.SelectSingleNode(mainConfPrefix + "common");
            
            try
            {
                expTime = ushort.Parse(commonNode.SelectSingleNode("exptime").InnerText);
                idleTime = ushort.Parse(commonNode.SelectSingleNode("meastime").InnerText);
                iVoltage = ushort.Parse(commonNode.SelectSingleNode("ivoltage").InnerText);
                CP = ushort.Parse(commonNode.SelectSingleNode("cp").InnerText);
                eCurrent = ushort.Parse(commonNode.SelectSingleNode("ecurrent").InnerText);
                hCurrent = ushort.Parse(commonNode.SelectSingleNode("hcurrent").InnerText);
                fV1 = ushort.Parse(commonNode.SelectSingleNode("focus1").InnerText);
                fV2 = ushort.Parse(commonNode.SelectSingleNode("focus2").InnerText);
            }
            catch (NullReferenceException)
            {
                throw new structureErrorOnLoadCommonData(cdConfName);
            }

            try
            {
                ushort befT, fT, bT;
                bool fAsbef;
                befT = ushort.Parse(commonNode.SelectSingleNode("before").InnerText);
                fT = ushort.Parse(commonNode.SelectSingleNode("forward").InnerText);
                bT = ushort.Parse(commonNode.SelectSingleNode("back").InnerText);
                fAsbef = bool.Parse(commonNode.SelectSingleNode("equal").InnerText);
                beforeTime = befT;
                forwardAsBefore = fAsbef;
                forwardTime = fT;
                backwardTime = bT;
            }
            catch (NullReferenceException)
            {
                //Use hard-coded defaults
                return;
            }
        }
        #region Error messages on loading different configs
        public class ConfigLoadException : System.Exception
        {
            public ConfigLoadException(string message, string filestring, string confname): base(message)
            {
                this.Data["FS"] = filestring;
                this.Data["CN"] = confname;
            }
            public void visualise()
            {
                if (!(this.Data["CN"].Equals(confName)))
                    System.Windows.Forms.MessageBox.Show(this.Message, this.Data["FS"] as string);
                else
                    System.Windows.Forms.MessageBox.Show(this.Message, "Ошибка чтения конфигурационного файла");
            }
        }
        private class wrongFormatOnLoadPrecise : wrongFormatOnLoad
        {
            public wrongFormatOnLoadPrecise(string configName)
                : base (configName, "Ошибка чтения файла прецизионных точек") { }
        }
        private class wrongFormatOnLoad : ConfigLoadException
        {
            public wrongFormatOnLoad(string configName, string errorFile)
                : base("Неверный формат данных", errorFile, configName) { }
        }
        private class structureErrorOnLoad : ConfigLoadException
        {
            public structureErrorOnLoad(string configName, string errorFile)
                : base("Ошибка структуры файла", errorFile, configName) { }
        }
        private class structureErrorOnLoadCommonData : structureErrorOnLoad
        {
            public structureErrorOnLoadCommonData(string configName)
                : base(configName, "Ошибка чтения файла общих настроек") { }
        }
        private class structureErrorOnLoadPrecise : structureErrorOnLoad
        {
            public structureErrorOnLoadPrecise(string configName)
                : base(configName, "Ошибка чтения файла прецизионных точек") { }
        }
        #endregion
        #region Graph scaling to mass coeffs
        private static double col1Coeff = 2770 * 28;
        private static double col2Coeff = 896.5 * 18;
        private static void loadMassCoeffs()
        {
            loadMassCoeffs(confName);
        }
        private static void loadMassCoeffs(string confName)
        {
            XmlDocument conf;

            newCommonOptionsFileOnLoad(out conf, confName);
            XmlNode interfaceNode = conf.SelectSingleNode(mainConfigPrefix + "interface");

            try
            {
                col1Coeff = double.Parse(interfaceNode.SelectSingleNode("coeff1").InnerText);
                col2Coeff = double.Parse(interfaceNode.SelectSingleNode("coeff2").InnerText);
            }
            catch (NullReferenceException)
            {
                //!!!
                throw new ConfigLoadException("", "", confName);
            }
        }
        internal static void setScalingCoeff(byte col, ushort pnt, double mass)
        {
            double value = mass * Config.scanVoltageReal(pnt);
            if (col == 1)
            {
                if (value != col1Coeff)
                {
                    col1Coeff = value;
                    Graph.RecomputeMassRows(col);
                }
            }
            else
            {
                if (value != col2Coeff)
                {
                    col2Coeff = value;
                    Graph.RecomputeMassRows(col);
                }
            }
            saveMassCoeffs();
        }
        internal static double pointToMass(ushort pnt, bool isFirstCollector)
        {
            double coeff;
            if (isFirstCollector) 
                coeff = col1Coeff;
            else 
                coeff = col2Coeff;
            return coeff / Config.scanVoltageReal(pnt);
        }
        #endregion
        #region Logging routines
        private static System.IO.StreamWriter openLog()
        {
            try
            {
                System.IO.StreamWriter errorLog;
                errorLog = new System.IO.StreamWriter(@logName, true);
                errorLog.AutoFlush = true;
                return errorLog;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Ошибка при открытии файла отказов");
                return null;
            }
        }
        private static string genMessage(string data, DateTime moment)
        {
            return string.Format("{0}-{1}-{2}|", moment.Year, moment.Month, moment.Day) +
                string.Format("{0}.{1}.{2}.{3}: ", moment.Hour, moment.Minute, moment.Second, moment.Millisecond) + data;
        }
        private static void log(System.IO.StreamWriter errorLog, string msg)
        {
            DateTime now = System.DateTime.Now;
            try
            {
                errorLog.WriteLine(genMessage(msg, now));
                //errorLog.Flush();
            }
            catch (Exception Error)
            {
                string message = "Ошибка записи файла отказов";
                string cause = "(" + msg + ") -- " + Error.Message;
                Console.WriteLine(message + cause);
                System.Windows.Forms.MessageBox.Show(cause, message);
            }
            finally
            {
                errorLog.Close();
            }
        }
        internal static void logCrash(byte[] commandline)
        {
            string cmd = "";
            List<byte> pack = new List<byte>();
            ModBus.buildPackBody(pack, commandline);
            foreach (byte b in pack)
            {
                cmd += (char)b;
            }
            System.IO.StreamWriter errorLog;
            if ((errorLog = openLog()) == null)
                return;
            log(errorLog, cmd);
        }
        internal static void logTurboPumpAlert(string message)
        {
            System.IO.StreamWriter errorLog;
            if ((errorLog = openLog()) == null)
                return;
            log(errorLog, message);
        }
        #endregion
        private static bool newConfigFile(out XmlDocument conf, string filename)
        {
            if (!filename.Equals(confName))
            {
                conf = new XmlDocument();
                return true;
            }
            conf = _conf;
            return false;
        }
        private static XmlNode createRootStub(XmlDocument conf, string header)
        {
            conf.AppendChild(conf.CreateNode(XmlNodeType.XmlDeclaration, "?xml version=\"1.0\" encoding=\"utf-8\" ?", ""));
            XmlNode rootNode = conf.CreateNode(XmlNodeType.Element, "control", "");
            conf.AppendChild(rootNode);
            XmlNode headerNode = conf.CreateNode(XmlNodeType.Element, "header", "");
            headerNode.InnerText = header;
            rootNode.AppendChild(headerNode);

            return rootNode;
        }
        private static XmlNode createCommonOptsStub(XmlDocument conf, XmlNode mountPoint)
        {
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
            mountPoint.AppendChild(commonNode);
            return commonNode;
        }
        private static XmlNode createPEDStub(XmlDocument pedConf, XmlNode mountPoint)
        {
            XmlNode senseNode = pedConf.CreateNode(XmlNodeType.Element, "sense", "");
            for (int i = 1; i <= 20; ++i)
            {
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