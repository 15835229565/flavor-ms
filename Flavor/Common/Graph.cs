using System;
using System.Collections.Generic;
using System.Text;
using ZedGraph;

namespace Flavor
{
    delegate void GraphEventHandler(bool fromFile, bool recreate);

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

        public void Add(int count, ushort pnt)
        {
            points[(int)DisplayValue.Step].Add(pnt, count);
            points[(int)DisplayValue.Voltage].Add(Config.scanVoltage(pnt), count);
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
    }

    static class Graph
    {
        public static event GraphEventHandler OnNewGraphData;

        private static pListScaled.DisplayValue axisMode = pListScaled.DisplayValue.Step;
        private static List<pListScaled>[] collectors = new List<pListScaled>[2];
        private static List<pListScaled>[] loadedSpectra = new List<pListScaled>[2];
        public static List<PointPairList> Collector1
        {
            get 
            {
                List<PointPairList> temp = new List<PointPairList>();
                foreach (pListScaled pLS in collectors[0])
                {
                    temp.Add(pLS.Points(axisMode));
                }
                return temp;
            }
        }
        public static List<PointPairList> Collector2
        {
            get
            {
                List<PointPairList> temp = new List<PointPairList>();
                foreach (pListScaled pLS in collectors[1])
                {
                    temp.Add(pLS.Points(axisMode));
                }
                return temp;
            }
        }
        public static List<PointPairList> LoadedSpectra1
        {
            get
            {
                List<PointPairList> temp = new List<PointPairList>();
                foreach (pListScaled pLS in loadedSpectra[0])
                {
                    temp.Add(pLS.Points(axisMode));
                }
                return temp;
            }
        }
        public static List<PointPairList> LoadedSpectra2
        {
            get
            {
                List<PointPairList> temp = new List<PointPairList>();
                foreach (pListScaled pLS in loadedSpectra[1])
                {
                    temp.Add(pLS.Points(axisMode));
                }
                return temp;
            }
        }

        public static List<PointPairList> pointLists1 = new List<PointPairList>();
        public static List<PointPairList> pointListsLoaded1 = new List<PointPairList>();

        public static List<PointPairList> pointLists2 = new List<PointPairList>();
        public static List<PointPairList> pointListsLoaded2 = new List<PointPairList>();

        public static PointPairList pointList1 = new PointPairList();
        public static PointPairList pointList2 = new PointPairList();
        public static PointPairList pointListLoaded1 = new PointPairList();
        public static PointPairList pointListLoaded2 = new PointPairList();
        
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

        public static void updateGraph(int y1, int y2, ushort pnt)
        {
            pointList1.Add(pnt, y1);
            pointList2.Add(pnt, y2);
            lastPoint = pnt;
            OnNewGraphData(false, false);
        }

        internal static void ResetPointLists()
        {
            pointList1.Clear();
            pointList2.Clear();
            pointLists1.Clear();
            pointLists2.Clear();
            OnNewGraphData(false, true);//!!!!!!!!
        }

        public static void updateLoaded1Graph(ushort pnt, int y)
        {
            pointListLoaded1.Add(pnt, y);
            //OnNewGraphData(true);
        }

        public static void updateLoaded2Graph(ushort pnt, int y)
        {
            pointListLoaded2.Add(pnt, y);
            //OnNewGraphData(true);
        }

        public static void updateLoaded() 
        {
            OnNewGraphData(true, false);
        }
        
        internal static void ResetLoadedPointLists()
        {
            pointListLoaded1.Clear();
            pointListLoaded2.Clear();
            pointListsLoaded1.Clear();
            pointListsLoaded2.Clear();
            OnNewGraphData(true, true);
        }

        internal static void updateGraph(int[][] senseModeCounts, PreciseEditorData[] peds)
        {
            ResetPointLists();
            /*pointList1.Clear();
            pointList2.Clear();
            pointLists1.Clear();
            pointLists2.Clear();*/
            for (int i = 0; i < peds.Length; ++i)
            {
                PointPairList temp = new PointPairList();
                if (peds[i].Collector == 1)
                {
                    for (int j = 0; j < senseModeCounts[i].Length; ++j)
                    {
                        temp.Add(peds[i].Step - peds[i].Width + j, senseModeCounts[i][j]);
                    }
                    pointLists1.Add(temp);
                }
                else
                {
                    for (int j = 0; j < senseModeCounts[i].Length; ++j)
                    {
                        temp.Add(peds[i].Step - peds[i].Width + j, senseModeCounts[i][j]);
                    }
                    pointLists2.Add(temp);
                }
                peds[i].AssociatedPoints = temp;
            }
            OnNewGraphData(false, true);
        }

        internal static void updateGraph(List <PreciseEditorData> peds)
        {
            ResetLoadedPointLists();
            /*pointListLoaded1.Clear();
            pointListLoaded2.Clear();
            pointListsLoaded1.Clear();
            pointListsLoaded2.Clear();*/
            foreach (PreciseEditorData ped in peds)
            {
                //PointPairList temp = new PointPairList();
                if (ped.Collector == 1)
                {
                    pointListsLoaded1.Add(ped.AssociatedPoints);
                    /*foreach (PointPair pp in ped.AssociatedPoints)
                    {
                        temp.Add(pp);
                    }
                    pointLists1.Add(temp);*/
                }
                else
                {
                    pointListsLoaded2.Add(ped.AssociatedPoints);
                    /*foreach (PointPair pp in ped.AssociatedPoints)
                    {
                        temp.Add(pp);
                    }
                    pointLists2.Add(temp);*/
                }
            }
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
