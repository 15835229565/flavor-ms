using System;
using System.Collections.Generic;
using ZedGraph;

namespace Flavor.Common {
    public class Graph {
        private static Graph instance = null;
        internal static Graph Instance {
            get {
                if (instance == null) {
                    instance = new Graph(Config.CommonOptions);
                    instance.displayMode = Displaying.Measured;
                    instance.preciseData = Config.PreciseData;
                }
                return instance;
            }
        }
        internal enum Displaying {
            Measured,
            Loaded,
            Diff
        }
        public class pListScaled {
            internal enum DisplayValue {
                Step = 0,
                Voltage = 1,
                Mass = 2
            }
            private readonly Utility.PreciseEditorData myPED;
            internal Utility.PreciseEditorData PEDreference {
                get { return myPED; }
            }

            private PointPairListPlus[] points = new PointPairListPlus[3];
            internal PointPairListPlus Step {
                get { return points[(int)DisplayValue.Step]; }
            }
            internal PointPairListPlus Voltage {
                get { return points[(int)DisplayValue.Voltage]; }
            }
            internal PointPairListPlus Mass {
                get { return points[(int)DisplayValue.Mass]; }
            }
            internal PointPairListPlus Points(DisplayValue which) {
                return points[(int)which];
            }
            private long peakSum = 0;
            internal long PeakSum {
                get { return peakSum; }
            }
            internal bool isEmpty {
                get { return (points[(int)DisplayValue.Step].Count == 0); }
            }
            private bool collector;
            internal bool IsFirstCollector {
                get { return collector; }
            }

            internal void Add(ushort pnt, long count) {
                peakSum += count;
                points[(int)DisplayValue.Step].Add(pnt, count);
                points[(int)DisplayValue.Voltage].Add(Config.CommonOptions.scanVoltageReal(pnt), count, pnt);
                points[(int)DisplayValue.Mass].Add(Config.pointToMass(pnt, collector), count, pnt);
            }
            private void SetRows(pListScaled pl) {
                points[(int)DisplayValue.Step] = new PointPairListPlus(pl.Step, null, this);
                points[(int)DisplayValue.Voltage] = new PointPairListPlus(pl.Voltage, null, this);
                points[(int)DisplayValue.Mass] = new PointPairListPlus(pl.Mass, null, this);
                peakSum = pl.peakSum;
            }
            internal void SetRows(PointPairListPlus dataPoints) {
                long sum = 0;
                foreach (PointPair pp in dataPoints)
                    sum += (long)(pp.Y);
                SetRows(dataPoints, sum);
            }
            // Be careful with sumCounts!
            internal void SetRows(PointPairListPlus dataPoints, long sumCounts) {
                if (dataPoints.PLSreference == null) {
                    points[(int)DisplayValue.Step] = dataPoints;
                    dataPoints.PLSreference = this;
                } else {
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
            internal void Clear() {
                points[(int)DisplayValue.Step] = new PointPairListPlus(this.Step.PEDreference, this);
                points[(int)DisplayValue.Voltage].Clear();
                points[(int)DisplayValue.Mass].Clear();
                peakSum = 0;
            }
            internal void RecomputeMassRow() {
                points[(int)DisplayValue.Mass].ForEach(zToMass);
            }

            internal pListScaled(Utility.PreciseEditorData ped) {
                collector = (ped.Collector == 1);
                myPED = ped;
                if (ped.AssociatedPoints != null)
                    SetRows(ped.AssociatedPoints);
            }
            internal pListScaled(bool isFirstCollector) {
                collector = isFirstCollector;
                myPED = null;
                points[(int)DisplayValue.Step] = new PointPairListPlus(null, this);
                points[(int)DisplayValue.Voltage] = new PointPairListPlus(null, this);
                points[(int)DisplayValue.Mass] = new PointPairListPlus(null, this);
            }
            internal pListScaled(pListScaled other) {
                collector = other.collector;
                myPED = null;
                SetRows(other);
            }
            internal pListScaled(bool isFirstCollector, PointPairListPlus dataPoints) {
                collector = isFirstCollector;
                myPED = dataPoints.PEDreference;
                SetRows(dataPoints);
            }

            private void setZ(PointPair pp) {
                pp.Z = pp.X;
            }
            private void zToVoltage(PointPair pp) {
                pp.X = Config.CommonOptions.scanVoltageReal((ushort)pp.Z);
            }
            private void zToMass(PointPair pp) {
                pp.X = Config.pointToMass((ushort)pp.Z, collector);
            }
        }
        internal delegate void GraphEventHandler(bool recreate);
        internal delegate void AxisModeEventHandler();

        internal enum Recreate {
            None,
            Col1,
            Col2,
            Both,
        }
        internal event GraphEventHandler OnNewGraphData;
        internal event AxisModeEventHandler OnAxisModeChanged;

        private pListScaled.DisplayValue axisMode = pListScaled.DisplayValue.Step;
        internal pListScaled.DisplayValue AxisDisplayMode {
            get {
                return axisMode;
            }
            set {
                if (axisMode != value) {
                    axisMode = value;
                    OnAxisModeChanged();
                }
            }
        }
        private Displaying displayMode = Displaying.Loaded;
        internal Displaying DisplayingMode {
            get { return displayMode; }
            /*set {
                if (displayMode != value) {
                    displayMode = value;
                    // here can be event
                }
            }*/
        }

        private Spectrum collectors;
        internal CommonOptions CommonOptions {
            get { return collectors.CommonOptions; }
        }
        private PreciseSpectrum preciseData = null;
        internal PreciseSpectrum PreciseData {
            get { return preciseData; }
        }
        private List<PointPairListPlus> getPointPairs(int col, bool useAxisMode) {
            List<PointPairListPlus> temp = new List<PointPairListPlus>();
            pListScaled.DisplayValue am = pListScaled.DisplayValue.Step;
            if (useAxisMode) am = axisMode;
            foreach (pListScaled pLS in collectors[col - 1]) {
                temp.Add(pLS.Points(am));
            }
            return temp;
        }
        internal List<PointPairListPlus> Displayed1 {
            get {
                return getPointPairs(1, true);
            }
        }
        internal List<PointPairListPlus> Displayed2 {
            get {
                return getPointPairs(2, true);
            }
        }
        internal List<PointPairListPlus> Displayed1Steps {
            get {
                return getPointPairs(1, false);
            }
        }
        internal List<PointPairListPlus> Displayed2Steps {
            get {
                return getPointPairs(2, false);
            }
        }
        internal List<pListScaled> DisplayedRows1 {
            get {
                return collectors[0];
            }
        }
        internal List<pListScaled> DisplayedRows2 {
            get {
                return collectors[1];
            }
        }
        internal bool isPreciseSpectrum {
            get {
                int count1, count2;
                count1 = collectors[0].Count;
                count2 = collectors[1].Count;
                // TODO: BAD - mistaken if no data is present
                if ((count1 == 1) && (count2 == 1)) {
                    return false;
                }
                return true;
            }
        }

        #region only during measure (static)
        private static ushort lastPoint;
        internal static ushort LastPoint {
            get { return lastPoint; }
        }
        private static Utility.PreciseEditorData curPeak;
        internal static Utility.PreciseEditorData CurrentPeak {
            get { return curPeak; }
        }

        internal static void ResetPointListsWithEvent() {
            instance.ResetPointLists();
            instance.displayMode = Displaying.Measured;
            instance.OnNewGraphData(true);
        }

        // scan mode
        internal static void updateGraphDuringScanMeasure(int y1, int y2, ushort pnt) {
            (instance.collectors[0])[0].Add(pnt, y1);
            (instance.collectors[1])[0].Add(pnt, y2);
            lastPoint = pnt;
            instance.OnNewGraphData(false);
        }

        // precise mode
        internal static void updateGraphDuringPreciseMeasure(ushort pnt, Utility.PreciseEditorData curped) {
            lastPoint = pnt;
            curPeak = curped;
            instance.OnNewGraphData(false);
        }
        internal static void updateGraphAfterPreciseMeasure(long[][] senseModeCounts, List<Utility.PreciseEditorData> peds, short shift) {
            instance.ResetPointLists();
            for (int i = 0; i < peds.Count; ++i) {
                if (!peds[i].Use) {
                    // checker peak
                    continue;
                }
                pListScaled temp = new pListScaled(peds[i].Collector == 1);
                // really need? peds[i].Step += shift;
                for (int j = 0; j < senseModeCounts[i].Length; ++j) {
                    temp.Add((ushort)(peds[i].Step - peds[i].Width + j), senseModeCounts[i][j]);
                }
                instance.collectors[peds[i].Collector - 1].Add(temp);
                peds[i].AssociatedPoints = temp.Step;
            }
            instance.OnNewGraphData(true);
        }
        #endregion
        
        #region peak to add (static)
        private static Utility.PreciseEditorData peakToAdd = null;
        internal static Utility.PreciseEditorData PointToAdd {
            get { return peakToAdd; }
            set {
                if (peakToAdd != value) {
                    peakToAdd = value;
                    // TODO: lock here
                    if (OnPointAdded != null) {
                        OnPointAdded(value != null);
                    }
                }
            }
        }
        internal delegate void PointAddedDelegate(bool notNull);
        internal static event PointAddedDelegate OnPointAdded;
        #endregion
        
        internal Graph(CommonOptions commonOpts) {
            collectors = new Spectrum(commonOpts);
            //Generating blank scan spectra for either collector
            collectors[0] = new List<pListScaled>();
            collectors[0].Add(new pListScaled(true));
            collectors[1] = new List<pListScaled>();
            collectors[1].Add(new pListScaled(false));
        }

        internal void ResetPointLists() {
            collectors[0].Clear();
            collectors[0].Add(new pListScaled(true));
            collectors[1].Clear();
            collectors[1].Add(new pListScaled(false));
        }

        internal void updateGraphAfterScanLoad(PointPairListPlus pl1, PointPairListPlus pl2) {
            (collectors[0])[0].SetRows(pl1);
            (collectors[1])[0].SetRows(pl2);
        }
        internal void updateGraphAfterPreciseLoad(PreciseSpectrum peds) {
            preciseData = peds;
            ResetPointLists();
            foreach (Utility.PreciseEditorData ped in peds)
                collectors[ped.Collector - 1].Add(new pListScaled((ped.Collector == 1), ped.AssociatedPoints));
        }

        internal void updateGraphAfterScanDiff(PointPairListPlus pl1, PointPairListPlus pl2) {
            updateGraphAfterScanLoad(pl1, pl2);
            displayMode = Displaying.Diff;
            OnNewGraphData(true);
        }
        internal void updateGraphAfterPreciseDiff(List<Utility.PreciseEditorData> peds) {
            foreach (Utility.PreciseEditorData ped in peds)
                collectors[ped.Collector - 1].Add(new pListScaled((ped.Collector == 1), ped.AssociatedPoints));
            displayMode = Displaying.Diff;
            OnNewGraphData(true);
        }

        internal void RecomputeMassRows(byte col) {
            foreach (pListScaled pl in collectors[col - 1]) {
                pl.RecomputeMassRow();
            }
            if (axisMode == pListScaled.DisplayValue.Mass) {
                //TODO: Нужно заменить recreate bool -> enum, чтобы перерисовывать только нужный коллектор
                OnNewGraphData(true);
            }
        }
    }
}
