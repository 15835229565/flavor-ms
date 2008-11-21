using System;
using System.Collections.Generic;
using System.Text;
using ZedGraph;

namespace Flavor
{
    delegate void GraphEventHandler(bool fromFile, bool recreate);

    static class Graph
    {
        public static event GraphEventHandler OnNewGraphData;

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
            OnNewGraphData(true, true);
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
            OnNewGraphData(true, true);
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
            pointList1.Clear();
            pointList2.Clear();
            pointLists1.Clear();
            pointLists2.Clear();
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
            pointListLoaded1.Clear();
            pointListLoaded2.Clear();
            pointListsLoaded1.Clear();
            pointListsLoaded2.Clear();
            foreach (PreciseEditorData ped in peds)
            {
                PointPairList temp = new PointPairList();
                if (ped.Collector == 1)
                {
                    foreach (PointPair pp in ped.AssociatedPoints)
                    {
                        temp.Add(pp);
                    }
                    pointLists1.Add(temp);
                }
                else
                {
                    foreach (PointPair pp in ped.AssociatedPoints)
                    {
                        temp.Add(pp);
                    }
                    pointLists2.Add(temp);
                }
            }
            OnNewGraphData(true, true);
        }

        internal static void updateGraph(ushort pnt, PreciseEditorData curped)
        {
            lastPoint = pnt;
            curPeak = curped;
            OnNewGraphData(false, false);
        }
    }
}
