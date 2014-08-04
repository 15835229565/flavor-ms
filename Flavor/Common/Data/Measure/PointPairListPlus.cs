namespace Flavor.Common.Data.Measure {
    class PointPairListPlus: ZedGraph.PointPairList {
        public PreciseEditorData PEDreference { get; set; }
        public ScalableDataList PLSreference { get; set; }

        public PointPairListPlus()
            : base() { }
        public PointPairListPlus(PreciseEditorData ped, ScalableDataList pls)
            : base() {
            PEDreference = ped;
            PLSreference = pls;
        }
        public PointPairListPlus(PointPairListPlus other, PreciseEditorData ped, ScalableDataList pls)
            : base(other) {
            PEDreference = ped;
            PLSreference = pls;
        }
    }
}
