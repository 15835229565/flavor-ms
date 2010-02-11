using System;
using System.Collections.Generic;
using System.Text;
using ZedGraph;

namespace Flavor.Common
{
    static class Graph
    {
        public enum Displaying
        {
            Measured,
            Loaded,
            Diff
        }
        public class pListScaled
        {
            public enum DisplayValue
            {
                Step = 0,
                Voltage = 1,
                Mass = 2
            }
            private readonly Utility.PreciseEditorData myPED;
            public Utility.PreciseEditorData PEDreference
            {
                get { return myPED; }
                //set { myPED = value; }
            }

            private PointPairListPlus[] points = new PointPairListPlus[3];
            public PointPairListPlus Step
            {
                get { return points[(int)DisplayValue.Step]; }
            }
            public PointPairListPlus Voltage
            {
                get { return points[(int)DisplayValue.Voltage]; }
            }
            public PointPairListPlus Mass
            {
                get { return points[(int)DisplayValue.Mass]; }
            }
            public PointPairListPlus Points(DisplayValue which)
            {
                return points[(int)which];
            }
            private long peakSum = 0;
            public long PeakSum
            {
                get { return peakSum; }
            }
            public bool isEmpty
            {
                get { return (points[(int)DisplayValue.Step].Count == 0); }
            }
            private bool collector;
            public bool IsFirstCollector
            {
                get { return collector; }
            }

            public void Add(ushort pnt, int count)
            {
                peakSum += count;
                points[(int)DisplayValue.Step].Add(pnt, count);
                points[(int)DisplayValue.Voltage].Add(Config.CommonOptions.scanVoltageReal(pnt), count, pnt);
                points[(int)DisplayValue.Mass].Add(Config.pointToMass(pnt, collector), count, pnt);
            }
            private void SetRows(pListScaled pl)
            {
                points[(int)DisplayValue.Step] = new PointPairListPlus(pl.Step, null, this);
                points[(int)DisplayValue.Voltage] = new PointPairListPlus(pl.Voltage, null, this);
                points[(int)DisplayValue.Mass] = new PointPairListPlus(pl.Mass, null, this);
                peakSum = pl.peakSum;
            }
            public void SetRows(PointPairListPlus dataPoints)
            {
                long sum = 0;
                foreach (PointPair pp in dataPoints)
                    sum += (long)(pp.Y);
                SetRows(dataPoints, sum);
            }
            // Be careful with sumCounts!
            internal void SetRows(PointPairListPlus dataPoints, long sumCounts)
            {
                if (dataPoints.PLSreference == null)
                {
                    points[(int)DisplayValue.Step] = dataPoints;
                    dataPoints.PLSreference = this;
                }
                else
                {
                    points[(int)DisplayValue.Step] = new PointPairListPlus(dataPoints, null, this);
                }

                points[(int)DisplayValue.Voltage] = new PointPairListPlus(dataPoints, null, this);
                //in this order!
                points[(int)DisplayValue.Voltage].ForEach(setZ);
                points[(int)DisplayValue.Voltage].ForEach(zToVoltage);

                points[(int)DisplayValue.Mass] = new PointPairListPlus(dataPoints, null, this);
                //in this order!
                points[(int)DisplayValue.Mass].ForEach(setZ);
                points[(int)DisplayValue.Mass].ForEach(zToMass);

                peakSum = sumCounts;
            }
            public void Clear()
            {
                points[(int)DisplayValue.Step]= new PointPairListPlus(this.Step.PEDreference, this);
                points[(int)DisplayValue.Voltage].Clear();
                points[(int)DisplayValue.Mass].Clear();
                peakSum = 0;
            }
            public void RecomputeMassRow()
            {
                points[(int)DisplayValue.Mass].ForEach(zToMass);
            }

            public pListScaled(Utility.PreciseEditorData ped)
            {
                collector = (ped.Collector == 1);
                myPED = ped;
                if (ped.AssociatedPoints != null)
                    SetRows(ped.AssociatedPoints);
            }
            public pListScaled(bool isFirstCollector)
            {
                collector = isFirstCollector;
                myPED = null;
                points[(int)DisplayValue.Step] = new PointPairListPlus(null, this);
                points[(int)DisplayValue.Voltage] = new PointPairListPlus(null, this);
                points[(int)DisplayValue.Mass] = new PointPairListPlus(null, this);
            }
            public pListScaled(pListScaled other)
            {
                collector = other.collector;
                myPED = null;
                SetRows(other);
            }
            public pListScaled(bool isFirstCollector, PointPairListPlus dataPoints)
            {
                collector = isFirstCollector;
                myPED = dataPoints.PEDreference;
                SetRows(dataPoints);
            }

            private void setZ(PointPair pp)
            {
                pp.Z = pp.X;
            }
            private void zToVoltage(PointPair pp)
            {
                pp.X = Config.CommonOptions.scanVoltageReal((ushort)pp.Z);
            }
            private void zToMass(PointPair pp)
            {
                pp.X = Config.pointToMass((ushort)pp.Z, collector);
            }
        }
        public delegate void GraphEventHandler(Displaying mode, bool recreate);
        public delegate void AxisModeEventHandler();

        public enum Recreate
        {
            None,
            Col1,
            Col2,
            Both,
        }
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
        private static Displaying displayMode = Displaying.Measured;
        public static Displaying DisplayingMode
        {
            get
            {
                return displayMode;
            }
            set
            {
                if (displayMode != value)
                {
                    displayMode = value;
                }
            }
        }

        private static Spectrum collectors = new Spectrum(Config.CommonOptions);
        private static Spectrum loadedSpectra = new Spectrum();
        private static Spectrum diffSpectra = new Spectrum();
        private static List<PointPairListPlus> getPointPairs(Spectrum which, int col, bool useAxisMode)
        {
            List<PointPairListPlus> temp = new List<PointPairListPlus>();
            pListScaled.DisplayValue am = pListScaled.DisplayValue.Step;
            if (useAxisMode) am = axisMode;
            foreach (pListScaled pLS in which[col - 1])
            {
                temp.Add(pLS.Points(am));
            }
            return temp;
        }
        public static List<PointPairListPlus> Displayed1
        {
            get
            {
                switch (displayMode)
                {
                    case Displaying.Loaded:
                        return getPointPairs(loadedSpectra, 1, true);
                    case Displaying.Measured:
                        return getPointPairs(collectors, 1, true);
                    case Displaying.Diff:
                        return getPointPairs(diffSpectra, 1, true);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public static List<PointPairListPlus> Displayed2
        {
            get
            {
                switch (displayMode)
                {
                    case Displaying.Loaded:
                        return getPointPairs(loadedSpectra, 2, true);
                    case Displaying.Measured:
                        return getPointPairs(collectors, 2, true);
                    case Displaying.Diff:
                        return getPointPairs(diffSpectra, 2, true);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public static List<PointPairListPlus> Displayed1Steps
        {
            get
            {
                switch (displayMode)
                {
                    case Displaying.Loaded:
                        return getPointPairs(loadedSpectra, 1, false);
                    case Displaying.Measured:
                        return getPointPairs(collectors, 1, false);
                    case Displaying.Diff:
                        return getPointPairs(diffSpectra, 1, false);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public static List<PointPairListPlus> Displayed2Steps
        {
            get
            {
                switch (displayMode)
                {
                    case Displaying.Loaded:
                        return getPointPairs(loadedSpectra, 2, false);
                    case Displaying.Measured:
                        return getPointPairs(collectors, 2, false);
                    case Displaying.Diff:
                        return getPointPairs(diffSpectra, 2, false);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public static List<pListScaled> DisplayedRows1
        {
            get
            {
                switch (displayMode)
                {
                    case Displaying.Loaded:
                        return loadedSpectra[0];
                    case Displaying.Measured:
                        return collectors[0];
                    case Displaying.Diff:
                        return diffSpectra[0];
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public static List<pListScaled> DisplayedRows2
        {
            get
            {
                switch (displayMode)
                {
                    case Displaying.Loaded:
                        return loadedSpectra[1];
                    case Displaying.Measured:
                        return collectors[1];
                    case Displaying.Diff:
                        return diffSpectra[1];
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        public static bool isPreciseSpectrum
        {
            get 
            {
                int count1, count2;
                switch (displayMode)
                {
                    case Displaying.Loaded:
                        count1 = loadedSpectra[0].Count;
                        count2 = loadedSpectra[1].Count;
                        break;
                    case Displaying.Measured:
                        count1 = collectors[0].Count;
                        count2 = collectors[1].Count;
                        break;
                    case Displaying.Diff:
                        count1 = diffSpectra[0].Count;
                        count2 = diffSpectra[1].Count;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if ((count1 == 1) && (count2 == 1)) 
                {
                    return false;
                }
                return true;
            }
        }

        private static ushort lastPoint;
        public static ushort LastPoint
        {
            get { return lastPoint; }
            //set { lastPoint = value; }
        }
        private static Utility.PreciseEditorData curPeak;
        public static Utility.PreciseEditorData CurrentPeak
        {
            get { return curPeak; }
            //set { curPeak = value; }
        }
        private static Utility.PreciseEditorData peakToAdd = null;
        public static Utility.PreciseEditorData PointToAdd
        {
            get { return peakToAdd;}
            set { peakToAdd = value;}
        }

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
            diffSpectra[0] = new List<pListScaled>();
            diffSpectra[0].Add(new pListScaled(true));
            diffSpectra[1] = new List<pListScaled>();
            diffSpectra[1].Add(new pListScaled(false));
        }

        internal static void updateGraph(int y1, int y2, ushort pnt)
        {
            (collectors[0])[0].Add(pnt, y1);
            (collectors[1])[0].Add(pnt, y2);
            lastPoint = pnt;
            OnNewGraphData(Displaying.Measured, false);
        }

        internal static void ResetPointLists()
        {
            collectors[0].Clear();
            collectors[0].Add(new pListScaled(true));
            collectors[1].Clear();
            collectors[1].Add(new pListScaled(false));
            displayMode = Displaying.Measured;
            OnNewGraphData(displayMode, true);//!!!!!!!!
        }

        internal static void updateLoaded1Graph(ushort pnt, int y)
        {
            (loadedSpectra[0])[0].Add(pnt, y);
        }

        internal static void updateLoaded2Graph(ushort pnt, int y)
        {
            (loadedSpectra[1])[0].Add(pnt, y);
        }

        internal static void updateLoaded() 
        {
            OnNewGraphData(Displaying.Loaded, false);
        }

        internal static void updateLoaded(PointPairListPlus pl1, PointPairListPlus pl2)
        {
            (loadedSpectra[0])[0].SetRows(pl1);
            (loadedSpectra[1])[0].SetRows(pl2);
            OnNewGraphData(Displaying.Loaded, false);
        }

        internal static void updateGraphAfterScanDiff(PointPairListPlus pl1, PointPairListPlus pl2)
        {
            DisplayedRows1[0].SetRows(pl1);
            DisplayedRows2[0].SetRows(pl2);
            //displayMode = Displaying.Diff;
            OnNewGraphData(displayMode, true);
        }

        internal static void ResetLoadedPointLists()
        {
            loadedSpectra[0].Clear();
            loadedSpectra[0].Add(new pListScaled(true));
            loadedSpectra[1].Clear();
            loadedSpectra[1].Add(new pListScaled(false));
            displayMode = Displaying.Loaded;
            OnNewGraphData(displayMode, false/*true*/);
        }
        internal static void ResetDiffPointLists()
        {
            diffSpectra[0].Clear();
            diffSpectra[0].Add(new pListScaled(true));
            diffSpectra[1].Clear();
            diffSpectra[1].Add(new pListScaled(false));
            displayMode = Displaying.Diff;
            OnNewGraphData(displayMode, false/*true*/);
        }

        internal static void updateGraphAfterPreciseMeasure(int[][] senseModeCounts, Utility.PreciseEditorData[] peds)
        {
            ResetPointLists();
            for (int i = 0; i < peds.Length; ++i)
            {
                pListScaled temp = new pListScaled(peds[i].Collector == 1);
                for (int j = 0; j < senseModeCounts[i].Length; ++j)
                {
                    temp.Add((ushort)(peds[i].Step - peds[i].Width + j), senseModeCounts[i][j]);
                }
                collectors[peds[i].Collector - 1].Add(temp);
                peds[i].AssociatedPoints = temp.Step;
            }
            OnNewGraphData(displayMode/*Graph.Displaying.Measured*/, true);
        }
        internal static void updateGraphAfterPreciseDiff(List<Utility.PreciseEditorData> peds)
        {
            ResetDiffPointLists();
            foreach (Utility.PreciseEditorData ped in peds)
                diffSpectra[ped.Collector - 1].Add(new pListScaled((ped.Collector == 1), ped.AssociatedPoints));
            displayMode = Displaying.Diff;
            OnNewGraphData(displayMode/*Graph.Displaying.Diff*/, true);
        }
        internal static void updateGraphAfterPreciseLoad()
        {
            ResetLoadedPointLists();
            foreach (Utility.PreciseEditorData ped in Config.PreciseDataLoaded)
                loadedSpectra[ped.Collector - 1].Add(new pListScaled((ped.Collector == 1), ped.AssociatedPoints));
            OnNewGraphData(displayMode/*Graph.Displaying.Loaded*/, false);
        }

        internal static void updateGraphDuringPreciseMeasure(ushort pnt, Utility.PreciseEditorData curped)
        {
            lastPoint = pnt;
            curPeak = curped;
            OnNewGraphData(Displaying.Measured, false);
        }

        internal static void RecomputeMassRows(byte col)
        {
            foreach (pListScaled pl in collectors[col - 1]){
                pl.RecomputeMassRow();
            }
            foreach (pListScaled pl in loadedSpectra[col - 1])
            {
                pl.RecomputeMassRow();
            }
            foreach (pListScaled pl in diffSpectra[col - 1])
            {
                pl.RecomputeMassRow();
            }
            if (axisMode == pListScaled.DisplayValue.Mass)
            {
                //Нужно заменить recreate bool -> enum, чтобы перерисовывать только нужный коллектор
                OnNewGraphData(displayMode, true);
            }
        }
    }
}
