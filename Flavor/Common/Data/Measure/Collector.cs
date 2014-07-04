using System;
using System.Collections.Generic;

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
        readonly Converter<ushort, double> point2mass;
        public Collector(double coeff, Converter<ushort, double> point2mass) {
            this.coeff = coeff;
            this.point2mass = point2mass;
            // data row for scan (has no PED reference)
            Add(new Graph.pListScaled(this));
        }
        public double pointToMass(ushort pnt) {
            return coeff / point2mass(pnt);
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
