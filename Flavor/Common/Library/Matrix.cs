using System;
using System.Collections.Generic;
using dnAnalytics.LinearAlgebra;
using dnAnalytics.LinearAlgebra.Decomposition;

namespace Flavor.Common.Library {
    /// <summary>
    ///      Proxy to dnAnalytics matrix equation library.
    /// </summary>
    class Matrix: DenseMatrix {
        private readonly LU decomposition;
        public Matrix(double[,] array)
            : base(array) {
            decomposition = new LU(this);
        }
        public void Init() {
            decomposition.Solve(new DenseVector(this.Columns, 0));
        }
        public double[] Solve(List<double> input) {
            if (this.Columns != input.Count) {
                // length mismatch
                // TODO: throw smth
            }
            return decomposition.Solve(new DenseVector(input)).ToArray();
        }
    }
}
