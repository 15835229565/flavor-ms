using System;
using System.Collections.Generic;
using CommonOptions = Flavor.Common.Settings.CommonOptions;

namespace Flavor.Common.Data.Measure {
    class Spectrum: List<Collector> {
        public CommonOptions CommonOptions { get; set; }
        public Spectrum(CommonOptions cd, params double[] coeffs)
            : base(coeffs.Length) {
            // better to clone here?
            CommonOptions = cd;
            if (coeffs.Length == 0)
                throw new ArgumentOutOfRangeException("coeffs");
            foreach (double coeff in coeffs)
                Add(new Collector(coeff));
        }
        public int[] RecomputeMassRows(double[] coeffs) {
            if (this.Count != coeffs.Length)
                throw new ArgumentOutOfRangeException("coeffs");
            var result = new List<int>(coeffs.Length);
            for (int i = 0; i < coeffs.Length; ++i) {
                var collector = this[i];
                double coeff = coeffs[i];
                if (coeff == collector.Coeff)
                    continue;
                collector.Coeff = coeff;
                //natural-based index
                result.Add(i + 1);
            }
            return result.ToArray();
        }
        public bool RecomputeMassRows(byte collectorNumber, double coeff) {
            //natural-based index
            if (collectorNumber > this.Count || collectorNumber < 1)
                throw new ArgumentOutOfRangeException("collectorNumber");
            var collector = this[--collectorNumber];
            if (collector.Coeff == coeff)
                return false;
            collector.Coeff = coeff;
            return true;
        }
        public new void Clear() {
            foreach (var collector in this)
                collector.Clear();
        }
    }
}
