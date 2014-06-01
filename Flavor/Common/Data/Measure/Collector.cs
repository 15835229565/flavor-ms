using System.Collections.Generic;
using CommonOptions = Flavor.Common.Settings.CommonOptions;

namespace Flavor.Common.Data.Measure {
    class Collector: List<Graph.pListScaled> {
        double coeff;
        public double Coeff {
            get { return coeff; }
            set {
                if (coeff == value)
                    return;
                coeff = value;
                foreach (var pl in this)
                    pl.RecomputeMassRow();
            }
        }

        public Collector(double coeff) {
            this.coeff = coeff;
            // data row for scan (has no PED reference)
            Add(new Graph.pListScaled(this));
        }
        public double pointToMass(ushort pnt) {
            return coeff / CommonOptions.scanVoltageReal(pnt);
        }
        public new void Clear() {
            base.Clear();
            // data row for scan (has no PED reference)
            Add(new Graph.pListScaled(this));
        }
        public void Add(PointPairListPlus ppl) {
            Add(new Graph.pListScaled(this, ppl));
        }
    }
}
