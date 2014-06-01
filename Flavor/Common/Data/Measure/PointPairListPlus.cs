namespace Flavor.Common.Data.Measure {
    class PointPairListPlus: ZedGraph.PointPairList {
        PreciseEditorData myPED;
        Graph.pListScaled myPLS;

        public PreciseEditorData PEDreference {
            get { return myPED; }
            set { myPED = value; }
        }
        public Graph.pListScaled PLSreference {
            get { return myPLS; }
            set { myPLS = value; }
        }

        public PointPairListPlus()
            : base() {
            myPED = null;
            myPLS = null;
        }
        public PointPairListPlus(PreciseEditorData ped, Graph.pListScaled pls)
            : base() {
            myPED = ped;
            myPLS = pls;
        }
        public PointPairListPlus(PointPairListPlus other, PreciseEditorData ped, Graph.pListScaled pls)
            : base(other) {
            myPED = ped;
            myPLS = pls;
        }
    }
}
