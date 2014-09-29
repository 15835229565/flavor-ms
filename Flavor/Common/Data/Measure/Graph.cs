using System;
using System.Collections.Generic;
using System.Linq;
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
        public event EventHandler<EventArgs<int[]>> GraphDataModified;
        protected virtual void OnGraphDataModified(params int[] recreate) {
            GraphDataModified.Raise(this, new EventArgs<int[]>(recreate));
        }

        public delegate void AxisModeEventHandler();
        public delegate void DisplayModeEventHandler(Displaying mode);
        public event AxisModeEventHandler OnAxisModeChanged;
        public event DisplayModeEventHandler OnDisplayModeChanged;

        ScalableDataList.DisplayValue axisMode = ScalableDataList.DisplayValue.Step;
        public ScalableDataList.DisplayValue AxisDisplayMode {
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
            var temp = new List<PointPairListPlus>();
            var am = useAxisMode ? axisMode : ScalableDataList.DisplayValue.Step;
            foreach (var pLS in Collectors[col - 1]) {
                temp.Add(pLS.Points(am));
            }
            return temp;
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
        
        public virtual bool isPreciseSpectrum { get { return PreciseData != null; } }

        public class MeasureGraph: Graph {
            static MeasureGraph instance = null;
            public static MeasureGraph Instance {
                get {
                    if (instance == null) {
                        instance = new MeasureGraph(Config.CommonOptions, Config.COLLECTOR_COEFFS) {
                            DisplayingMode = Displaying.Measured,
                            PreciseData = Config.PreciseData
                        };
                    }
                    return instance;
                }
            }
            public override bool isPreciseSpectrum { get { return Collectors.Any(c => c.Count > 1); } }
            public delegate void GraphEventHandler(ushort pnt, uint[] counts, params int[] recreate);
            public event GraphEventHandler NewGraphData;
            protected virtual void OnNewGraphData(ushort pnt, uint[] counts, params int[] recreate) {
                //lock here?
                if (NewGraphData != null)
                    NewGraphData(pnt, counts, recreate);
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
                OnGraphDataModified(_all);
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
                    var ped = peds[i];
                    if (ped.Use) {
                        var temp = new PointPairListPlus();
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
                // TODO: only affected collectors!
                OnGraphDataModified(_all);
            }

            void setDateTimeAndShift(DateTime dt, short? shift) {
                dateTime = dt;
                Shift = shift;
            }
            public override bool setScalingCoeff(byte col, ushort pnt, double mass) {
                if (base.setScalingCoeff(col, pnt, mass)) {
                    Config.setScalingCoeff(col, pnt, mass);
                    return true;
                }
                return false;
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

        protected readonly int[] _all;
        public Graph(CommonOptions commonOpts, params double[] coeffs) {
            Collectors = new Spectrum(commonOpts, coeffs);
            int length = coeffs.Length;
            _all = new int[length];
            for (int i = 0; i < length; ++i) {
                _all[i] = i + 1;
            }
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
            dateTime = dt;
            updateGraphAfterScanLoad(plists);
        }
        public void updateGraphAfterPreciseLoad(List<PreciseEditorData> peds) {
            PreciseData = peds;
            foreach (var ped in peds) {
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
            if (newData)
                OnGraphDataModified(_all);
        }
        public void updateGraphAfterPreciseDiff(List<PreciseEditorData> peds) {
            ResetPointLists();
            updateGraphAfterPreciseDiff(peds, true);
        }
        public void updateGraphAfterPreciseDiff(List<PreciseEditorData> peds, bool newData) {
            updateGraphAfterPreciseLoad(peds, DateTime.MaxValue, short.MaxValue);
            DisplayingMode = Displaying.Diff;
            if (newData)
                // TODO: only affected collectors!
                OnGraphDataModified(_all);
        }
        #region Graph scaling to mass coeffs
        public virtual bool setScalingCoeff(byte col, ushort pnt, double mass) {
            // this graph CommonOptions used
            double value = mass * CommonOptions.scanVoltageRealNew(pnt);
            bool result = Collectors.RecomputeMassRows(col, value);
            if (result && axisMode == ScalableDataList.DisplayValue.Mass) {
                OnGraphDataModified(col);
            }
            return result;
        }
        #endregion
    }
}
