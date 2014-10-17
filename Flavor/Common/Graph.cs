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
                    instance.DisplayingMode = Displaying.Measured;
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
        [Flags]
        internal enum Recreate {
            None,
            Col1,
            Col2,
            Both = Col1 | Col2,
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
            private readonly Collector collector;
            internal bool IsFirstCollector {
                get { return collector.IsFirst; }
            }

            internal void Add(ushort pnt, long count) {
                peakSum += count;
                points[(int)DisplayValue.Step].Add(pnt, count);
                points[(int)DisplayValue.Voltage].Add(CommonOptions.scanVoltageReal(pnt), count, pnt);
                points[(int)DisplayValue.Mass].Add(collector.pointToMass(pnt), count, pnt);
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
                points[(int)DisplayValue.Voltage].ForEach((pp) => { setZ(pp); zToVoltage(pp); });

                points[(int)DisplayValue.Mass] = new PointPairListPlus(dataPoints, null, this);
                points[(int)DisplayValue.Mass].ForEach((pp) => { setZ(pp); zToMass(pp); });

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
            internal pListScaled(Collector col) {
                collector = col;
                myPED = null;//!
                points[(int)DisplayValue.Step] = new PointPairListPlus(null, this);
                points[(int)DisplayValue.Voltage] = new PointPairListPlus(null, this);
                points[(int)DisplayValue.Mass] = new PointPairListPlus(null, this);
            }
            internal pListScaled(pListScaled other) {
                collector = other.collector;
                myPED = other.PEDreference;//?
                SetRows(other);
            }
            internal pListScaled(Collector col, PointPairListPlus dataPoints) {
                collector = col;
                myPED = dataPoints.PEDreference;
                SetRows(dataPoints);
            }

            private void setZ(PointPair pp) {
                pp.Z = pp.X;
            }
            private void zToVoltage(PointPair pp) {
                pp.X = CommonOptions.scanVoltageReal((ushort)pp.Z);
            }
            private void zToMass(PointPair pp) {
                pp.X = collector.pointToMass((ushort)pp.Z);
            }
        }
        
        internal delegate void GraphEventHandler(Recreate recreate);
        internal delegate void AxisModeEventHandler();
        internal delegate void DisplayModeEventHandler(Displaying mode);

        internal event GraphEventHandler OnNewGraphData;
        internal event AxisModeEventHandler OnAxisModeChanged;
        internal event DisplayModeEventHandler OnDisplayModeChanged;

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
            private set {
                if (displayMode != value) {
                    displayMode = value;
                    //lock here?
                    if (OnDisplayModeChanged != null) {
                        OnDisplayModeChanged(value);
                    }
                }
            }
        }

        private Spectrum collectors;
        internal CommonOptions CommonOptions {
            get { return collectors.CommonOptions; }
        }
        private List<Utility.PreciseEditorData> preciseData = null;
        internal List<Utility.PreciseEditorData> PreciseData {
            get { return preciseData; }
        }
        
        private DateTime dateTime = DateTime.MaxValue;
        internal DateTime DateTime {
            get { return dateTime; }
        }
        private short? shift = null;
        internal short? Shift {
            get { return shift; }
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
        internal Collector DisplayedRows1 {
            get {
                return collectors[0];
            }
        }
        internal Collector DisplayedRows2 {
            get {
                return collectors[1];
            }
        }
        
        internal bool isPreciseSpectrum {
            get {
                if (this != instance && preciseData != null) {
                    return true;
                }
                if ((collectors[0].Count > 1) || (collectors[1].Count > 1)) {
                    return true;
                }
                return false;
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

        internal static void Reset() {
            instance.ResetPointLists();
            instance.collectors.CommonOptions = Config.CommonOptions;
            instance.preciseData = Config.PreciseData;
            instance.DisplayingMode = Displaying.Measured;
        }
        internal static void ResetForMonitor() {
            instance.ResetPointLists();
            instance.collectors.CommonOptions = Config.CommonOptions;
            instance.preciseData = Config.PreciseDataWithChecker;
            instance.DisplayingMode = Displaying.Measured;
        }
        internal static void ResetPointListsWithEvent() {
            Reset();
            instance.OnNewGraphData(Recreate.Both);
        }

        // scan mode
        internal static void updateGraphDuringScanMeasure(int y1, int y2, ushort pnt) {
            instance.collectors[0][0].Add(pnt, y1);
            instance.collectors[1][0].Add(pnt, y2);
            lastPoint = pnt;
            instance.OnNewGraphData(Recreate.None);
        }

        // precise mode
        internal static void updateGraphDuringPreciseMeasure(ushort pnt, Utility.PreciseEditorData curped) {
            lastPoint = pnt;
            curPeak = curped;
            instance.OnNewGraphData(Recreate.None);
        }
        internal static void updateGraphAfterPreciseMeasure(long[][] senseModeCounts, List<Utility.PreciseEditorData> peds, /*Obsolete*/short? shift) {
            for (int i = 0; i < peds.Count; ++i) {
                Utility.PreciseEditorData ped = peds[i];
                if (ped.Use) {
                    PointPairListPlus temp = new PointPairListPlus();
                    {
                        long[] countRow = senseModeCounts[i];
                        // really need? ped.Step += shift;
                        for (int j = 0; j < countRow.Length; ++j)
                            temp.Add(ped.Step - ped.Width + j, countRow[j]);
                    }
                    // order is important here!:
                    ped.AssociatedPoints = temp;
                    instance.collectors[ped.Collector - 1].Add(ped.AssociatedPoints);
                }
            }
            // TODO: better solution!
            instance.OnNewGraphData(Recreate.Both);
        }

        internal static void setDateTimeAndShift(DateTime dt, short? shift) {
            instance.dateTime = dt;
            instance.shift = shift;
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
        }

        internal void ResetPointLists() {
            collectors.Clear();
        }

        internal void updateGraphAfterScanLoad(PointPairListPlus pl1, PointPairListPlus pl2) {
            collectors[0][0].SetRows(pl1);
            collectors[1][0].SetRows(pl2);
        }
        internal void updateGraphAfterScanLoad(PointPairListPlus pl1, PointPairListPlus pl2, DateTime dt) {
            updateGraphAfterScanLoad(pl1, pl2);
            this.dateTime = dt;
        }
        internal void updateGraphAfterPreciseLoad(List<Utility.PreciseEditorData> peds) {
            preciseData = peds;
            foreach (Utility.PreciseEditorData ped in peds) {
                if (ped == null || ped.AssociatedPoints == null || ped.AssociatedPoints.Count == 0)
                    continue;
                // TODO: check if skipping of empty data rows can lead to program misbehaviour
                collectors[ped.Collector - 1].Add(ped.AssociatedPoints);
            }
        }
        internal void updateGraphAfterPreciseLoad(List<Utility.PreciseEditorData> peds, DateTime dt, short shift) {
            updateGraphAfterPreciseLoad(peds);
            this.dateTime = dt;
            this.shift = shift;
        }

        internal void updateGraphAfterScanDiff(PointPairListPlus pl1, PointPairListPlus pl2) {
            updateGraphAfterScanDiff(pl1, pl2, true);
        }
        internal void updateGraphAfterScanDiff(PointPairListPlus pl1, PointPairListPlus pl2, bool newData) {
            updateGraphAfterScanLoad(pl1, pl2, DateTime.MaxValue);
            DisplayingMode = Displaying.Diff;
            //lock here?
            if (newData && OnNewGraphData != null)
                OnNewGraphData(Recreate.Both);
        }
        internal void updateGraphAfterPreciseDiff(List<Utility.PreciseEditorData> peds) {
            ResetPointLists();
            updateGraphAfterPreciseDiff(peds, true);
        }
        internal void updateGraphAfterPreciseDiff(List<Utility.PreciseEditorData> peds, bool newData) {
            updateGraphAfterPreciseLoad(peds, DateTime.MaxValue, short.MaxValue);
            DisplayingMode = Displaying.Diff;
            //lock here?
            if (newData && OnNewGraphData != null)
                OnNewGraphData(Recreate.Both);
        }
        #region Graph scaling to mass coeffs
        internal bool setScalingCoeff(byte col, ushort pnt, double mass) {
            double value = mass * CommonOptions.scanVoltageReal(pnt);
            bool result = collectors.RecomputeMassRows(col, value);
            if (result && axisMode == pListScaled.DisplayValue.Mass) {
                OnNewGraphData(col == 1 ? Recreate.Col1 : Recreate.Col2);
            }
            return result;
        }
        #endregion
    }
}
