using System;
using System.Collections.Generic;
using ZedGraph;

namespace Flavor.Common {
    internal class Graph {
        private static Graph instance = null;
        internal static Graph Instance {
            get {
                if (instance == null)
                    instance = new Graph(Config.CommonOptions);
                return instance;
            }
        }
        internal enum Displaying {
            Measured,
            Loaded,
            Diff
        }
        internal class pListScaled {
            internal enum DisplayValue {
                Step = 0,
                Voltage = 1,
                Mass = 2
            }
            private readonly Utility.PreciseEditorData myPED;
            internal Utility.PreciseEditorData PEDreference {
                get { return myPED; }
                //set { myPED = value; }
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
        private Displaying displayMode = Displaying.Measured;
        internal Displaying DisplayingMode {
            get {
                return displayMode;
            }
            set {
                if (displayMode != value) {
                    displayMode = value;
                    // here can be event
                }
            }
        }

        private Spectrum collectors;
        //private Spectrum loadedSpectra = new Spectrum();
        //private Spectrum diffSpectra = new Spectrum();
        private List<PointPairListPlus> getPointPairs(Spectrum which, int col, bool useAxisMode) {
            List<PointPairListPlus> temp = new List<PointPairListPlus>();
            pListScaled.DisplayValue am = pListScaled.DisplayValue.Step;
            if (useAxisMode) am = axisMode;
            foreach (pListScaled pLS in which[col - 1]) {
                temp.Add(pLS.Points(am));
            }
            return temp;
        }
        internal List<PointPairListPlus> Displayed1 {
            get {
                return getPointPairs(collectors, 1, true);
                /*switch (displayMode) {
                    case Displaying.Loaded:
                        return getPointPairs(loadedSpectra, 1, true);
                    case Displaying.Measured:
                        return getPointPairs(collectors, 1, true);
                    case Displaying.Diff:
                        return getPointPairs(diffSpectra, 1, true);
                    default:
                        throw new ArgumentOutOfRangeException();
                }*/
            }
        }
        internal List<PointPairListPlus> Displayed2 {
            get {
                return getPointPairs(collectors, 2, true);
                /*switch (displayMode) {
                    case Displaying.Loaded:
                        return getPointPairs(loadedSpectra, 2, true);
                    case Displaying.Measured:
                        return getPointPairs(collectors, 2, true);
                    case Displaying.Diff:
                        return getPointPairs(diffSpectra, 2, true);
                    default:
                        throw new ArgumentOutOfRangeException();
                }*/
            }
        }
        internal List<PointPairListPlus> Displayed1Steps {
            get {
                return getPointPairs(collectors, 1, false);
                /*switch (displayMode) {
                    case Displaying.Loaded:
                        return getPointPairs(loadedSpectra, 1, false);
                    case Displaying.Measured:
                        return getPointPairs(collectors, 1, false);
                    case Displaying.Diff:
                        return getPointPairs(diffSpectra, 1, false);
                    default:
                        throw new ArgumentOutOfRangeException();
                }*/
            }
        }
        internal List<PointPairListPlus> Displayed2Steps {
            get {
                return getPointPairs(collectors, 2, false);
                /*switch (displayMode) {
                    case Displaying.Loaded:
                        return getPointPairs(loadedSpectra, 2, false);
                    case Displaying.Measured:
                        return getPointPairs(collectors, 2, false);
                    case Displaying.Diff:
                        return getPointPairs(diffSpectra, 2, false);
                    default:
                        throw new ArgumentOutOfRangeException();
                }*/
            }
        }
        internal List<pListScaled> DisplayedRows1 {
            get {
                return collectors[0];
                /*switch (displayMode) {
                    case Displaying.Loaded:
                        return loadedSpectra[0];
                    case Displaying.Measured:
                        return collectors[0];
                    case Displaying.Diff:
                        return diffSpectra[0];
                    default:
                        throw new ArgumentOutOfRangeException();
                }*/
            }
        }
        internal List<pListScaled> DisplayedRows2 {
            get {
                return collectors[1];
                /*switch (displayMode) {
                    case Displaying.Loaded:
                        return loadedSpectra[1];
                    case Displaying.Measured:
                        return collectors[1];
                    case Displaying.Diff:
                        return diffSpectra[1];
                    default:
                        throw new ArgumentOutOfRangeException();
                }*/
            }
        }
        internal bool isPreciseSpectrum {
            get {
                int count1, count2;
                count1 = collectors[0].Count;
                count2 = collectors[1].Count;
                /*switch (displayMode) {
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
                }*/
                if ((count1 == 1) && (count2 == 1)) {
                    return false;
                }
                return true;
            }
        }

        private ushort lastPoint;
        internal ushort LastPoint {
            get { return lastPoint; }
            //set { lastPoint = value; }
        }
        private Utility.PreciseEditorData curPeak;
        internal Utility.PreciseEditorData CurrentPeak {
            get { return curPeak; }
            //set { curPeak = value; }
        }
        private Utility.PreciseEditorData peakToAdd = null;
        internal Utility.PreciseEditorData PointToAdd {
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
        internal event PointAddedDelegate OnPointAdded;

        internal Graph(CommonOptions commonOpts) {
            collectors = new Spectrum(commonOpts);
            //Generating blank scan spectra for either collector
            collectors[0] = new List<pListScaled>();
            collectors[0].Add(new pListScaled(true));
            collectors[1] = new List<pListScaled>();
            collectors[1].Add(new pListScaled(false));
            /*loadedSpectra[0] = new List<pListScaled>();
            loadedSpectra[0].Add(new pListScaled(true));
            loadedSpectra[1] = new List<pListScaled>();
            loadedSpectra[1].Add(new pListScaled(false));
            diffSpectra[0] = new List<pListScaled>();
            diffSpectra[0].Add(new pListScaled(true));
            diffSpectra[1] = new List<pListScaled>();
            diffSpectra[1].Add(new pListScaled(false));*/
        }

        internal void updateGraph(int y1, int y2, ushort pnt) {
            (collectors[0])[0].Add(pnt, y1);
            (collectors[1])[0].Add(pnt, y2);
            lastPoint = pnt;
            OnNewGraphData(false);
        }

        internal void ResetPointLists() {
            collectors[0].Clear();
            collectors[0].Add(new pListScaled(true));
            collectors[1].Clear();
            collectors[1].Add(new pListScaled(false));
            displayMode = Displaying.Measured;
            OnNewGraphData(true);
        }

        internal void updateLoaded1Graph(ushort pnt, int y) {
            (collectors[0])[0].Add(pnt, y);
            //(loadedSpectra[0])[0].Add(pnt, y);
        }

        internal void updateLoaded2Graph(ushort pnt, int y) {
            (collectors[1])[0].Add(pnt, y);
            //(loadedSpectra[1])[0].Add(pnt, y);
        }

        internal void updateLoaded(PointPairListPlus pl1, PointPairListPlus pl2) {
            (collectors[0])[0].SetRows(pl1);
            (collectors[1])[0].SetRows(pl2);
            /*(loadedSpectra[0])[0].SetRows(pl1);
            (loadedSpectra[1])[0].SetRows(pl2);*/
        }

        internal void updateGraphAfterScanDiff(PointPairListPlus pl1, PointPairListPlus pl2) {
            DisplayedRows1[0].SetRows(pl1);
            DisplayedRows2[0].SetRows(pl2);
            displayMode = Displaying.Diff;
            OnNewGraphData(true);
        }

        internal void ResetLoadedPointLists() {
            collectors[0].Clear();
            collectors[0].Add(new pListScaled(true));
            collectors[1].Clear();
            collectors[1].Add(new pListScaled(false));
            /*loadedSpectra[0].Clear();
            loadedSpectra[0].Add(new pListScaled(true));
            loadedSpectra[1].Clear();
            loadedSpectra[1].Add(new pListScaled(false));*/
            displayMode = Displaying.Loaded;
        }

        internal void updateGraphAfterPreciseMeasure(long[][] senseModeCounts, List<Utility.PreciseEditorData> peds, short shift) {
            ResetPointLists();
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
                collectors[peds[i].Collector - 1].Add(temp);
                peds[i].AssociatedPoints = temp.Step;
            }
            OnNewGraphData(true);
        }
        internal void updateGraphAfterPreciseDiff(List<Utility.PreciseEditorData> peds) {
            foreach (Utility.PreciseEditorData ped in peds)
                collectors[ped.Collector - 1].Add(new pListScaled((ped.Collector == 1), ped.AssociatedPoints));
                //diffSpectra[ped.Collector - 1].Add(new pListScaled((ped.Collector == 1), ped.AssociatedPoints));
            displayMode = Displaying.Diff;
            OnNewGraphData(true);
        }
        internal void updateGraphAfterPreciseLoad(PreciseSpectrum peds) {
            ResetLoadedPointLists();
            foreach (Utility.PreciseEditorData ped in peds)
                collectors[ped.Collector - 1].Add(new pListScaled((ped.Collector == 1), ped.AssociatedPoints));
                //loadedSpectra[ped.Collector - 1].Add(new pListScaled((ped.Collector == 1), ped.AssociatedPoints));
        }

        internal void updateGraphDuringPreciseMeasure(ushort pnt, Utility.PreciseEditorData curped) {
            lastPoint = pnt;
            curPeak = curped;
            OnNewGraphData(false);
        }

        internal void RecomputeMassRows(byte col) {
            foreach (pListScaled pl in collectors[col - 1]) {
                pl.RecomputeMassRow();
            }
            /*foreach (pListScaled pl in loadedSpectra[col - 1]) {
                pl.RecomputeMassRow();
            }
            foreach (pListScaled pl in diffSpectra[col - 1]) {
                pl.RecomputeMassRow();
            }*/
            if (axisMode == pListScaled.DisplayValue.Mass) {
                //TODO: Нужно заменить recreate bool -> enum, чтобы перерисовывать только нужный коллектор
                OnNewGraphData(true);
            }
        }
    }
}
