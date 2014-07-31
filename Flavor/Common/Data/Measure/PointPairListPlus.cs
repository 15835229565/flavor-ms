namespace Flavor.Common.Data.Measure {
    class PointPairListPlus: ZedGraph.PointPairList {
        public PreciseEditorData PEDreference { get; set; }
        public Graph.pListScaled PLSreference { get; set; }

        public PointPairListPlus()
            : base() { }
        public PointPairListPlus(PreciseEditorData ped, Graph.pListScaled pls)
            : base() {
            PEDreference = ped;
            PLSreference = pls;
        }
        public PointPairListPlus(PointPairListPlus other, PreciseEditorData ped, Graph.pListScaled pls)
            : base(other) {
            PEDreference = ped;
            PLSreference = pls;
        }
    }
}
