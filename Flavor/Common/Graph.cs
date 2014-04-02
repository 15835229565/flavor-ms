using System;
using System.Collections.Generic;
using System.Linq;
using ZedGraph;
using System.Collections;

namespace Flavor.Common {
    public class Graph {
        private static MeasureGraph instance = null;
        internal static MeasureGraph Instance {
            get {
                if (instance == null) {
                    instance = new MeasureGraph(Config.CommonOptions, Config.COLLECTOR_COEFFS);
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
        // TODO: use IEnumerable instead of int[]
        internal class Recreate: IEnumerable<int> {
            public readonly static IEnumerable<int> None = Enumerable.Empty<int>();
            public readonly IEnumerable<int> All = new Recreate { };
            private Recreate() { }
            #region IEnumerable<int> Members
            public IEnumerator<int> GetEnumerator() {
                for (int i = 1; i <= Config.COLLECTOR_COEFFS.Length; ++i)
                    yield return i;
            }
            #endregion
            #region IEnumerable Members
            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }
            #endregion
        }
        // TODO: move to view, not data (Utility)
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
        
        // TODO: move to MeasureGraph (but is used on diff)
        internal delegate void GraphEventHandler(int[] recreate);
        internal event GraphEventHandler NewGraphData;
        private void OnNewGraphData(params int[] recreate) {
            //lock here?
            if (NewGraphData != null)
                NewGraphData(recreate);
        }

        internal delegate void AxisModeEventHandler();
        internal delegate void DisplayModeEventHandler(Displaying mode);
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

        internal Spectrum Collectors { get; private set; }
        internal CommonOptions CommonOptions {
            get { return Collectors.CommonOptions; }
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

        //TODO: move to view, not data
        private List<PointPairListPlus> getPointPairs(int col, bool useAxisMode) {
            List<PointPairListPlus> temp = new List<PointPairListPlus>();
            pListScaled.DisplayValue am = useAxisMode ? axisMode : pListScaled.DisplayValue.Step;
            foreach (pListScaled pLS in Collectors[col - 1]) {
                temp.Add(pLS.Points(am));
            }
            return temp;
        }
        [Obsolete]
        internal List<PointPairListPlus> Displayed1 {
            get {
                return getPointPairs(1, true);
            }
        }
        [Obsolete]
        internal List<PointPairListPlus> Displayed2 {
            get {
                return getPointPairs(2, true);
            }
        }
        [Obsolete]
        internal List<PointPairListPlus> Displayed1Steps {
            get {
                return getPointPairs(1, false);
            }
        }
        [Obsolete]
        internal List<PointPairListPlus> Displayed2Steps {
            get {
                return getPointPairs(2, false);
            }
        }
        
        internal bool isPreciseSpectrum {
            get {
                if (this != instance && preciseData != null)
                    return true;
                if (Collectors.Any(c => c.Count > 1))
                    return true;
                return false;
            }
        }

        internal class MeasureGraph: Graph {
            public ushort LastPoint { get; private set; }
            public Utility.PreciseEditorData CurrentPeak { get; private set; }

            public MeasureGraph(CommonOptions commonOpts, params double[] coeffs)
                : base(commonOpts, coeffs) { }

            public void Reset() {
                ResetPointLists();
                Collectors.CommonOptions = Config.CommonOptions;
                preciseData = Config.PreciseData;
                DisplayingMode = Displaying.Measured;
            }
            public void ResetForMonitor() {
                ResetPointLists();
                Collectors.CommonOptions = Config.CommonOptions;
                preciseData = Config.PreciseDataWithChecker;
                DisplayingMode = Displaying.Measured;
            }
            public void ResetPointListsWithEvent() {
                Reset();
                // TODO: All collectors
                OnNewGraphData(1, 2);
            }

            // scan mode
            public void updateGraphDuringScanMeasure(ushort pnt, params int[] ys) {
                int count = ys.Length;
                if (count != Collectors.Count)
                    throw new ArgumentOutOfRangeException("ys");
                for (int i = 0; i < count; ++i) {
                    Collectors[i][0].Add(pnt, ys[i]);
                }
                LastPoint = pnt;
                OnNewGraphData();
            }

            // precise mode
            public void updateGraphDuringPreciseMeasure(ushort pnt, Utility.PreciseEditorData curped) {
                LastPoint = pnt;
                CurrentPeak = curped;
                OnNewGraphData();
            }
            public void updateGraphAfterPreciseMeasure(long[][] senseModeCounts, List<Utility.PreciseEditorData> peds, /*Obsolete*/short? shift) {
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
                        Collectors[ped.Collector - 1].Add(ped.AssociatedPoints);
                    }
                }
                // TODO: only affected collectors
                OnNewGraphData(1, 2);
            }

            public void setDateTimeAndShift(DateTime dt, short? shift) {
                dateTime = dt;
                this.shift = shift;
            }
        }
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
        
        internal Graph(CommonOptions commonOpts, params double[] coeffs) {
            Collectors = new Spectrum(commonOpts, coeffs);
        }

        internal void ResetPointLists() {
            Collectors.Clear();
        }

        internal void updateGraphAfterScanLoad(params PointPairListPlus[] plists) {
            int count = plists.Length;
            if (count != Collectors.Count)
                throw new ArgumentOutOfRangeException("plists");
            for (int i = 0; i < count; ++i) {
                Collectors[i][0].SetRows(plists[i]);
            }
        }
        internal void updateGraphAfterScanLoad(DateTime dt, params PointPairListPlus[] plists) {
            this.dateTime = dt;
            updateGraphAfterScanLoad(plists);
        }
        internal void updateGraphAfterPreciseLoad(List<Utility.PreciseEditorData> peds) {
            preciseData = peds;
            foreach (Utility.PreciseEditorData ped in peds) {
                if (ped == null || ped.AssociatedPoints == null || ped.AssociatedPoints.Count == 0)
                    continue;
                // TODO: check if skipping of empty data rows can lead to program misbehaviour
                Collectors[ped.Collector - 1].Add(ped.AssociatedPoints);
            }
        }
        internal void updateGraphAfterPreciseLoad(List<Utility.PreciseEditorData> peds, DateTime dt, short shift) {
            updateGraphAfterPreciseLoad(peds);
            this.dateTime = dt;
            this.shift = shift;
        }

        internal void updateGraphAfterScanDiff(params PointPairListPlus[] plists) {
            updateGraphAfterScanDiff(true, plists);
        }
        internal void updateGraphAfterScanDiff(bool newData, params PointPairListPlus[] plists) {
            updateGraphAfterScanLoad(DateTime.MaxValue, plists);
            DisplayingMode = Displaying.Diff;
            //lock here?
            if (newData)
                // TODO: All collectors
                OnNewGraphData(1, 2);
        }
        internal void updateGraphAfterPreciseDiff(List<Utility.PreciseEditorData> peds) {
            ResetPointLists();
            updateGraphAfterPreciseDiff(peds, true);
        }
        internal void updateGraphAfterPreciseDiff(List<Utility.PreciseEditorData> peds, bool newData) {
            updateGraphAfterPreciseLoad(peds, DateTime.MaxValue, short.MaxValue);
            DisplayingMode = Displaying.Diff;
            //lock here?
            if (newData)
                // TODO: only affected collectors
                OnNewGraphData(1, 2);
        }
        #region Graph scaling to mass coeffs
        internal bool setScalingCoeff(byte col, ushort pnt, double mass) {
            double value = mass * CommonOptions.scanVoltageReal(pnt);
            bool result = Collectors.RecomputeMassRows(col, value);
            if (result && axisMode == pListScaled.DisplayValue.Mass) {
                OnNewGraphData(col);
            }
            return result;
        }
        #endregion
    }
}
