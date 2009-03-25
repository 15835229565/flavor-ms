using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Flavor
{
    public class PreciseEditorData
    {
        public PreciseEditorData(byte pn, ushort st, byte co, ushort it, ushort wi, float pr)
        {
            pointNumber = pn;
            step = st;
            collector = co;
            iterations = it;
            width = wi;
            precision = pr;
        }
        public PreciseEditorData(bool useit, byte pn, ushort st, byte co, ushort it, ushort wi, float pr, string comm)
        {
            usethis = useit;
            pointNumber = pn;
            step = st;
            collector = co;
            iterations = it;
            width = wi;
            precision = pr;
            comment = comm;
        }
        private bool usethis = true;
        private byte pointNumber;
        private ushort step;
        private byte collector;
        private ushort iterations;
        private ushort width;
        private float precision;
        private string comment = "";
        ZedGraph.PointPairList associatedPoints;
        public ZedGraph.PointPairList AssociatedPoints
        {
            get { return associatedPoints; }
            set { associatedPoints = value; }
        }
        public bool Use
        {
            get { return usethis; }
            //set { usethis = value; }
        }
        public byte pNumber
        {
            get { return pointNumber; }
            //set { pointNumber = value; }
        }
        public ushort Step
        {
            get { return step; }
            //set { step = value; }
        }
        public byte Collector
        {
            get { return collector; }
            //set { collector = value; }
        }
        public ushort Iterations
        {
            get { return iterations; }
            //set { iterations = value; }
        }
        public ushort Width
        {
            get { return width; }
            //set { width = value; }
        }
        public float Precision
        {
            get { return precision; }
            //set { precision = value; }
        }
        public string Comment
        {
            get { return comment; }
            //set { comment = value; }
        }
    }

    static class Config
    {
        private static XmlDocument _conf = new XmlDocument();
        private static string mainConfigPrefix = "/control/";

        private static string initialDir;
        private static string confName;

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
        
        private static List<PreciseEditorData> preciseData = new List<PreciseEditorData>();
        //private static List<PreciseEditorData> preciseDataLoaded = new List<PreciseEditorData>();

        public static List<PreciseEditorData> PreciseData
        {
            get { return preciseData; }
            //set { preciseData = value; }
        }
        /*
        public static List<PreciseEditorData> PreciseDataLoaded
        {
            get { return preciseDataLoaded; }
            //set { preciseData = value; }
        }
        */
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
                System.Windows.Forms.MessageBox.Show("Ошибка чтения конфигурационного файла", "Ошибка структуры конфигурационного файла");
            }
            loadCommonOptions();
            LoadPreciseEditorData();
        }

        private static void saveScanOptions()
        {
            _conf.SelectSingleNode("/control/overview/start").InnerText = sPoint.ToString();
            _conf.SelectSingleNode("/control/overview/end").InnerText = ePoint.ToString();
            _conf.Save(confName);
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
        
        internal static void OpenSpecterFile(string p)
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
                return;
            }
            try
            {
                Graph.ResetLoadedPointLists();
                foreach (XmlNode pntNode in sf.SelectNodes("/overview/collector1/p"))
                {
                    X = ushort.Parse(pntNode.SelectSingleNode("s").InnerText);
                    Y = int.Parse(pntNode.SelectSingleNode("c").InnerText);
                    Graph.updateLoaded1Graph(X, Y);
                }
                foreach (XmlNode pntNode in sf.SelectNodes("/overview/collector2/p"))
                {
                    X = ushort.Parse(pntNode.SelectSingleNode("s").InnerText);
                    Y = int.Parse(pntNode.SelectSingleNode("c").InnerText);
                    Graph.updateLoaded2Graph(X, Y);
                }
                Graph.updateLoaded();
            }
            catch (NullReferenceException)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка чтения файла спектра", "Ошибка структуры файла");
                return;
            }
        }
        internal static void SaveSpecterFile(string p, bool isFromFile)
        {
            XmlDocument sf = new XmlDocument();
            XmlNode temp;
            sf.AppendChild(sf.CreateNode(XmlNodeType.XmlDeclaration, "?xml version=\"1.0\" encoding=\"utf-8\" ?", ""));
            sf.AppendChild(sf.CreateNode(XmlNodeType.Element, "overview", ""));
            sf.SelectSingleNode("overview").AppendChild(sf.CreateNode(XmlNodeType.Element, "header", ""));
            sf.SelectSingleNode("overview").AppendChild(sf.CreateNode(XmlNodeType.Element, "start", ""));
            sf.SelectSingleNode("overview").AppendChild(sf.CreateNode(XmlNodeType.Element, "end", ""));
            sf.SelectSingleNode("overview").AppendChild(sf.CreateNode(XmlNodeType.Element, "collector1", ""));
            sf.SelectSingleNode("overview").AppendChild(sf.CreateNode(XmlNodeType.Element, "collector2", ""));
            sf.SelectSingleNode("/overview/start").InnerText = sPoint.ToString();
            sf.SelectSingleNode("/overview/end").InnerText = ePoint.ToString();
            if (isFromFile)
            {
                foreach (ZedGraph.PointPair pp in Graph.LoadedSpectra1Steps[0])
                {
                    temp = sf.CreateNode(XmlNodeType.Element, "p", "");
                    temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "s", "")).InnerText = pp.X.ToString();
                    temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "c", "")).InnerText = pp.Y.ToString();
                    sf.SelectSingleNode(string.Format("overview/collector1")).AppendChild(temp);
                }
                foreach (ZedGraph.PointPair pp in Graph.LoadedSpectra2Steps[0])
                {
                    temp = sf.CreateNode(XmlNodeType.Element, "p", "");
                    temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "s", "")).InnerText = pp.X.ToString();
                    temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "c", "")).InnerText = pp.Y.ToString();
                    sf.SelectSingleNode(string.Format("overview/collector2")).AppendChild(temp);
                }
            }
            else
            {
                foreach (ZedGraph.PointPair pp in Graph.Collector1Steps[0])
                {
                    temp = sf.CreateNode(XmlNodeType.Element, "p", "");
                    temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "s", "")).InnerText = pp.X.ToString();
                    temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "c", "")).InnerText = pp.Y.ToString();
                    sf.SelectSingleNode(string.Format("overview/collector1")).AppendChild(temp);
                }
                foreach (ZedGraph.PointPair pp in Graph.Collector2Steps[0])
                {
                    temp = sf.CreateNode(XmlNodeType.Element, "p", "");
                    temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "s", "")).InnerText = pp.X.ToString();
                    temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "c", "")).InnerText = pp.Y.ToString();
                    sf.SelectSingleNode(string.Format("overview/collector2")).AppendChild(temp);
                }
            }
            sf.Save(p);
        }
        internal static void AutoSaveSpecterFile()
        {
            SaveSpecterFile(@genAutoSaveFilename("sdf"), false);
        }

        internal static void OpenPreciseSpecterFile(string p)
        {
            XmlDocument sf = new XmlDocument();
            List<PreciseEditorData> peds = new List<PreciseEditorData>();
            ushort X = 0;
            int Y = 0;
            try
            {
                sf.Load(p);
            }
            catch (Exception Error)
            {
                System.Windows.Forms.MessageBox.Show(Error.Message, "Ошибка чтения файла прецизионного спектра");
                return;
            }
            for (int i = 1; i <= 20; ++i)
            {
                PreciseEditorData temp = null;
                try
                {
                    bool allFilled = ((sf.SelectSingleNode(string.Format("/sense/region{0}/peak", i)).InnerText != "") &&
                                      (sf.SelectSingleNode(string.Format("/sense/region{0}/iteration", i)).InnerText != "") &&
                                      (sf.SelectSingleNode(string.Format("/sense/region{0}/width", i)).InnerText != "") &&
                                      (sf.SelectSingleNode(string.Format("/sense/region{0}/col", i)).InnerText != ""));
                    if (allFilled)
                    {
                        temp = new PreciseEditorData((byte)(i - 1),
                                                     ushort.Parse(sf.SelectSingleNode(string.Format("/sense/region{0}/peak", i)).InnerText),
                                                     byte.Parse(sf.SelectSingleNode(string.Format("/sense/region{0}/col", i)).InnerText),
                                                     ushort.Parse(sf.SelectSingleNode(string.Format("/sense/region{0}/iteration", i)).InnerText),
                                                     ushort.Parse(sf.SelectSingleNode(string.Format("/sense/region{0}/width", i)).InnerText),
                                                     (float)0);

                        ZedGraph.PointPairList tempPntLst = new ZedGraph.PointPairList();
                        foreach (XmlNode pntNode in sf.SelectNodes(string.Format("/sense/region{0}/p", i)))
                        {
                            X = ushort.Parse(pntNode.SelectSingleNode("s").InnerText);
                            Y = int.Parse(pntNode.SelectSingleNode("c").InnerText);
                            tempPntLst.Add(X, Y);
                        }
                        temp.AssociatedPoints = tempPntLst;
                    }
                }
                catch (NullReferenceException)
                {
                    System.Windows.Forms.MessageBox.Show("Ошибка чтения файла прецизионного спектра", "Ошибка структуры файла");
                    return;
                }
                if (temp != null) peds.Add(temp);
            }
            Graph.updateGraph(peds);
        }
        internal static void SavePreciseSpecterFile(string p, bool isFromFile)
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
            if (isFromFile)
            {
                foreach (PreciseEditorData ped in Config.PreciseData/*Loaded*/)
                {
                    foreach (ZedGraph.PointPair pp in ped.AssociatedPoints)
                    {
                        temp = sf.CreateNode(XmlNodeType.Element, "p", "");
                        temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "s", "")).InnerText = pp.X.ToString();
                        temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "c", "")).InnerText = pp.Y.ToString();
                        sf.SelectSingleNode(string.Format("/sense/region{0}", ped.pNumber + 1)).AppendChild(temp);
                    }
                }
            }
            else
            {
                foreach (PreciseEditorData ped in Config.PreciseData)
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
            }
            sf.Save(p);
        }
        internal static void AutoSavePreciseSpecterFile()
        {
            SavePreciseSpecterFile(@genAutoSaveFilename("psf"), false);
        }

        internal static void SavePreciseOptions() 
        {
            SavePreciseOptions(Config.PreciseData, confName);
        }
        internal static void SavePreciseOptions(List<PreciseEditorData> ped)
        {
            preciseData = ped;
            SavePreciseOptions(ped, confName);
        }
        internal static void SavePreciseOptions(List<PreciseEditorData> ped, string pedConfName)
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

            foreach (PreciseEditorData p in ped)
            {
                pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/peak", p.pNumber + 1)).InnerText = p.Step.ToString();
                pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/iteration", p.pNumber + 1)).InnerText = p.Iterations.ToString();
                pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/width", p.pNumber + 1)).InnerText = p.Width.ToString();
                pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/error", p.pNumber + 1)).InnerText = p.Precision.ToString();
                pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/col", p.pNumber + 1)).InnerText = p.Collector.ToString();
                pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/comment", p.pNumber + 1)).InnerText = p.Comment;
                pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/use", p.pNumber + 1)).InnerText = p.Use.ToString();
            }
            pedConf.Save(pedConfName);
        }

        internal static List<PreciseEditorData> LoadPreciseEditorData(string pedConfName)
        {
            List<PreciseEditorData> ped = new List<PreciseEditorData>();
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
            for (int i = 1; i <= 20; ++i)
            {
                PreciseEditorData temp = null;
                string peak, iter, width, col;
                try
                {
                    peak = pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/peak", i)).InnerText;
                    col = pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/col", i)).InnerText;
                    iter = pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/iteration", i)).InnerText;
                    width = pedConf.SelectSingleNode(string.Format(mainConfPrefix + "sense/region{0}/width", i)).InnerText;
                }
                catch (NullReferenceException)
                {
                    if (!pedConfName.Equals(confName))
                        System.Windows.Forms.MessageBox.Show("Ошибка структуры файла", "Ошибка чтения файла прецизионных точек");
                    else
                        System.Windows.Forms.MessageBox.Show("Ошибка структуры файла", "Ошибка чтения конфигурационного файла");
                    return null;
                }
                if ((peak != "") && (iter != "") && (width != "") && (col != ""))
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
                        temp = new PreciseEditorData(use, (byte)(i - 1), ushort.Parse(peak),
                                                     byte.Parse(col), ushort.Parse(iter),
                                                     ushort.Parse(width), (float)0, comment);
                    }
                    catch (FormatException)
                    {
                        if (!pedConfName.Equals(confName))
                            System.Windows.Forms.MessageBox.Show("Неверный формат данных", "Ошибка чтения файла прецизионных точек");
                        else
                            System.Windows.Forms.MessageBox.Show("Неверный формат данных", "Ошибка чтения конфигурационного файла");
                        return null;
                    }
                }
                if (temp != null) ped.Add(temp);
            }
            return ped;
        }
        internal static void LoadPreciseEditorData()
        {
            List<PreciseEditorData> pedl = LoadPreciseEditorData(confName);
            if ((pedl != null) && (pedl.Count > 0)) 
            { 
                PreciseData.Clear();
                PreciseData.AddRange(pedl); 
            }
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
                if (!cdConfName.Equals(confName))
                    System.Windows.Forms.MessageBox.Show("Ошибка чтения файла общих настроек", "Ошибка структуры файла");
                else
                    System.Windows.Forms.MessageBox.Show("Ошибка чтения конфигурационного файла", "Ошибка структуры файла");
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

        internal static double pointToMass(ushort pnt, bool isFirstCollector)
        {
            //!!! Requires real law !!!
            double coeff = 896.5 * 18;
            if (isFirstCollector) coeff = 2770 * 28;
            return coeff / Config.scanVoltageReal(pnt);
        }
        //Comparers and predicate for sorting and finding PreciseEditorData objects in List
        internal static int ComparePreciseEditorDataByPeakValue(PreciseEditorData ped1, PreciseEditorData ped2)
        {
            //Forward sort
            if (ped1 == null)
            {
                if (ped2 == null)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (ped2 == null)
                    return 1;
                else
                    return (int)(ped1.Step - ped2.Step);
            }
        }
        internal static int ComparePreciseEditorDataByUseFlagAndPeakValue(PreciseEditorData ped1, PreciseEditorData ped2)
        {
            //Forward sort
            if ((ped1 == null) || !ped1.Use)
            {
                if ((ped2 == null) || !ped2.Use)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if ((ped2 == null) || !ped2.Use)
                    return 1;
                else
                    return (int)(ped1.Step - ped2.Step);
            }
        }
        internal static bool PeakIsUsed(PreciseEditorData ped)
        {
            return ped.Use;
        }
    }
}