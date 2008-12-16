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

        byte pointNumber;
        ushort step;
        byte collector;
        ushort iterations;
        ushort width;
        float precision;
        ZedGraph.PointPairList associatedPoints;

        public ZedGraph.PointPairList AssociatedPoints
        {
            get { return associatedPoints; }
            set { associatedPoints = value; }
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
    }

    static class Config
    {
        private static XmlDocument _conf = new XmlDocument();

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
        private static List<PreciseEditorData> preciseDataLoaded = new List<PreciseEditorData>();

        public static List<PreciseEditorData> PreciseData
        {
            get { return preciseData; }
            //set { preciseData = value; }
        }

        public static List<PreciseEditorData> PreciseDataLoaded
        {
            get { return preciseDataLoaded; }
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
            //return Convert.ToUInt16(4095 * Math.Pow(((double)527 / (double)528), step));
            if (step <= 456) return (ushort)(4095 - 5 * step);
            return (ushort)(4095 - 5 * 456 - 2 * (step - 456));
        }

        public static double scanVoltageReal(ushort step)
        {
            return (double)(scanVoltage(step) * 5 * 600) / 4096;
        }

        public static void getInitialDirectory()
        {
            initialDir = System.IO.Directory.GetCurrentDirectory();
            confName = initialDir + "\\config.xml";
        }

        public static void LoadConfig()
        {
            try
            {
                _conf.Load(confName);
            }
            catch (Exception Error)
            {
                System.Windows.Forms.MessageBox.Show(Error.Message, "Ошибка чтения конфигурационного файла");
                //throw new System.IO.FileNotFoundException();            
            }

            try
            {
                SerialPort = (_conf.SelectSingleNode("/control/connect/port").InnerText);
                SerialBaudRate = ushort.Parse(_conf.SelectSingleNode("/control/connect/baudrate").InnerText);
                SendTry = byte.Parse(_conf.SelectSingleNode("/control/connect/try").InnerText);
                sPoint = ushort.Parse(_conf.SelectSingleNode("/control/overview/start").InnerText);
                ePoint = ushort.Parse(_conf.SelectSingleNode("/control/overview/end").InnerText);
                eTime = ushort.Parse(_conf.SelectSingleNode("/control/common/exptime").InnerText);
                iTime = ushort.Parse(_conf.SelectSingleNode("/control/common/meastime").InnerText);
                iVoltage = ushort.Parse(_conf.SelectSingleNode("/control/common/ivoltage").InnerText);
                CP = ushort.Parse(_conf.SelectSingleNode("/control/common/cp").InnerText);
                eCurrent = ushort.Parse(_conf.SelectSingleNode("/control/common/ecurrent").InnerText);
                hCurrent = ushort.Parse(_conf.SelectSingleNode("/control/common/hcurrent").InnerText);
                fV1 = ushort.Parse(_conf.SelectSingleNode("/control/common/focus1").InnerText);
                fV2 = ushort.Parse(_conf.SelectSingleNode("/control/common/focus2").InnerText);
            }
            catch (NullReferenceException)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка чтения конфигурационного файла", "Ошибка структуры конфигурационного файла");
            }
            //LoadPreciseEditorData();
        }

        public static void SaveScanOptions()
        {
            _conf.SelectSingleNode("/control/overview/start").InnerText = sPoint.ToString();
            _conf.SelectSingleNode("/control/overview/end").InnerText = ePoint.ToString();
            _conf.SelectSingleNode("/control/common/exptime").InnerText = eTime.ToString();
            _conf.SelectSingleNode("/control/common/meastime").InnerText = iTime.ToString();
            _conf.SelectSingleNode("/control/common/ivoltage").InnerText = iVoltage.ToString();
            _conf.SelectSingleNode("/control/common/cp").InnerText = CP.ToString();
            _conf.SelectSingleNode("/control/common/ecurrent").InnerText = eCurrent.ToString();
            _conf.SelectSingleNode("/control/common/hcurrent").InnerText = hCurrent.ToString();
            _conf.SelectSingleNode("/control/common/focus1").InnerText = fV1.ToString();
            _conf.SelectSingleNode("/control/common/focus2").InnerText = fV2.ToString();

            _conf.Save(confName);
        }

        public static void SaveScanOptions(ushort sPointReal, ushort ePointReal, ushort eTimeReal, ushort mTimeReal, double iVoltageReal, double CPReal, double eCurrentReal, double hCurrentReal, double fV1Real, double fV2Real)
        {
            Config.sPoint = sPointReal;//!!!
            Config.ePoint = ePointReal;//!!!
            Config.eTimeReal = eTimeReal;
            Config.iTimeReal = mTimeReal;
            Config.iVoltageReal = iVoltageReal;
            Config.CPReal = CPReal;
            Config.eCurrentReal = eCurrentReal;
            Config.hCurrentReal = hCurrentReal;
            Config.fV1Real = fV1Real;
            Config.fV2Real = fV2Real;
            Config.SaveScanOptions();
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

        public static void SaveConnectOptions(string port, ushort baudrate)
        {
            Config.Port = port;
            Config.BaudRate = baudrate;
            Config.SaveConnectOptions();
        }

        internal static void SaveAll()
        {
            Config.SaveConnectOptions();
            Config.SaveScanOptions();
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
                foreach (ZedGraph.PointPair pp in Graph.pointListLoaded1)
                {
                    temp = sf.CreateNode(XmlNodeType.Element, "p", "");
                    temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "s", "")).InnerText = pp.X.ToString();
                    temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "c", "")).InnerText = pp.Y.ToString();
                    sf.SelectSingleNode(string.Format("overview/collector1")).AppendChild(temp);
                }
                foreach (ZedGraph.PointPair pp in Graph.pointListLoaded2)
                {
                    temp = sf.CreateNode(XmlNodeType.Element, "p", "");
                    temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "s", "")).InnerText = pp.X.ToString();
                    temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "c", "")).InnerText = pp.Y.ToString();
                    sf.SelectSingleNode(string.Format("overview/collector2")).AppendChild(temp);
                }
            }
            else
            {
                foreach (ZedGraph.PointPair pp in Graph.pointList1)
                {
                    temp = sf.CreateNode(XmlNodeType.Element, "p", "");
                    temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "s", "")).InnerText = pp.X.ToString();
                    temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "c", "")).InnerText = pp.Y.ToString();
                    sf.SelectSingleNode(string.Format("overview/collector1")).AppendChild(temp);
                }
                foreach (ZedGraph.PointPair pp in Graph.pointList2)
                {
                    temp = sf.CreateNode(XmlNodeType.Element, "p", "");
                    temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "s", "")).InnerText = pp.X.ToString();
                    temp.AppendChild(sf.CreateNode(XmlNodeType.Element, "c", "")).InnerText = pp.Y.ToString();
                    sf.SelectSingleNode(string.Format("overview/collector2")).AppendChild(temp);
                }
            }
            sf.Save(p);
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
                foreach (PreciseEditorData ped in Config.PreciseDataLoaded)
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
        /*
        internal static void SaveSpecterFile(string p, bool isFromFile)
        {
            System.IO.StreamWriter sf = new System.IO.StreamWriter(@p);
            sf.WriteLine(":header");
            sf.WriteLine(":col1");
            if (isFromFile) {
                foreach (ZedGraph.PointPair pp in Graph.pointListLoaded1)
                {
                    sf.WriteLine("{0:g} {1:g}", pp.X, pp.Y);
                }
                sf.WriteLine(":col2");
                foreach (ZedGraph.PointPair pp in Graph.pointListLoaded2)
                {
                    sf.WriteLine("{0:g} {1:g}", pp.X, pp.Y);
                }
            }
            else {
                foreach (ZedGraph.PointPair pp in Graph.pointList1)
                {
                    sf.WriteLine("{0:g} {1:g}", pp.X, pp.Y);
                }
                sf.WriteLine(":col2");
                foreach (ZedGraph.PointPair pp in Graph.pointList2)
                {
                    sf.WriteLine("{0:g} {1:g}", pp.X, pp.Y);
                }
            }
            sf.WriteLine(":eof");
            sf.Close();
        }
        */
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

                        foreach (XmlNode pntNode in sf.SelectNodes("/sense/region{0}/p"))
                        {
                            X = ushort.Parse(pntNode.SelectSingleNode("s").InnerText);
                            Y = int.Parse(pntNode.SelectSingleNode("c").InnerText);
                            temp.AssociatedPoints.Add(X, Y);
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    System.Windows.Forms.MessageBox.Show("Ошибка чтения файла прецизионного спектра", "Ошибка структуры файла");
                    return;
                }
                if (temp != null) peds.Add(temp);
            }
            Graph.ResetLoadedPointLists();
            Graph.updateGraph(peds);
        }
        /*
        internal static void OpenSpecterFile(string p)
        {
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            string tempstr;
            string outputString = "";
            bool readingX;
            ushort X = 0;
            int Y = 0;
            //int tempint;
            System.IO.StreamReader sf = new System.IO.StreamReader(@p);
            try
            {
                tempstr = sf.ReadLine();
                while (tempstr != ":col1")
                {
                    if (tempstr == null)
                    {
                        //Wrong format
                        return;
                    }
                    //process header?
                    tempstr = sf.ReadLine();
                }
                Graph.ResetLoadedPointLists();
                tempstr = sf.ReadLine();
                while (tempstr != ":col2")
                {
                    if (tempstr == null)
                    {
                        //Wrong format
                        return;
                    }
                    //process col1
                    readingX = true;
                    foreach (char ch in tempstr.ToCharArray())
                    {
                        if (readingX && (ch == ' '))
                        {
                            readingX = false;
                            X = Convert.ToUInt16(outputString);
                            outputString = "";
                        }
                        else
                        {
                            foreach (char compareChar in numbers)
                            {
                                if (ch == compareChar)
                                {
                                    outputString += ch;
                                    break;
                                }
                            }
                        }
                    }
                    Y = Convert.ToInt32(outputString);
                    outputString = "";
                    Graph.updateLoaded1Graph(X, Y);
                    tempstr = sf.ReadLine();
                }
                tempstr = sf.ReadLine();
                while (tempstr != ":eof")
                {
                    if (tempstr == null)
                    {
                        //Wrong format
                        return;
                    }
                    //process col2
                    readingX = true;
                    foreach (char ch in tempstr.ToCharArray())
                    {
                        if (readingX && (ch == ' '))
                        {
                            readingX = false;
                            X = Convert.ToUInt16(outputString);
                            outputString = "";
                        }
                        else
                        {
                            foreach (char compareChar in numbers)
                            {
                                if (ch == compareChar)
                                {
                                    outputString += ch;
                                    break;
                                }
                            }
                        }
                    }
                    Y = Convert.ToInt32(outputString);
                    outputString = "";
                    Graph.updateLoaded2Graph(X, Y);
                    tempstr = sf.ReadLine();
                }
                Graph.updateLoaded();
            }
            catch (System.IO.IOException)
            {
            }
            return;
        }
        */
        internal static void AutoSaveSpecterFile()
        {
            string filename;
            string dirname;
            DateTime now = System.DateTime.Now;
            dirname = initialDir + string.Format("\\{0}-{1}-{2}", now.Year, now.Month, now.Day);
            if (!System.IO.Directory.Exists(@dirname))
            { System.IO.Directory.CreateDirectory(@dirname); }
            filename = dirname + "\\" + string.Format("{0}-{1}-{2}-{3}.sdf", now.Hour, now.Minute, now.Second, now.Millisecond);
            SaveSpecterFile(@filename, false);
        }

        internal static void SavePreciseOptions(List<PreciseEditorData> ped, ushort eTimeReal, ushort mTimeReal, double iVoltageReal, double CPReal, double eCurrentReal, double hCurrentReal, double fV1Real, double fV2Real)
        {
            preciseData = ped;

            for (int i = 1; i <= 20; ++i)
            {
                _conf.SelectSingleNode(string.Format("/control/sense/region{0}/peak", i)).InnerText = "";
                _conf.SelectSingleNode(string.Format("/control/sense/region{0}/iteration", i)).InnerText = "";
                _conf.SelectSingleNode(string.Format("/control/sense/region{0}/width", i)).InnerText = "";
                _conf.SelectSingleNode(string.Format("/control/sense/region{0}/error", i)).InnerText = "";
                if (_conf.SelectSingleNode(string.Format("/control/sense/region{0}/col", i)) == null)
                {
                    XmlNode temp = _conf.CreateNode(XmlNodeType.Element, string.Format("col", i), "");
                    _conf.SelectSingleNode(string.Format("/control/sense/region{0}", i)).AppendChild(temp);
                }
                else
                {
                    _conf.SelectSingleNode(string.Format("/control/sense/region{0}/col", i)).InnerText = "";
                }
            }

            foreach (PreciseEditorData p in ped)
            {
                _conf.SelectSingleNode(string.Format("/control/sense/region{0}/peak", p.pNumber + 1)).InnerText = p.Step.ToString();
                _conf.SelectSingleNode(string.Format("/control/sense/region{0}/iteration", p.pNumber + 1)).InnerText = p.Iterations.ToString();
                _conf.SelectSingleNode(string.Format("/control/sense/region{0}/width", p.pNumber + 1)).InnerText = p.Width.ToString();
                _conf.SelectSingleNode(string.Format("/control/sense/region{0}/error", p.pNumber + 1)).InnerText = p.Precision.ToString();
                _conf.SelectSingleNode(string.Format("/control/sense/region{0}/col", p.pNumber + 1)).InnerText = p.Collector.ToString();
            }

            Config.eTimeReal = eTimeReal;
            Config.iTimeReal = mTimeReal;
            Config.iVoltageReal = iVoltageReal;
            Config.CPReal = CPReal;
            Config.eCurrentReal = eCurrentReal;
            Config.hCurrentReal = hCurrentReal;
            Config.fV1Real = fV1Real;
            Config.fV2Real = fV2Real;
            Config.SaveScanOptions();
        }

        internal static void AutoSavePreciseSpecterFile()
        {
            string filename;
            string dirname;
            DateTime now = System.DateTime.Now;
            dirname = initialDir + string.Format("\\{0}-{1}-{2}", now.Year, now.Month, now.Day);
            if (!System.IO.Directory.Exists(@dirname))
            { System.IO.Directory.CreateDirectory(@dirname); }
            filename = dirname + "\\" + string.Format("{0}-{1}-{2}-{3}.psf", now.Hour, now.Minute, now.Second, now.Millisecond);
            SavePreciseSpecterFile(@filename, false);
        }

        internal static void SavePreciseOptions(List<PreciseEditorData> ped, string pedConfName)
        {
            XmlDocument pedConf = new XmlDocument();
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
                pedConf.SelectSingleNode(string.Format("sense")).AppendChild(tempRegion);
            }
            foreach (PreciseEditorData p in ped)
            {
                pedConf.SelectSingleNode(string.Format("/sense/region{0}/peak", p.pNumber + 1)).InnerText = p.Step.ToString();
                pedConf.SelectSingleNode(string.Format("/sense/region{0}/iteration", p.pNumber + 1)).InnerText = p.Iterations.ToString();
                pedConf.SelectSingleNode(string.Format("/sense/region{0}/width", p.pNumber + 1)).InnerText = p.Width.ToString();
                pedConf.SelectSingleNode(string.Format("/sense/region{0}/error", p.pNumber + 1)).InnerText = p.Precision.ToString();
                pedConf.SelectSingleNode(string.Format("/sense/region{0}/col", p.pNumber + 1)).InnerText = p.Collector.ToString();
            }
            pedConf.Save(pedConfName);
        }

        internal static List<PreciseEditorData> LoadPreciseEditorData(string pedConfName)
        {
            XmlDocument pedConf = new XmlDocument();
            List<PreciseEditorData> ped = new List<PreciseEditorData>();
            try
            {
                pedConf.Load(pedConfName);
            }
            catch (Exception Error)
            {
                System.Windows.Forms.MessageBox.Show(Error.Message, "Ошибка чтения файла прецизионных точек");
                return null;
            }
            for (int i = 1; i <= 20; ++i)
            {
                PreciseEditorData temp = null;
                try
                {
                    bool allFilled = ((pedConf.SelectSingleNode(string.Format("/sense/region{0}/peak", i)).InnerText != "") &&
                                      (pedConf.SelectSingleNode(string.Format("/sense/region{0}/iteration", i)).InnerText != "") &&
                                      (pedConf.SelectSingleNode(string.Format("/sense/region{0}/width", i)).InnerText != "") &&
                                      (pedConf.SelectSingleNode(string.Format("/sense/region{0}/col", i)).InnerText != ""));

                    if (allFilled)
                    {
                        temp = new PreciseEditorData((byte)(i - 1),
                                                     ushort.Parse(pedConf.SelectSingleNode(string.Format("/sense/region{0}/peak", i)).InnerText),
                                                     byte.Parse(pedConf.SelectSingleNode(string.Format("/sense/region{0}/col", i)).InnerText),
                                                     ushort.Parse(pedConf.SelectSingleNode(string.Format("/sense/region{0}/iteration", i)).InnerText),
                                                     ushort.Parse(pedConf.SelectSingleNode(string.Format("/sense/region{0}/width", i)).InnerText),
                                                     (float)0);
                    }
                }
                catch (NullReferenceException)
                {
                    System.Windows.Forms.MessageBox.Show("Ошибка чтения файла прецизионных точек", "Ошибка структуры файла");
                    return null;
                }
                if (temp != null) ped.Add(temp);
            }
            return ped;
        }

        internal static void LoadPreciseEditorData()
        {
            for (int i = 1; i <= 20; ++i)
            {
                PreciseEditorData temp = null;
                try
                {
                    bool allFilled = ((_conf.SelectSingleNode(string.Format("/control/sense/region{0}/peak", i)).InnerText != "") &&
                                      (_conf.SelectSingleNode(string.Format("/control/sense/region{0}/iteration", i)).InnerText != "") &&
                                      (_conf.SelectSingleNode(string.Format("/control/sense/region{0}/width", i)).InnerText != "") &&
                                      (_conf.SelectSingleNode(string.Format("/control/sense/region{0}/col", i)).InnerText != ""));

                    if (allFilled)
                    {
                        temp = new PreciseEditorData((byte)(i - 1),
                                                     ushort.Parse(_conf.SelectSingleNode(string.Format("/control/sense/region{0}/peak", i)).InnerText),
                                                     byte.Parse(_conf.SelectSingleNode(string.Format("/control/sense/region{0}/col", i)).InnerText),
                                                     ushort.Parse(_conf.SelectSingleNode(string.Format("/control/sense/region{0}/iteration", i)).InnerText),
                                                     ushort.Parse(_conf.SelectSingleNode(string.Format("/control/sense/region{0}/width", i)).InnerText),
                                                     (float)0);
                    }
                }
                catch (NullReferenceException)
                {
                    System.Windows.Forms.MessageBox.Show("Ошибка чтения конфигурационного файла", "Ошибка структуры файла");
                }
                if (temp != null) PreciseData.Add(temp);
            }
        }

        internal static void saveCommonOptions(string fn, ushort eT, ushort iT, double iV, double cp, double eC, double hC, double fv1, double fv2)
        {
            XmlDocument cdConf = new XmlDocument();
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

            cdConf.SelectSingleNode("common/exptime").InnerText = eT.ToString();
            cdConf.SelectSingleNode("common/meastime").InnerText = iT.ToString();
            cdConf.SelectSingleNode("common/ivoltage").InnerText = iV.ToString();
            cdConf.SelectSingleNode("common/cp").InnerText = cp.ToString();
            cdConf.SelectSingleNode("common/ecurrent").InnerText = eC.ToString();
            cdConf.SelectSingleNode("common/hcurrent").InnerText = hC.ToString();
            cdConf.SelectSingleNode("common/focus1").InnerText = fv1.ToString();
            cdConf.SelectSingleNode("common/focus2").InnerText = fv2.ToString();
            
            cdConf.Save(fn);
        }

        internal static void loadCommonOptions(string cdConfName)
        {
            XmlDocument cdConf = new XmlDocument();
            try
            {
                cdConf.Load(cdConfName);
            }
            catch (Exception Error)
            {
                System.Windows.Forms.MessageBox.Show(Error.Message, "Ошибка чтения файла общих настроек");
                return;
            }
            ushort eT, iT;
            double iV, cp, eC, hC, fv1, fv2;
            try
            {
                eT = ushort.Parse(cdConf.SelectSingleNode("common/exptime").InnerText);
                iT = ushort.Parse(cdConf.SelectSingleNode("common/meastime").InnerText);
                iV = ushort.Parse(cdConf.SelectSingleNode("common/ivoltage").InnerText);
                cp = ushort.Parse(cdConf.SelectSingleNode("common/cp").InnerText);
                eC = ushort.Parse(cdConf.SelectSingleNode("common/ecurrent").InnerText);
                hC = ushort.Parse(cdConf.SelectSingleNode("common/hcurrent").InnerText);
                fv1 = ushort.Parse(cdConf.SelectSingleNode("common/focus1").InnerText);
                fv2 = ushort.Parse(cdConf.SelectSingleNode("common/focus2").InnerText);
            }
            catch (NullReferenceException)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка чтения файла общих настроек", "Ошибка структуры файла");
                return;
            }
            eTime = eT;
            iTime = iT;
            iVoltageReal = iV;
            CPReal = cp;
            eCurrentReal = eC;
            hCurrentReal = hC;
            fV1Real = fv1;
            fV2Real = fv2;
        }
    }
}