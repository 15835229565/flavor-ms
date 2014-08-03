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
        readonly Converter<ushort, double> _step2voltage;
        public Collector(double coeff, Converter<ushort, double> step2voltage) {
            this.coeff = coeff;
            _step2voltage = step2voltage;
            // data row for scan (has no PED reference)
            Add(new Graph.pListScaled(this));
        }
        public double pointToVoltage(ushort pnt) {
            return _step2voltage(pnt);
        }
        public double pointToMass(ushort pnt) {
            return coeff / _step2voltage(pnt);
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
