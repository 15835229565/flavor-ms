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

        private static string SerialPort;
        private static ushort SerialBaudRate;
        private static byte SendTry;

        private static ushort startPoint;
        private static ushort endPoint;

        private static ushort expTime;
        private static ushort idleTime;
        private static ushort ionizationVoltage;
        private static ushort CPVoltage;
        private static ushort heatCurrent;
        private static ushort emissionCurrent;
        private static ushort focusVoltage1;
        private static ushort focusVoltage2;
        
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

        public static ushort eTime
        {
            get { return expTime; }
            set { expTime = value; }
        }
        public static ushort eTimeReal
        {
            get { return (ushort)(eTime * 5); }
            set
            {
                eTime = (ushort)(value / 5);
            }
        }

        public static ushort iTime
        {
            get { return idleTime; }
            set { idleTime = value; }
        }
        public static ushort iTimeReal
        {
            get { return (ushort)(5 * iTime); }
            set
            {
                iTime = (ushort)(value / 5);
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

        internal static void LoadConfig()
        {
            try
            {
                _conf.Load(confName);
            }
            catch (Exception Error)
            {
                System.Windows.Forms.MessageBox.Show(Error.Message, "Ошибка чтения конфигурационного файла");
            }

            try
            {
                SerialPort = (_conf.SelectSingleNode("/control/connect/port").InnerText);
                SerialBaudRate = ushort.Parse(_conf.SelectSingleNode("/control/connect/baudrate").InnerText);
                SendTry = byte.Parse(_conf.SelectSingleNode("/control/connect/try").InnerText);
                sPoint = ushort.Parse(_conf.SelectSingleNode("/control/overview/start").InnerText);
                ePoint = ushort.Parse(_conf.SelectSingleNode("/control/overview/end").InnerText);
            }
            catch (NullReferenceException)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка структуры конфигурационного файла", "Ошибка чтения конфигурационного файла");
            }
            loadCommonOptions();
            LoadPreciseEditorData();
        }

        private static void saveScanOptions()
        {
            try
            {
                _conf.SelectSingleNode("/control/overview/start").InnerText = sPoint.ToString();
            }
            catch (NullReferenceException)
            {
                System.Windows.Forms.MessageBox.Show("Невозможно найти начальное значение напряжения развертки при сканировании", "Ошибка структуры конфигурационного файла");
            }
            try
            {
                _conf.SelectSingleNode("/control/overview/end").InnerText = ePoint.ToString();
            }
            catch (NullReferenceException)
            {
                System.Windows.Forms.MessageBox.Show("Невозможно найти конечное значение напряжения развертки при сканировании", "Ошибка структуры конфигурационного файла");
            }
            _conf.Save(@confName);
        }
        internal static void saveScanOptions(ushort sPointReal, ushort ePointReal)
        {
            Config.sPoint = sPointReal;//!!!
            Config.ePoint = ePointReal;//!!!
            Config.saveScanOptions();
        }
        
        private static void SaveConnectOptions()
        {
            try
            {
                _conf.SelectSingleNode("/control/connect/port").InnerText = Config.Port;
            }
            catch (NullReferenceException)
            {
                System.Windows.Forms.MessageBox.Show("Невозможно найти имя последовательного порта", "Ошибка структуры конфигурационного файла");
            }
            try
            {
                _conf.SelectSingleNode("/control/connect/baudrate").InnerText = Config.BaudRate.ToString();
            }
            catch (NullReferenceException)
            {
                System.Windows.Forms.MessageBox.Show("Невозможно найти настройки скорости соединения", "Ошибка структуры конфигурационного файла");
            }
            _conf.Save(@confName);
        }
        internal static void SaveConnectOptions(string port, ushort baudrate)
        {
            Config.Port = port;
            Config.BaudRate = baudrate;
            Config.SaveConnectOptions();
        }

        internal static void SaveAll()
        {
            Config.SaveConnectOptions();
            Config.saveScanOptions();
            Config.saveCommonOptions();
            Config.SavePreciseOptions();
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
        
        internal static bool OpenSpecterFile(string p)
        {
            ZedGraph.PointPairList pl1 = new ZedGraph.PointPairList(), pl2 = new ZedGraph.PointPairList();
            bool result = OpenSpecterFile(p, pl1, pl2);
            if (result)
            {
                Graph.ResetLoadedPointLists();
                Graph.updateLoaded(pl1, pl2);
            }
            return result;
        }
        internal static bool OpenSpecterFile(string p, ZedGraph.PointPairList pl1, ZedGraph.PointPairList pl2)
        {
            XmlDocument sf = new XmlDocument();
            ushort X = 0;
            int Y = 0;
            try
            {
                sf.Load(p);
            }
            catch (Exception Error)
            {
                System.Windows.Forms.MessageBox.Show(Error.Message, "Ошибка чтения файла спектра");
                return false;
            }
            try
            {
                foreach (XmlNode pntNode in sf.SelectNodes("/overview/collector1/p"))
                {
                    X = ushort.Parse(pntNode.SelectSingleNode("s").InnerText);
                    Y = int.Parse(pntNode.SelectSingleNode("c").InnerText);
                    pl1.Add(X, Y);
                }
                foreach (XmlNode pntNode in sf.SelectNodes("/overview/collector2/p"))
                {
                    X = ushort.Parse(pntNode.SelectSingleNode("s").InnerText);
                    Y = int.Parse(pntNode.SelectSingleNode("c").InnerText);
                    pl2.Add(X, Y);
                }
            }
            catch (NullReferenceException)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка структуры файла", "Ошибка чтения файла спектра");
                return false;
            }
            pl1.Sort(ZedGraph.SortType.XValues);
            pl2.Sort(ZedGraph.SortType.XValues);
            return true;
        }
        internal static void SaveSpecterFile(string p, Graph.Displaying displayMode)
        {
            XmlDocument sf = new XmlDocument();
            XmlNode temp;
            sf.AppendChild(sf.CreateNode(XmlNodeType.XmlDeclaration, "?xml version=\"1.0\" encoding=\"utf-8\" ?", ""));
            sf.AppendChild(sf.CreateNode(XmlNodeType.Element, "overview", ""));
            sf.SelectSingleNode("overview").AppendChild(sf.CreateNode(XmlNodeType.Element, "header", ""));
            switch (displayMode)
            {
                case Graph.Displaying.Loaded:
                    sf.SelectSingleNode("/overview/header").InnerText = "Custom save";
                    break;
                case Graph.Displaying.Measured:
                    sf.SelectSingleNode("overview").AppendChild(sf.CreateNode(XmlNodeType.Element, "start", ""));
                    sf.SelectSingleNode("overview").AppendChild(sf.CreateNode(XmlNodeType.Element, "end", ""));
                    sf.SelectSingleNode("/overview/header").InnerText = "Measure save";
                    sf.SelectSingleNode("/overview/start").InnerText = sPoint.ToString();
                    sf.SelectSingleNode("/overview/end").InnerText = ePoint.ToString();
                    // In case of loaded (not auto) start/end points and measure parameters are not connected to spectrum data..
                    break;
                case Graph.Displaying.Diff:
                    sf.SelectSingleNode("/overview/header").InnerText = "Diff save";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            sf.SelectSingleNode("overview").AppendChild(sf.CreateNode(XmlNodeType.Element, "collector1", ""));
            sf.SelectSingleNode("overview").AppendChild(sf.CreateNode(XmlNodeType.Element, "collector2", ""));
            foreach (ZedGraph.PointPair pp in Graph.Displayed1Steps[0])
            {
                temp = sf.CreateNode(XmlNodeType.Element, "p", "");
                temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "s", "")).InnerText = pp.X.ToString();
                temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "c", "")).InnerText = pp.Y.ToString();
                sf.SelectSingleNode(string.Format("overview/collector1")).AppendChild(temp);
            }
            foreach (ZedGraph.PointPair pp in Graph.Displayed2Steps[0])
            {
                temp = sf.CreateNode(XmlNodeType.Element, "p", "");
                temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "s", "")).InnerText = pp.X.ToString();
                temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "c", "")).InnerText = pp.Y.ToString();
                sf.SelectSingleNode(string.Format("overview/collector2")).AppendChild(temp);
            }
            sf.Save(@p);
        }
        internal static void AutoSaveSpecterFile()
        {
            SaveSpecterFile(genAutoSaveFilename("sdf"), Graph.Displaying.Measured);
        }

        internal static void DistractSpectra(string from, string what)
        {
            ZedGraph.PointPairList pl11 = new ZedGraph.PointPairList();
            ZedGraph.PointPairList pl21 = new ZedGraph.PointPairList();
            ZedGraph.PointPairList pl12 = new ZedGraph.PointPairList();
            ZedGraph.PointPairList pl22 = new ZedGraph.PointPairList();
            if (OpenSpecterFile(from, pl11, pl21) && OpenSpecterFile(what, pl12, pl22))
            {
                try
                {
                    ZedGraph.PointPairList diff1 = PointPairListDiff(pl11, pl12);
                    ZedGraph.PointPairList diff2 = PointPairListDiff(pl21, pl22);
                    Graph.updateNotPrecise(diff1, diff2);
                }
                catch (System.ArgumentException)
                {
                    System.Windows.Forms.MessageBox.Show("Несовпадение рядов данных", "Ошибка при вычитании спектров");
                    return;
                }
            }
        }
        internal static void DistractSpectra(string what)
        {
            if (Graph.isPreciseSpectrum)
            {
                List<Utility.PreciseEditorData> peds = new List<Utility.PreciseEditorData>();
                if (OpenPreciseSpecterFile(what, peds))
                {
                    peds.Sort(Utility.ComparePreciseEditorData);
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
                            temp = new List<Utility.PreciseEditorData>(preciseDataDiff);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    temp.Sort(Utility.ComparePreciseEditorData);
                    try
                    {
                        temp = PreciseEditorDataListDiff(temp, peds);
                        preciseDataDiff = temp;
                        Graph.updatePrecise(temp);
                    }
                    catch (System.ArgumentException)
                    {
                        System.Windows.Forms.MessageBox.Show("Несовпадение рядов данных", "Ошибка при вычитании спектров");
                        return;
                    }
                }
            }
            else
            {
                ZedGraph.PointPairList pl12 = new ZedGraph.PointPairList();
                ZedGraph.PointPairList pl22 = new ZedGraph.PointPairList();
                if (OpenSpecterFile(what, pl12, pl22))
                {
                    try
                    {
                        ZedGraph.PointPairList diff1 = PointPairListDiff(Graph.Displayed1Steps[0], pl12);
                        ZedGraph.PointPairList diff2 = PointPairListDiff(Graph.Displayed2Steps[0], pl22);
                        Graph.ResetDiffPointLists();
                        Graph.updateNotPrecise(diff1, diff2);
                    }
                    catch (System.ArgumentException)
                    {
                        System.Windows.Forms.MessageBox.Show("Несовпадение рядов данных", "Ошибка при вычитании спектров");
                        return;
                    }
                }
            }
        }
        private static ZedGraph.PointPairList PointPairListDiff(ZedGraph.PointPairList from, ZedGraph.PointPairList what)
        {
            if (from.Count != what.Count)
                throw new System.ArgumentOutOfRangeException();
            ZedGraph.PointPairList res = new ZedGraph.PointPairList(from);
            for (int i = 0; i < res.Count; ++i)
            {
                if (res[i].X != what[i].X)
                    throw new System.ArgumentException();
                res[i].Y -= what[i].Y;
            }
            return res;
        }
        private static Utility.PreciseEditorData PreciseEditorDataDiff(Utility.PreciseEditorData from, Utility.PreciseEditorData what)
        {
            if (!from.Equals(what))
                throw new System.ArgumentException();
            if (from.AssociatedPoints.Count != what.AssociatedPoints.Count)
                throw new System.ArgumentOutOfRangeException();
            Utility.PreciseEditorData res = new Utility.PreciseEditorData(from);
            for (int i = 0; i < res.AssociatedPoints.Count; ++i)
            {
                if (res.AssociatedPoints[i].X != what.AssociatedPoints[i].X)
                    throw new System.ArgumentException();
                res.AssociatedPoints[i].Y -= what.AssociatedPoints[i].Y;
            }
            return res;
        }
        private static List<Utility.PreciseEditorData> PreciseEditorDataListDiff(List<Utility.PreciseEditorData> from, List<Utility.PreciseEditorData> what)
        {
            if (from.Count != what.Count)
                throw new System.ArgumentOutOfRangeException();
            List<Utility.PreciseEditorData> res = new List<Utility.PreciseEditorData>(from);
            for (int i = 0; i < res.Count; ++i)
            {
                res[i] = PreciseEditorDataDiff(res[i], what[i]);
            }
            return res;
        }

        internal static bool OpenPreciseSpecterFile(string p)
        {
            List<Utility.PreciseEditorData> peds = new List<Utility.PreciseEditorData>();
            bool result = OpenPreciseSpecterFile(p, peds);
            if (result)
            {
                preciseDataLoaded = peds;
                Graph.updatePrecise();
            }
            return result;
        }
        internal static bool OpenPreciseSpecterFile(string p, List<Utility.PreciseEditorData> peds)
        {
            XmlDocument sf = new XmlDocument();
            try
            {
                sf.Load(p);
            }
            catch (Exception Error)
            {
                System.Windows.Forms.MessageBox.Show(Error.Message, "Ошибка чтения файла прецизионного спектра");
                return false;
            }
            return LoadPED(sf, p, peds, true, "");
        }
        internal static void SavePreciseSpecterFile(string p, Graph.Displaying displayMode)
        {
            XmlDocument sf = new XmlDocument();
            XmlNode temp;
            sf.AppendChild(sf.CreateNode(XmlNodeType.XmlDeclaration, "?xml version=\"1.0\" encoding=\"utf-8\" ?", ""));
            sf.AppendChild(sf.CreateNode(XmlNodeType.Element, "sense", ""));
            sf.SelectSingleNode("sense").AppendChild(sf.CreateNode(XmlNodeType.Element, "header", ""));
            for (int i = 1; i <= 20; ++i)
            {
                temp = sf.CreateNode(XmlNodeType.Element, string.Format("region{0}", i), "");
                temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "peak", ""));
                temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "col", ""));
                temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "iteration", ""));
                temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "width", ""));
                temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "error", ""));
                sf.SelectSingleNode(string.Format("sense")).AppendChild(temp);
            }
            List<Utility.PreciseEditorData> processed;
            switch (displayMode)
            {
                case Graph.Displaying.Loaded:
                    processed = Config.PreciseDataLoaded;
                    break;
                case Graph.Displaying.Measured:
                    processed = Config.PreciseData;
                    break;
                case Graph.Displaying.Diff:
                    processed = Config.PreciseDataDiff;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            foreach (Utility.PreciseEditorData ped in processed)
            {
                sf.SelectSingleNode(string.Format("/sense/region{0}/peak", ped.pNumber + 1)).InnerText = ped.Step.ToString();
                sf.SelectSingleNode(string.Format("/sense/region{0}/iteration", ped.pNumber + 1)).InnerText = ped.Iterations.ToString();
                sf.SelectSingleNode(string.Format("/sense/region{0}/width", ped.pNumber + 1)).InnerText = ped.Width.ToString();
                sf.SelectSingleNode(string.Format("/sense/region{0}/error", ped.pNumber + 1)).InnerText = ped.Precision.ToString();
                sf.SelectSingleNode(string.Format("/sense/region{0}/col", ped.pNumber + 1)).InnerText = ped.Collector.ToString();
                foreach (ZedGraph.PointPair pp in ped.AssociatedPoints)
                {
                    temp = sf.CreateNode(XmlNodeType.Element, "p", "");
                    temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "s", "")).InnerText = pp.X.ToString();
                    temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "c", "")).InnerText = pp.Y.ToString();
                    sf.SelectSingleNode(string.Format("/sense/region{0}", ped.pNumber + 1)).AppendChild(temp);
                }
            }
            sf.Save(@p);
        }
        internal static void AutoSavePreciseSpecterFile()
        {
            SavePreciseSpecterFile(genAutoSaveFilename("psf"), Graph.Displaying.Measured);
        }

        internal static void SavePreciseOptions() 
        {
            SavePreciseOptions(Config.PreciseData, confName);
        }
        internal static void SavePreciseOptions(List<Utility.PreciseEditorData> ped)
        {
            preciseData = ped;
            SavePreciseOptions(ped, confName);
        }
        internal static void SavePreciseOptions(List<Utility.PreciseEditorData> ped, string pedConfName)
        {
            XmlDocument pedConf;
            string mainConfPrefix = "";

            if (!pedConfName.Equals(confName))
            {
                pedConf = new XmlDocument();
                pedConf.AppendChild(pedConf.CreateNode(XmlNodeType.XmlDeclaration, "?xml version=\"1.0\" encoding=\"utf-8\" ?", ""));
                pedConf.AppendChild(pedConf.CreateNode(XmlNodeType.Element, "sense", ""));
                pedConf.SelectSingleNode("sense").AppendChild(pedConf.CreateNode(XmlNodeType.Element, "header", ""));
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
                    pedConf.SelectSingleNode(string.Format("sense")).AppendChild(tempRegion);
                }
            }
            else
            {
                for (int i = 1; i <= 20; ++i)
                {
                    _conf.SelectSingleNode(string.Format("/control/sense/region{0}/peak", i)).InnerText = "";
                    _conf.SelectSingleNode(string.Format("/control/sense/region{0}/iteration", i)).InnerText = "";
                    _conf.SelectSingleNode(string.Format("/control/sense/region{0}/width", i)).InnerText = "";
                    _conf.SelectSingleNode(string.Format("/control/sense/region{0}/error", i)).InnerText = "";
                    try
                    {
                        _conf.SelectSingleNode(string.Format("/control/sense/region{0}/col", i)).InnerText = "";
                    }
                    catch (NullReferenceException)
                    {
                        _conf.SelectSingleNode(string.Format("/control/sense/region{0}", i)).AppendChild(_conf.CreateNode(XmlNodeType.Element, "col", ""));
                    }
                    try 
                    {
                        _conf.SelectSingleNode(string.Format("/control/sense/region{0}/comment", i)).InnerText = "";
                    }
                    catch (NullReferenceException)
                    {
                        _conf.SelectSingleNode(string.Format("/control/sense/region{0}", i)).AppendChild(_conf.CreateNode(XmlNodeType.Element, "comment", ""));
                    }
                    try
                    {
                        _conf.SelectSingleNode(string.Format("/control/sense/region{0}/use", i)).InnerText = "";
                    }
                    catch (NullReferenceException)
                    {
                        _conf.SelectSingleNode(string.Format("/control/sense/region{0}", i)).AppendChild(_conf.CreateNode(XmlNodeType.Element, "use", ""));
                    }
                }

                pedConf = _conf;
                mainConfPrefix = mainConfigPrefix;
            }

            foreach (Utility.PreciseEditorData p in ped)
            {
                pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/peak", p.pNumber + 1)).InnerText = p.Step.ToString();
                pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/iteration", p.pNumber + 1)).InnerText = p.Iterations.ToString();
                pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/width", p.pNumber + 1)).InnerText = p.Width.ToString();
                pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/error", p.pNumber + 1)).InnerText = p.Precision.ToString();
                pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/col", p.pNumber + 1)).InnerText = p.Collector.ToString();
                pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/comment", p.pNumber + 1)).InnerText = p.Comment;
                pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/use", p.pNumber + 1)).InnerText = p.Use.ToString();
            }
            pedConf.Save(@pedConfName);
        }

        internal static List<Utility.PreciseEditorData> LoadPreciseEditorData(string pedConfName)
        {
            List<Utility.PreciseEditorData> ped = new List<Utility.PreciseEditorData>();
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
                    System.Windows.Forms.MessageBox.Show(Error.Message, "Ошибка чтения файла прецизионных точек");
                    return null;
                }
            }
            else 
            {
                pedConf = _conf;
                mainConfPrefix = mainConfigPrefix;
            }

            if (LoadPED(pedConf, pedConfName, ped, false, mainConfPrefix))
                return ped;
            return null;
        }
        internal static void LoadPreciseEditorData()
        {
            List<Utility.PreciseEditorData> pedl = LoadPreciseEditorData(confName);
            if ((pedl != null) && (pedl.Count > 0)) 
            { 
                //BAD!!! cleaning previous points!!!
                preciseData.Clear();
                preciseData.AddRange(pedl); 
            }
        }

        private static bool LoadPED(XmlDocument pedConf, string pedConfName, List<Utility.PreciseEditorData> ped, bool readSpectrum, string mainConfPrefix)
        {
            for (int i = 1; i <= 20; ++i)
            {
                Utility.PreciseEditorData temp = null;
                string peak, iter, width, col;
                try
                {
                    peak = pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/peak", i)).InnerText;
                    col = pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/col", i)).InnerText;
                    iter = pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/iteration", i)).InnerText;
                    width = pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/width", i)).InnerText;
                    bool allFilled = ((peak != "") && (iter != "") && (width != "") && (col != ""));
                    if (allFilled)
                    {
                        string comment = "";
                        try
                        {
                            comment = pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/comment", i)).InnerText;
                        }
                        catch (NullReferenceException) { }
                        bool use = true;
                        try
                        {
                            use = bool.Parse(pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/use", i)).InnerText);
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
                                System.Windows.Forms.MessageBox.Show("Неверный формат данных", "Ошибка чтения файла прецизионного спектра");
                            else
                                wrongFormatOnLoadPrecise(confName);
                            return false;
                        }
                        if (readSpectrum)
                        {
                            int X, Y;
                            ZedGraph.PointPairList tempPntLst = new ZedGraph.PointPairList();
                            try
                            {
                                foreach (XmlNode pntNode in pedConf.SelectNodes(string.Format(mainConfPrefix + "sense/region{0}/p", i)))
                                {
                                    X = ushort.Parse(pntNode.SelectSingleNode("s").InnerText);
                                    Y = int.Parse(pntNode.SelectSingleNode("c").InnerText);
                                    tempPntLst.Add(X, Y);
                                }
                            }
                            catch (FormatException)
                            {
                                System.Windows.Forms.MessageBox.Show("Неверный формат данных", "Ошибка чтения файла прецизионного спектра");
                                return false;
                            }
                            temp.AssociatedPoints = tempPntLst;
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    if (readSpectrum)
                        System.Windows.Forms.MessageBox.Show("Ошибка структуры файла", "Ошибка чтения файла прецизионного спектра");
                    else
                        structureErrorOnLoadPrecise(pedConfName);
                    return false;
                }
                if (temp != null) ped.Add(temp);
            }
            return true;
        }

        internal static void saveCommonOptions()
        {
            saveCommonOptions(confName);
        }
        internal static void saveCommonOptions(ushort eT, ushort iT, double iV, double cp, double eC, double hC, double fv1, double fv2)
        {
            saveCommonOptions(confName, eT, iT, iV, cp, eC, hC, fv1, fv2);
        }
        internal static void saveCommonOptions(string fn, ushort eT, ushort iT, double iV, double cp, double eC, double hC, double fv1, double fv2)
        {
            Config.eTimeReal = eT;
            Config.iTimeReal = iT;
            Config.iVoltageReal = iV;
            Config.CPReal = cp;
            Config.eCurrentReal = eC;
            Config.hCurrentReal = hC;
            Config.fV1Real = fv1;
            Config.fV2Real = fv2;

            saveCommonOptions(fn);
        }
        internal static void saveCommonOptions(string fn)
        {
            XmlDocument cdConf;
            string mainConfPrefix = "";

            if (!fn.Equals(confName))
            {
                cdConf = new XmlDocument();
                cdConf.AppendChild(cdConf.CreateNode(XmlNodeType.XmlDeclaration, "?xml version=\"1.0\" encoding=\"utf-8\" ?", ""));
                cdConf.AppendChild(cdConf.CreateNode(XmlNodeType.Element, "common", ""));
                cdConf.SelectSingleNode("common").AppendChild(cdConf.CreateNode(XmlNodeType.Element, "header", ""));
                cdConf.SelectSingleNode("common").AppendChild(cdConf.CreateNode(XmlNodeType.Element, "exptime", ""));
                cdConf.SelectSingleNode("common").AppendChild(cdConf.CreateNode(XmlNodeType.Element, "meastime", ""));
                cdConf.SelectSingleNode("common").AppendChild(cdConf.CreateNode(XmlNodeType.Element, "ivoltage", ""));
                cdConf.SelectSingleNode("common").AppendChild(cdConf.CreateNode(XmlNodeType.Element, "cp", ""));
                cdConf.SelectSingleNode("common").AppendChild(cdConf.CreateNode(XmlNodeType.Element, "ecurrent", ""));
                cdConf.SelectSingleNode("common").AppendChild(cdConf.CreateNode(XmlNodeType.Element, "hcurrent", ""));
                cdConf.SelectSingleNode("common").AppendChild(cdConf.CreateNode(XmlNodeType.Element, "focus1", ""));
                cdConf.SelectSingleNode("common").AppendChild(cdConf.CreateNode(XmlNodeType.Element, "focus2", ""));
            }
            else 
            {
                cdConf = _conf;
                mainConfPrefix = mainConfigPrefix;
            }
            
            cdConf.SelectSingleNode(mainConfPrefix + "common/exptime").InnerText = Config.eTime.ToString();
            cdConf.SelectSingleNode(mainConfPrefix + "common/meastime").InnerText = Config.iTime.ToString();
            cdConf.SelectSingleNode(mainConfPrefix + "common/ivoltage").InnerText = Config.iVoltage.ToString();
            cdConf.SelectSingleNode(mainConfPrefix + "common/cp").InnerText = Config.CP.ToString();
            cdConf.SelectSingleNode(mainConfPrefix + "common/ecurrent").InnerText = Config.eCurrent.ToString();
            cdConf.SelectSingleNode(mainConfPrefix + "common/hcurrent").InnerText = Config.hCurrent.ToString();
            cdConf.SelectSingleNode(mainConfPrefix + "common/focus1").InnerText = Config.fV1.ToString();
            cdConf.SelectSingleNode(mainConfPrefix + "common/focus2").InnerText = Config.fV2.ToString();
            
            cdConf.Save(fn);
        }

        internal static void loadCommonOptions() 
        {
            loadCommonOptions(confName);
        }
        internal static void loadCommonOptions(string cdConfName)
        {
            XmlDocument cdConf;
            string mainConfPrefix = "";

            if (!cdConfName.Equals(confName))
            {
                cdConf = new XmlDocument();
                try
                {
                    cdConf.Load(cdConfName);
                }
                catch (Exception Error)
                {
                    System.Windows.Forms.MessageBox.Show(Error.Message, "Ошибка чтения файла общих настроек");
                    return;
                }
            }
            else
            {
                cdConf = _conf;
                mainConfPrefix = mainConfigPrefix;
            }
            ushort eT, iT, iV, cp, eC, hC, fv1, fv2;
            try
            {
                eT = ushort.Parse(cdConf.SelectSingleNode(mainConfPrefix + "common/exptime").InnerText);
                iT = ushort.Parse(cdConf.SelectSingleNode(mainConfPrefix + "common/meastime").InnerText);
                iV = ushort.Parse(cdConf.SelectSingleNode(mainConfPrefix + "common/ivoltage").InnerText);
                cp = ushort.Parse(cdConf.SelectSingleNode(mainConfPrefix + "common/cp").InnerText);
                eC = ushort.Parse(cdConf.SelectSingleNode(mainConfPrefix + "common/ecurrent").InnerText);
                hC = ushort.Parse(cdConf.SelectSingleNode(mainConfPrefix + "common/hcurrent").InnerText);
                fv1 = ushort.Parse(cdConf.SelectSingleNode(mainConfPrefix + "common/focus1").InnerText);
                fv2 = ushort.Parse(cdConf.SelectSingleNode(mainConfPrefix + "common/focus2").InnerText);
            }
            catch (NullReferenceException)
            {
                structureErrorOnLoadCommonData(cdConfName);
                return;
            }
            eTime = eT;
            iTime = iT;
            iVoltage = iV;
            CP = cp;
            eCurrent = eC;
            hCurrent = hC;
            fV1 = fv1;
            fV2 = fv2;
        }
        //Error messages on loading different configs
        private static void wrongFormatOnLoadPrecise(string configName)
        {
            wrongFormatOnLoad(configName, "Ошибка чтения файла прецизионных точек");
        }
        private static void wrongFormatOnLoad(string configName, string errorFile)
        {
            errorOnLoad(configName, errorFile, "Неверный формат данных");
        }
        private static void structureErrorOnLoadCommonData(string configName)
        {
            structureErrorOnLoad(configName, "Ошибка чтения файла общих настроек");
        }
        private static void structureErrorOnLoadPrecise(string configName)
        {
            structureErrorOnLoad(configName, "Ошибка чтения файла прецизионных точек");
        }
        private static void structureErrorOnLoad(string configName, string errorFile)
        {
            errorOnLoad(configName, errorFile, "Ошибка структуры файла");
        }
        private static void errorOnLoad(string configName, string errorFile, string errorMessage)
        {
            if (!configName.Equals(confName))
                System.Windows.Forms.MessageBox.Show(errorMessage, errorFile);
            else
                System.Windows.Forms.MessageBox.Show(errorMessage, "Ошибка чтения конфигурационного файла");
        }

        private static double col1Coeff = 2770 * 28;
        private static double col2Coeff = 896.5 * 18;
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

        internal static void logCrash(byte[] commandline)
        {
            System.IO.StreamWriter errorLog;
            try
            {
                errorLog = new System.IO.StreamWriter(@logName, true);
                errorLog.AutoFlush = true;
                DateTime now = System.DateTime.Now;
                errorLog.WriteLine(string.Format("{0}-{1}-{2}|", now.Year, now.Month, now.Day) + 
                    string.Format("{0}.{1}.{2}.{3}: ", now.Hour, now.Minute, now.Second, now.Millisecond) +
                    commandline.ToString());
                errorLog.Close();
            }
            catch (Exception Error)
            {
                string message = "Ошибка записи файла отказов";
                string cause = "(" + commandline.ToString() + ") -- " + Error.Message;
                Console.WriteLine(message + cause);
                System.Windows.Forms.MessageBox.Show(cause, message);
            }
        }
    }
}