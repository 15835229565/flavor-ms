using System;
using System.Collections.Generic;
using System.Text;
using ZedGraph;

namespace Flavor
{
    delegate void GraphEventHandler(bool fromFile, bool recreate);
    delegate void AxisModeEventHandler();

    public class pListScaled
    {
        public enum DisplayValue
        {
            Step = 0,
            Voltage = 1,
            Mass = 2
        }
        
        public PointPairList Step 
        {
            get {return points[(int)DisplayValue.Step];}
        }

        public PointPairList Voltage
        {
            get { return points[(int)DisplayValue.Voltage]; }
        }

        public PointPairList Mass
        {
            get { return points[(int)DisplayValue.Mass]; }
        }

        public bool isEmpty
        {
            get { return (points[(int)DisplayValue.Step].Count == 0); }
        }

        private bool collector;
        private PointPairList[] points = new PointPairList[3];

        public void Add(ushort pnt, int count)
        {
            points[(int)DisplayValue.Step].Add(pnt, count);
            points[(int)DisplayValue.Voltage].Add(Config.scanVoltageReal(pnt), count);
            points[(int)DisplayValue.Mass].Add(Config.pointToMass(pnt, collector), count);
        }

        public void Clear()
        {
            points[(int)DisplayValue.Step].Clear();
            points[(int)DisplayValue.Voltage].Clear();
            points[(int)DisplayValue.Mass].Clear();
        }

        public PointPairList Points(DisplayValue which)
        {
            return points[(int)which];
        }

        public pListScaled(bool isFirstCollector)
        {
            collector = isFirstCollector;
            points[(int)DisplayValue.Step] = new PointPairList();
            points[(int)DisplayValue.Voltage] = new PointPairList();
            points[(int)DisplayValue.Mass] = new PointPairList();
        }

        public pListScaled(bool isFirstCollector, PointPairList dataPoints)
        {
            collector = isFirstCollector;
            points[(int)DisplayValue.Step] = new PointPairList(dataPoints);
            (points[(int)DisplayValue.Voltage] = new PointPairList(dataPoints)).ForEach(xToVoltage);
            (points[(int)DisplayValue.Mass] = new PointPairList(dataPoints)).ForEach(xToMass);
        }

        private void xToVoltage(PointPair pp)
        {
            pp.X = Config.scanVoltageReal((ushort)pp.X);
        }

        private void xToMass(PointPair pp)
        {
            pp.X = Config.pointToMass((ushort)pp.X, collector);
        }
    }

    static class Graph
    {
        public static event GraphEventHandler OnNewGraphData;
        public static event AxisModeEventHandler OnAxisModeChanged;

        private static pListScaled.DisplayValue axisMode = pListScaled.DisplayValue.Step;
        public static pListScaled.DisplayValue AxisDisplayMode 
        {
            get
            {
                return axisMode;
            }
            set
            {
                if (axisMode != value)
                {
                    axisMode = value;
                    OnAxisModeChanged();
                }
            }
        }
        private static List<pListScaled>[] collectors = new List<pListScaled>[2];
        private static List<pListScaled>[] loadedSpectra = new List<pListScaled>[2];
        public static List<PointPairList> Collector1Steps
        {
            get 
            {
                return getPointPairs(collectors, 1, false);
            }
        }
        public static List<PointPairList> Collector2Steps
        {
            get
            {
                return getPointPairs(collectors, 2, false);
            }
        }
        public static List<PointPairList> LoadedSpectra1Steps
        {
            get
            {
                return getPointPairs(loadedSpectra, 1, false);
            }
        }
        public static List<PointPairList> LoadedSpectra2Steps
        {
            get
            {
                return getPointPairs(loadedSpectra, 2, false);
            }
        }
        public static List<PointPairList> Collector1
        {
            get
            {
                return getPointPairs(collectors, 1, true);
            }
        }
        public static List<PointPairList> Collector2
        {
            get
            {
                return getPointPairs(collectors, 2, true);
            }
        }
        public static List<PointPairList> LoadedSpectra1
        {
            get
            {
                return getPointPairs(loadedSpectra, 1, true);
            }
        }
        public static List<PointPairList> LoadedSpectra2
        {
            get
            {
                return getPointPairs(loadedSpectra, 2, true);
            }
        }
        private static List<PointPairList> getPointPairs(List<pListScaled>[] which, int col, bool useAxisMode)
        {
            List<PointPairList> temp = new List<PointPairList>();
            pListScaled.DisplayValue am = pListScaled.DisplayValue.Step;
            if (useAxisMode) am = axisMode;
            foreach (pListScaled pLS in which[col - 1])
            {
                temp.Add(pLS.Points(am));
            }
            return temp;
        }
        /*
        public static List<PointPairList> pointLists1 = new List<PointPairList>();
        public static List<PointPairList> pointListsLoaded1 = new List<PointPairList>();

        public static List<PointPairList> pointLists2 = new List<PointPairList>();
        public static List<PointPairList> pointListsLoaded2 = new List<PointPairList>();

        public static PointPairList pointList1 = new PointPairList();
        public static PointPairList pointList2 = new PointPairList();
        public static PointPairList pointListLoaded1 = new PointPairList();
        public static PointPairList pointListLoaded2 = new PointPairList();
        */
        public static ushort lastPoint;
        public static PreciseEditorData curPeak;

        static Graph()
        {
            //Generating blank scan spectra for either collector
            collectors[0] = new List<pListScaled>();
            collectors[0].Add(new pListScaled(true));
            collectors[1] = new List<pListScaled>();
            collectors[1].Add(new pListScaled(false));
            loadedSpectra[0] = new List<pListScaled>();
            loadedSpectra[0].Add(new pListScaled(true));
            loadedSpectra[1] = new List<pListScaled>();
            loadedSpectra[1].Add(new pListScaled(false));
        }

        internal static void updateGraph(int y1, int y2, ushort pnt)
        {
            (collectors[0])[0].Add(pnt, y1);
            (collectors[1])[0].Add(pnt, y2);
            //pointList1.Add(pnt, y1);
            //pointList2.Add(pnt, y2);
            lastPoint = pnt;
            OnNewGraphData(false, false);
        }

        internal static void ResetPointLists()
        {
            collectors[0].Clear();
            collectors[0].Add(new pListScaled(true));
            collectors[1].Clear();
            collectors[1].Add(new pListScaled(false));
            //pointList1.Clear();
            //pointList2.Clear();
            //pointLists1.Clear();
            //pointLists2.Clear();
            OnNewGraphData(false, true);//!!!!!!!!
        }

        internal static void updateLoaded1Graph(ushort pnt, int y)
        {
            (loadedSpectra[0])[0].Add(pnt, y);
            //pointListLoaded1.Add(pnt, y);
            //OnNewGraphData(true);
        }

        internal static void updateLoaded2Graph(ushort pnt, int y)
        {
            (loadedSpectra[1])[0].Add(pnt, y);
            //pointListLoaded2.Add(pnt, y);
            //OnNewGraphData(true);
        }

        internal static void updateLoaded() 
        {
            OnNewGraphData(true, false);
        }
        
        internal static void ResetLoadedPointLists()
        {
            loadedSpectra[0].Clear();
            loadedSpectra[0].Add(new pListScaled(true));
            loadedSpectra[1].Clear();
            loadedSpectra[1].Add(new pListScaled(false));
            //pointListLoaded1.Clear();
            //pointListLoaded2.Clear();
            //pointListsLoaded1.Clear();
            //pointListsLoaded2.Clear();
            OnNewGraphData(true, true);
        }

        internal static void updateGraph(int[][] senseModeCounts, PreciseEditorData[] peds)
        {
            ResetPointLists();
            for (int i = 0; i < peds.Length; ++i)
            {
                pListScaled temp = new pListScaled(peds[i].Collector == 1);
                //PointPairList temp = new PointPairList();
                for (int j = 0; j < senseModeCounts[i].Length; ++j)
                {
                    temp.Add((ushort)(peds[i].Step - peds[i].Width + j), senseModeCounts[i][j]);
                }
                collectors[peds[i].Collector - 1].Add(temp);
                /*
                if (peds[i].Collector == 1)
                {
                    for (int j = 0; j < senseModeCounts[i].Length; ++j)
                    {
                        temp.Add(peds[i].Step - peds[i].Width + j, senseModeCounts[i][j]);
                    }
                    //pointLists1.Add(temp);
                }
                else
                {
                    for (int j = 0; j < senseModeCounts[i].Length; ++j)
                    {
                        temp.Add(peds[i].Step - peds[i].Width + j, senseModeCounts[i][j]);
                    }
                    //pointLists2.Add(temp);
                }
                */
                peds[i].AssociatedPoints = temp.Step;
            }
            OnNewGraphData(false, true);
        }

        internal static void updateGraph(List <PreciseEditorData> peds)
        {
            ResetLoadedPointLists();
            foreach (PreciseEditorData ped in peds)
                loadedSpectra[ped.Collector - 1].Add(new pListScaled((ped.Collector == 1), ped.AssociatedPoints));
            /*
            {
                if (ped.Collector == 1)
                {
                    pointListsLoaded1.Add(ped.AssociatedPoints);
                }
                else
                {
                    pointListsLoaded2.Add(ped.AssociatedPoints);
                }
            }
            */
            OnNewGraphData(true, false);
        }

        internal static void updateGraph(ushort pnt, PreciseEditorData curped)
        {
            lastPoint = pnt;
            curPeak = curped;
            OnNewGraphData(false, false);
        }
    }
}
