using System;
using System.Collections.Generic;
using System.Linq;
using ZedGraph;
using System.Collections;
using CommonOptions = Flavor.Common.Settings.CommonOptions;
using Config = Flavor.Common.Settings.Config;

namespace Flavor.Common.Data.Measure {
    class Graph {
        public enum Displaying {
            Measured,
            Loaded,
            Diff
        }
        // TODO: move to view, not data (Utility)
        public class pListScaled {
            public enum DisplayValue {
                Step = 0,
                Voltage = 1,
                Mass = 2
            }
            readonly PreciseEditorData myPED;
            public PreciseEditorData PEDreference {
                get { return myPED; }
            }

            PointPairListPlus[] points = new PointPairListPlus[3];
            public PointPairListPlus Step {
                get { return points[(int)DisplayValue.Step]; }
            }
            public PointPairListPlus Voltage {
                get { return points[(int)DisplayValue.Voltage]; }
            }
            public PointPairListPlus Mass {
                get { return points[(int)DisplayValue.Mass]; }
            }
            public PointPairListPlus Points(DisplayValue which) {
                return points[(int)which];
            }
            long peakSum = 0;
            public long PeakSum {
                get { return peakSum; }
            }
            public bool isEmpty {
                get { return (points[(int)DisplayValue.Step].Count == 0); }
            }
            readonly Collector collector;

            public void Add(ushort pnt, long count) {
                peakSum += count;
                points[(int)DisplayValue.Step].Add(pnt, count);
                points[(int)DisplayValue.Voltage].Add(CommonOptions.scanVoltageReal(pnt), count, pnt);
                points[(int)DisplayValue.Mass].Add(collector.pointToMass(pnt), count, pnt);
            }
            void SetRows(pListScaled pl) {
                points[(int)DisplayValue.Step] = new PointPairListPlus(pl.Step, null, this);
                points[(int)DisplayValue.Voltage] = new PointPairListPlus(pl.Voltage, null, this);
                points[(int)DisplayValue.Mass] = new PointPairListPlus(pl.Mass, null, this);
                peakSum = pl.peakSum;
            }
            public void SetRows(PointPairListPlus dataPoints) {
                long sum = 0;
                foreach (PointPair pp in dataPoints)
                    sum += (long)(pp.Y);
                SetRows(dataPoints, sum);
            }
            // Be careful with sumCounts!
            public void SetRows(PointPairListPlus dataPoints, long sumCounts) {
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
            public void Clear() {
                points[(int)DisplayValue.Step] = new PointPairListPlus(this.Step.PEDreference, this);
                points[(int)DisplayValue.Voltage].Clear();
                points[(int)DisplayValue.Mass].Clear();
                peakSum = 0;
            }
            public void RecomputeMassRow() {
                points[(int)DisplayValue.Mass].ForEach(zToMass);
            }
            public pListScaled(Collector col) {
                collector = col;
                myPED = null;//!
                points[(int)DisplayValue.Step] = new PointPairListPlus(null, this);
                points[(int)DisplayValue.Voltage] = new PointPairListPlus(null, this);
                points[(int)DisplayValue.Mass] = new PointPairListPlus(null, this);
            }
            public pListScaled(pListScaled other) {
                collector = other.collector;
                myPED = other.PEDreference;//?
                SetRows(other);
            }
            public pListScaled(Collector col, PointPairListPlus dataPoints) {
                collector = col;
                myPED = dataPoints.PEDreference;
                SetRows(dataPoints);
            }

            void setZ(PointPair pp) {
                pp.Z = pp.X;
            }
            void zToVoltage(PointPair pp) {
                pp.X = CommonOptions.scanVoltageReal((ushort)pp.Z);
            }
            void zToMass(PointPair pp) {
                pp.X = collector.pointToMass((ushort)pp.Z);
            }
        }
        
        public event EventHandler<EventArgs<int[]>> GraphDataModified;
        protected virtual void OnGraphDataModified(params int[] recreate) {
            GraphDataModified.Raise(this, new EventArgs<int[]>(recreate));
        }

        public delegate void AxisModeEventHandler();
        public delegate void DisplayModeEventHandler(Displaying mode);
        public event AxisModeEventHandler OnAxisModeChanged;
        public event DisplayModeEventHandler OnDisplayModeChanged;

        pListScaled.DisplayValue axisMode = pListScaled.DisplayValue.Step;
        public pListScaled.DisplayValue AxisDisplayMode {
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
        Displaying displayMode = Displaying.Loaded;
        public Displaying DisplayingMode {
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

        public Spectrum Collectors { get; private set; }
        public CommonOptions CommonOptions {
            get { return Collectors.CommonOptions; }
        }
        public List<PreciseEditorData> PreciseData { get; private set; }
        
        DateTime dateTime = DateTime.MaxValue;
        public DateTime DateTime {
            get { return dateTime; }
        }
        // TODO: move to monitor graph only
        public short? Shift { get; private set; }

        //TODO: move to view, not data
        List<PointPairListPlus> getPointPairs(int col, bool useAxisMode) {
            List<PointPairListPlus> temp = new List<PointPairListPlus>();
            pListScaled.DisplayValue am = useAxisMode ? axisMode : pListScaled.DisplayValue.Step;
            foreach (pListScaled pLS in Collectors[col - 1]) {
                temp.Add(pLS.Points(am));
            }
            return temp;
        }
        [Obsolete]
        public List<PointPairListPlus> Displayed1 {
            get {
                return getPointPairs(1, true);
            }
        }
        [Obsolete]
        public List<PointPairListPlus> Displayed2 {
            get {
                return getPointPairs(2, true);
            }
        }
        [Obsolete]
        public List<PointPairListPlus> Displayed1Steps {
            get {
                return getPointPairs(1, false);
            }
        }
        [Obsolete]
        public List<PointPairListPlus> Displayed2Steps {
            get {
                return getPointPairs(2, false);
            }
        }
        
        public virtual bool isPreciseSpectrum {
            get {
                if (PreciseData != null)
                    return true;
                return false;
            }
        }

        public class MeasureGraph: Graph {
            static MeasureGraph instance = null;
            public static MeasureGraph Instance {
                get {
                    if (instance == null) {
                        instance = new MeasureGraph(Config.CommonOptions, Config.COLLECTOR_COEFFS);
                        instance.DisplayingMode = Displaying.Measured;
                        instance.PreciseData = Config.PreciseData;
                    }
                    return instance;
                }
            }
            public override bool isPreciseSpectrum {
                get {
                    if (Collectors.Any(c => c.Count > 1))
                        return true;
                    return false;
                }
            }
            public delegate void GraphEventHandler(ushort pnt, uint[] counts, params int[] recreate);
            public event GraphEventHandler NewGraphData;
            protected virtual void OnNewGraphData(ushort pnt, uint[] counts, params int[] recreate) {
                //lock here?
                if (NewGraphData != null)
                    NewGraphData(pnt, counts, recreate);
                //OnGraphDataModified(recreate);
            }

            public PreciseEditorData CurrentPeak { get; private set; }

            public MeasureGraph(CommonOptions commonOpts, params double[] coeffs)
                : base(commonOpts, coeffs) { }

            public void Reset() {
                ResetPointLists();
                Collectors.CommonOptions = Config.CommonOptions;
                PreciseData = Config.PreciseData;
                DisplayingMode = Displaying.Measured;
            }
            public void ResetForMonitor() {
                ResetPointLists();
                Collectors.CommonOptions = Config.CommonOptions;
                PreciseData = Config.PreciseDataWithChecker;
                DisplayingMode = Displaying.Measured;
            }
            public void ResetPointListsWithEvent() {
                Reset();
                // TODO: All collectors
                OnGraphDataModified(1, 2, 3);
            }

            // scan mode
            public void updateGraphDuringScanMeasure(ushort pnt, params uint[] ys) {
                int count = ys.Length;
                if (count != Collectors.Count)
                    throw new ArgumentOutOfRangeException("ys");
                for (int i = 0; i < count; ++i) {
                    Collectors[i][0].Add(pnt, ys[i]);
                }
                setDateTimeAndShift(DateTime.Now, null);
                OnNewGraphData(pnt, ys);
            }

            // precise mode
            public void updateGraphDuringPreciseMeasure(ushort pnt, PreciseEditorData curped, params uint[] ys) {
                CurrentPeak = curped;
                // TODO: & peak
                OnNewGraphData(pnt, ys);
            }
            public void updateGraphAfterPreciseMeasure(long[][] senseModeCounts, List<PreciseEditorData> peds, short? shift) {
                for (int i = 0; i < peds.Count; ++i) {
                    PreciseEditorData ped = peds[i];
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
                setDateTimeAndShift(DateTime.Now, shift);
                // TODO: only affected collectors
                OnGraphDataModified(1, 2, 3);
            }

            void setDateTimeAndShift(DateTime dt, short? shift) {
                dateTime = dt;
                Shift = shift;
            }
        }
        #region peak to add (static)
        static PreciseEditorData peakToAdd = null;
        public static PreciseEditorData PointToAdd {
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
        public delegate void PointAddedDelegate(bool notNull);
        public static event PointAddedDelegate OnPointAdded;
        #endregion
        
        public Graph(CommonOptions commonOpts, params double[] coeffs) {
            Collectors = new Spectrum(commonOpts, coeffs);
        }

        void ResetPointLists() {
            Collectors.Clear();
        }

        public void updateGraphAfterScanLoad(params PointPairListPlus[] plists) {
            int count = plists.Length;
            if (count != Collectors.Count)
                throw new ArgumentOutOfRangeException("plists");
            for (int i = 0; i < count; ++i) {
                Collectors[i][0].SetRows(plists[i]);
            }
        }
        public void updateGraphAfterScanLoad(DateTime dt, params PointPairListPlus[] plists) {
            this.dateTime = dt;
            updateGraphAfterScanLoad(plists);
        }
        public void updateGraphAfterPreciseLoad(List<PreciseEditorData> peds) {
            PreciseData = peds;
            foreach (PreciseEditorData ped in peds) {
                if (ped == null || ped.AssociatedPoints == null || ped.AssociatedPoints.Count == 0)
                    continue;
                // TODO: check if skipping of empty data rows can lead to program misbehaviour
                Collectors[ped.Collector - 1].Add(ped.AssociatedPoints);
            }
        }
        public void updateGraphAfterPreciseLoad(List<PreciseEditorData> peds, DateTime dt, short shift) {
            updateGraphAfterPreciseLoad(peds);
            dateTime = dt;
            Shift = shift;
        }

        public void updateGraphAfterScanDiff(params PointPairListPlus[] plists) {
            updateGraphAfterScanDiff(true, plists);
        }
        public void updateGraphAfterScanDiff(bool newData, params PointPairListPlus[] plists) {
            updateGraphAfterScanLoad(DateTime.MaxValue, plists);
            DisplayingMode = Displaying.Diff;
            //lock here?
            if (newData)
                // TODO: All collectors
                OnGraphDataModified(1, 2, 3);
        }
        public void updateGraphAfterPreciseDiff(List<PreciseEditorData> peds) {
            ResetPointLists();
            updateGraphAfterPreciseDiff(peds, true);
        }
        public void updateGraphAfterPreciseDiff(List<PreciseEditorData> peds, bool newData) {
            updateGraphAfterPreciseLoad(peds, DateTime.MaxValue, short.MaxValue);
            DisplayingMode = Displaying.Diff;
            //lock here?
            if (newData)
                // TODO: only affected collectors
                OnGraphDataModified(1, 2, 3);
        }
        #region Graph scaling to mass coeffs
        public bool setScalingCoeff(byte col, ushort pnt, double mass) {
            // this graph CommonOptions used
            double value = mass * CommonOptions.scanVoltageRealNew(pnt);
            bool result = Collectors.RecomputeMassRows(col, value);
            if (result && axisMode == pListScaled.DisplayValue.Mass) {
                OnGraphDataModified(col);
            }
            return result;
        }
        #endregion
    }
}
