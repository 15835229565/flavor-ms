using ZedGraph;

namespace Flavor.Common.Data.Measure {
    // TODO: move to view, not data (Utility)
    // TODO: unify with PointPairSpecial from MonitorForm
    class ScalableDataList {
        public enum DisplayValue {
            Step = 0,
            Voltage = 1,
            Mass = 2
        }
        readonly PreciseEditorData myPED;
        public PreciseEditorData PEDreference {
            get { return myPED; }
        }

        readonly PointPairListPlus[] points = new PointPairListPlus[3];
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
            points[(int)DisplayValue.Voltage].Add(collector.pointToVoltage(pnt), count, pnt);
            points[(int)DisplayValue.Mass].Add(collector.pointToMass(pnt), count, pnt);
        }
        void SetRows(ScalableDataList pl) {
            points[(int)DisplayValue.Step] = new PointPairListPlus(pl.Step, null, this);
            points[(int)DisplayValue.Voltage] = new PointPairListPlus(pl.Voltage, null, this);
            points[(int)DisplayValue.Mass] = new PointPairListPlus(pl.Mass, null, this);
            peakSum = pl.peakSum;
        }
        public void SetRows(PointPairListPlus dataPoints) {
            long sum = 0;
            foreach (var pp in dataPoints)
                sum += (long)pp.Y;
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
            points[(int)DisplayValue.Voltage].ForEach(pp => ToVoltage(pp));

            points[(int)DisplayValue.Mass] = new PointPairListPlus(dataPoints, null, this);
            points[(int)DisplayValue.Mass].ForEach(pp => { setZ(pp); ToMass(pp); });

            peakSum = sumCounts;
        }
        public void Clear() {
            points[(int)DisplayValue.Step] = new PointPairListPlus(this.Step.PEDreference, this);
            points[(int)DisplayValue.Voltage].Clear();
            points[(int)DisplayValue.Mass].Clear();
            peakSum = 0;
        }
        public void RecomputeMassRow() {
            points[(int)DisplayValue.Mass].ForEach(ToMass);
        }
        public ScalableDataList(Collector col) {
            collector = col;
            myPED = null;//!
            points[(int)DisplayValue.Step] = new PointPairListPlus(null, this);
            points[(int)DisplayValue.Voltage] = new PointPairListPlus(null, this);
            points[(int)DisplayValue.Mass] = new PointPairListPlus(null, this);
        }
        public ScalableDataList(Collector col, PointPairListPlus dataPoints) {
            collector = col;
            myPED = dataPoints.PEDreference;
            SetRows(dataPoints);
        }

        void setZ(PointPair pp) {
            pp.Z = pp.X;
        }
        void ToVoltage(PointPair pp) {
            setZ(pp);
            pp.X = collector.pointToVoltage((ushort)pp.Z);
        }
        void ToMass(PointPair pp) {
            pp.X = collector.pointToMass((ushort)pp.Z);
        }
    }
}
